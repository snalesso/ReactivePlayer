using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WaveformTimeline.Commons;
using WaveformTimeline.Primitives;

namespace WaveformTimeline.Controls
{
    /// <summary>
    /// Shows and animates the position of playback in time relative to the total length of the audio stream, and lets the DJ change the playback position.
    /// </summary>
    [DisplayName(@"Progress")]
    [Description("Shows and animates the position of playback in time relative to the total length of the audio stream, and lets the DJ change the playback position.")]
    [ToolboxItem(true)]
    [TemplatePart(Name = "PART_Triangle", Type = typeof(Canvas)),
     TemplatePart(Name = "PART_ProgressLine", Type = typeof(Canvas))]
    public sealed class ProgressAnimator : BaseControl
    {
        private readonly Storyboard _trackProgressAnimationBoard = new Storyboard();
        private Canvas _indicatorCanvas;
        private readonly Line _progressLine = new Line();
        private readonly Polygon _progressIndicator = new Polygon();
        private readonly Rectangle _captureMouse = new Rectangle();
        private readonly double _indicatorWidth = 6;
        private readonly Brush _transparentBrush = new SolidColorBrush { Color = Color.FromScRgb(0, 0, 0, 0), Opacity = 0 };
        private readonly Thickness _zeroMargin = new Thickness(0);
        private IDisposable _playbackOnOffNotifier;
        private IDisposable _playbackTempoNotifier;
        private bool StoryboardStarted { get; set; }

        /// <summary>
        /// Identifies the <see cref="ProgressBarBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ProgressBarBrushProperty = DependencyProperty.Register(
            nameof(ProgressBarBrush),
            typeof(Brush),
            typeof(ProgressAnimator),
            new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(0xCD, 0xBA, 0x00, 0xFF)), OnProgressBarBrushChanged));

        private static void OnProgressBarBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as ProgressAnimator)?.OnProgressBarBrushChanged();
        }

        /// <summary>
        /// Called after the <see cref="ProgressBarBrush"/> value has changed.
        /// </summary>
        private void OnProgressBarBrushChanged()
        {
            this.Render();
        }

        /// <summary>
        /// Gets or sets a brush used to draw the track progress indicator bar.
        /// </summary>
        [Category("Brushes")]
        public Brush ProgressBarBrush
        {
            get => (Brush)this.GetValue(ProgressBarBrushProperty);
            set => this.SetValue(ProgressBarBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ProgressBarThickness" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ProgressBarThicknessProperty = DependencyProperty.Register(
                nameof(ProgressBarThickness),
                typeof(double),
                typeof(ProgressAnimator),
                new UIPropertyMetadata(2.0d, OnProgressBarThicknessChanged, OnCoerceProgressBarThickness));

        private static object OnCoerceProgressBarThickness(DependencyObject o, object value)
        {
            return (o as ProgressAnimator)?.OnCoerceProgressBarThickness((double)value) ?? value;
        }

        /// <summary>
        /// Coerces the value of <see cref="ProgressBarThickness"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="ProgressBarThickness"/></param>
        /// <returns>The adjusted value of <see cref="ProgressBarThickness"/></returns>
        private double OnCoerceProgressBarThickness(double value)
        {
            return Math.Max(value, 0.0d);
        }

        private static void OnProgressBarThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as ProgressAnimator)?.OnProgressBarThicknessChanged((double)e.NewValue);
        }

        /// <summary>
        /// Called after the <see cref="ProgressBarThickness"/> value has changed.
        /// </summary>
        /// <param name="newValue">The new value of <see cref="ProgressBarThickness"/></param>
        private void OnProgressBarThicknessChanged(double newValue)
        {
            this._progressLine.StrokeThickness = newValue;
        }

        /// <summary>
        /// Get or sets the thickness of the progress indicator bar.
        /// </summary>
        [Category("Widths")]
        public double ProgressBarThickness
        {
            get => (double)this.GetValue(ProgressBarThicknessProperty);
            set => this.SetValue(ProgressBarThicknessProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AllowRepositioning"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowRepositioningProperty = DependencyProperty.Register(
                "AllowRepositioning",
                typeof(bool),
                typeof(ProgressAnimator),
                new UIPropertyMetadata(true));

        /// <summary>
        /// Whether the mouse click to the waveform should trigger playback repositioning or not
        /// Default is True
        /// </summary>
        [Category("Playback")]
        public bool AllowRepositioning
        {
            get => (bool)this.GetValue(AllowRepositioningProperty);
            set => this.SetValue(AllowRepositioningProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.MainCanvas = this.GetTemplateChild("PART_ProgressLine") as Canvas;
            this._indicatorCanvas = this.GetTemplateChild("PART_Triangle") as Canvas;
            this._captureMouse.Fill = this._transparentBrush;
            this.MainCanvas.Children.Add(this._captureMouse);
            this.Render();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this._captureMouse.Width = this.MainCanvas.RenderSize.Width;
            this._captureMouse.Height = this.MainCanvas.RenderSize.Height;
            this.Render();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            var currentPoint = e.GetPosition(this.MainCanvas);
            // ReleaseMouseCapture();
            if (!this.AllowRepositioning || !this.MainCanvas.IsMouseOver || this.Tune is NoTune)
                return;

            var percProgress = this._waveformDimensions.PercentOfRenderedWaveform(currentPoint.X);
            double positionInChannelInSeconds = this._coverageArea.ActualPosition(percProgress
                ); //PositionOnVirtualWaveform(currentPoint) * _coverageArea.Duration() + _coverageArea.StartingPoint();
            this.Tune.Seek(TimeSpan.FromTicks(
                Math.Min(this.Tune.TotalTime().Ticks,
                    Math.Max(0, TimeSpan.FromSeconds(positionInChannelInSeconds).Ticks))));
            this.Render();
        }

        protected override void OnTuneChanged()
        {
            this.Render();
        }

        private double StartingX()
        {
            return new FiniteDouble(this._waveformDimensions.PositionOnRenderedWaveform(
                this._coverageArea.Progress(this.Tune.CurrentTime().TotalSeconds))); // this is progress within the rendered area
        }

        private double IndicatorCanvasHeight()
        {
            return this._indicatorCanvas?.RenderSize.Height ?? 0.0d;
        }

        protected override void Render()
        {
            this.Clear();
            this.MeasureArea();
            var uiContext = SynchronizationContext.Current;
            if (this.Tune == null || this.MainCanvas == null || uiContext == null || this.Tune.TotalTime().TotalSeconds <= 0 || this._waveformDimensions.AreEmpty())
                return;

            this._trackProgressAnimationBoard.Completed += this.TrackProgressAnimationBoardOnCompleted;
            this._playbackOnOffNotifier = Observable.Create<EventArgs>(o =>
                {
                    EventHandler<EventArgs> h = (s, e) => o.OnNext(e);
                    this.Tune.Transitioned += h;
                    return Disposable.Create(() => this.Tune.Transitioned -= h);
                })
                .ObserveOn(uiContext)
                .Subscribe(this.ControlProgressAnimation);

            this._playbackTempoNotifier = Observable.Create<EventArgs>(observer =>
                {
                    EventHandler<EventArgs> handler1 = (s, e) => observer.OnNext(e);
                    this.Tune.TempoShifted += handler1;
                    return Disposable.Create(() => this.Tune.TempoShifted -= handler1);
                })
                //.Throttle(TimeSpan.FromMilliseconds(100))
                .ObserveOn(uiContext)
                .Subscribe(this.AlterProgressAnimationSpeed);

            var xLocation = this.StartingX();
            this._progressIndicator.Points = new PointCollection()
            {
                new Point(xLocation - this._indicatorWidth/2.0d, this.IndicatorCanvasHeight()/2 + 1),
                new Point(xLocation + this._indicatorWidth/2.0d, this.IndicatorCanvasHeight()/2 + 1),
                new Point(xLocation, this.IndicatorCanvasHeight())
            };
            this._progressIndicator.Margin = this._zeroMargin;
            this._progressLine.X1 = this._waveformDimensions.LeftMargin();
            this._progressLine.X2 = this._waveformDimensions.LeftMargin();
            this._progressLine.Y1 = 0;
            this._progressLine.Y2 = this.MainCanvas.RenderSize.Height;
            this.MainCanvas.Children.Add(this._progressLine);
            this._indicatorCanvas.Children.Add(this._progressIndicator);
            this._progressIndicator.Fill = this.ProgressBarBrush;
            this._progressLine.Stroke = this.ProgressBarBrush;
            this.ControlProgressAnimation(EventArgs.Empty);
        }

        /// <summary>
        /// Set up the animations given the remaining time and tempo
        /// </summary>
        private void SetUpProgressAnimation()
        {
            if (!this.Tune.IsPlaybackOn())
                return;

            var sourceX = this.StartingX();
            var targetX = this._waveformDimensions.LeftMargin() + this._waveformDimensions.Width();
            if (targetX < sourceX || targetX <= 0)
                return;

            var remainingTimeInSeconds = TimeSpan.FromSeconds(this._coverageArea.Remaining(this.Tune.CurrentTime().TotalSeconds)); // Tip: do not adjust by Tune.Tempo, it will be taken into account by AlterProgressAnimationSpeed()
            DoubleAnimation XAnimation() => new DoubleAnimation(sourceX, targetX, remainingTimeInSeconds);
            var lineAnimationX1 = XAnimation();
            var lineAnimationX2 = XAnimation();
            var centerPoint = this._progressIndicator.Points[2];
            var triangleAnimation = new ThicknessAnimation(
                new Thickness(sourceX - centerPoint.X, 0, 0, 0),
                new Thickness(targetX - centerPoint.X, 0, 0, 0), remainingTimeInSeconds);

            this._trackProgressAnimationBoard.Children.Clear();
            this._trackProgressAnimationBoard.Children.Add(lineAnimationX1);
            this._trackProgressAnimationBoard.Children.Add(lineAnimationX2);
            this._trackProgressAnimationBoard.Children.Add(triangleAnimation);
            this._trackProgressAnimationBoard.Duration = remainingTimeInSeconds;

            Storyboard.SetTarget(lineAnimationX1, this._progressLine);
            Storyboard.SetTarget(lineAnimationX2, this._progressLine);
            Storyboard.SetTarget(triangleAnimation, this._progressIndicator);

            Storyboard.SetTargetProperty(lineAnimationX1, new PropertyPath(Line.X1Property));
            Storyboard.SetTargetProperty(lineAnimationX2, new PropertyPath(Line.X2Property));
            Storyboard.SetTargetProperty(triangleAnimation, new PropertyPath(MarginProperty));
            System.Windows.Media.Animation.Timeline.SetDesiredFrameRate(this._trackProgressAnimationBoard, 60);
        }

        /// <summary>
        /// Run, pause, or resume the animation
        /// </summary>
        /// <param name="e"></param>
        private void ControlProgressAnimation(EventArgs e)
        {
            if (this.Tune.IsPlaybackOn())
            {
                if (!this.StoryboardStarted)
                {
                    this.LaunchAnimation();
                }
                else
                {
                    this._trackProgressAnimationBoard.Resume(this);
                }
            }
            else
            {
                this._trackProgressAnimationBoard.Pause(this);
            }
        }

        private void LaunchAnimation()
        {
            this.SetUpProgressAnimation();
            this._trackProgressAnimationBoard.Begin(this, true);
            this._trackProgressAnimationBoard.Seek(this, TimeSpan.Zero, TimeSeekOrigin.BeginTime);
            this.AlterProgressAnimationSpeed(new PropertyChangedEventArgs(string.Empty));
            this.StoryboardStarted = true;
        }

        /// <summary>
        /// Stop the animations
        /// </summary>
        private void StopAnimations()
        {
            if (!this.StoryboardStarted)
                return;
            this.StoryboardStarted = false;
            this._trackProgressAnimationBoard.Stop(this);
        }

        private void AlterProgressAnimationSpeed(EventArgs e)
        {
            this._trackProgressAnimationBoard.SetSpeedRatio(this, this.Tune.Tempo() / 100);
        }

        /// <summary>
        /// Called when the storyboard completes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void TrackProgressAnimationBoardOnCompleted(object sender, EventArgs eventArgs)
        {
            this.StoryboardStarted = false;
        }

        private void Clear()
        {
            this._playbackTempoNotifier?.Dispose();
            this._playbackOnOffNotifier?.Dispose();
            this.StopAnimations();
            this._trackProgressAnimationBoard.Completed -= this.TrackProgressAnimationBoardOnCompleted;
            this.MainCanvas.Children.Remove(this._progressLine);
            this._indicatorCanvas?.Children.Remove(this._progressIndicator);
        }
    }
}