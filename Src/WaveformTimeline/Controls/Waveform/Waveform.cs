using JetBrains.Annotations;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WaveformTimeline.Commons;
using WaveformTimeline.Contracts;

namespace WaveformTimeline.Controls.Waveform
{
    /// <summary>
    /// Responsible for drawing the waveform shape on the provided Canvas using data from the provided ITune instance.
    /// </summary>
    [DisplayName(@"Waveform")]
    [Description("Responsible for drawing the waveform shape on the provided Canvas using data from the provided ITune instance")]
    [ToolboxItem(true)]
    [TemplatePart(Name = "PART_Waveform", Type = typeof(Canvas))]
    public sealed class Waveform : BaseControl
    {
        public Waveform()
        {
            this._uiContext = SynchronizationContext.Current;
            this._redrawObservable = new RedrawObservable();
            Unloaded += this.OnUnloaded;
        }

        private readonly SynchronizationContext _uiContext;
        private readonly RedrawObservable _redrawObservable;
        private IDisposable _redrawDisposable;
        private readonly Path _leftPath = new Path();
        private readonly Path _rightPath = new Path();
        private readonly Line _centerLine = new Line();
        private readonly List<Line> _leftSideOffsetDashes = new List<Line>();
        private readonly List<Line> _rightSideOffsetDashes = new List<Line>();
        private IDisposable _waveformBuildDisposable;
        private RenderedToDimensions _lastRenderedToDimensions;

        private class RenderedToDimensions
        {
            public RenderedToDimensions(ITune tune, WaveformDimensions dimensions)
            {
                this.Tune = tune;
                this.Dimensions = dimensions;
            }

            public ITune Tune { get; }
            public WaveformDimensions Dimensions { get; }
        }

        /// <summary>
        /// Identifies the <see cref="LeftLevelBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LeftLevelBrushProperty = DependencyProperty.Register(
            nameof(LeftLevelBrush),
            typeof(Brush), 
            typeof(Waveform),
            new UIPropertyMetadata(new SolidColorBrush(Colors.Blue), OnLeftLevelBrushChanged));

        private static void OnLeftLevelBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as Waveform)?.OnLeftLevelBrushChanged((Brush)e.NewValue);
        }

        /// <summary>
        /// Called after the <see cref="LeftLevelBrush"/> value has changed.
        /// </summary>
        /// <param name="newValue">The new value of <see cref="LeftLevelBrush"/></param>
        private void OnLeftLevelBrushChanged(Brush newValue)
        {
            this._leftPath.Fill = newValue;
        }

        /// <summary>
        /// Gets or sets a brush used to draw the left channel output on the waveform.
        /// </summary>        
        [Category("Brushes")]
        public Brush LeftLevelBrush
        {
            get => (Brush)this.GetValue(LeftLevelBrushProperty);
            set => this.SetValue(LeftLevelBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RightLevelBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty RightLevelBrushProperty
            = DependencyProperty.Register("RightLevelBrush", typeof(Brush), typeof(Waveform),
                new UIPropertyMetadata(new SolidColorBrush(Colors.Red), OnRightLevelBrushChanged));

        private static void OnRightLevelBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as Waveform)?.OnRightLevelBrushChanged((Brush)e.NewValue);
        }

        /// <summary>
        /// Called after the <see cref="RightLevelBrush"/> value has changed.
        /// </summary>
        /// <param name="brush">The new value of <see cref="RightLevelBrush"/></param>
        private void OnRightLevelBrushChanged(Brush brush)
        {
            this._rightPath.Fill = brush;
        }

        /// <summary>
        /// Gets or sets a brush used to draw the right speaker levels on the waveform.
        /// </summary>
        [Category("Brushes")]
        public Brush RightLevelBrush
        {
            get => (Brush)this.GetValue(RightLevelBrushProperty);
            set => this.SetValue(RightLevelBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CenterLineBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CenterLineBrushProperty
            = DependencyProperty.Register("CenterLineBrush", typeof(Brush), typeof(Waveform),
                new UIPropertyMetadata(new SolidColorBrush(Colors.Black), OnCenterLineBrushChanged));

        private static void OnCenterLineBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as Waveform)?.OnCenterLineBrushChanged((Brush)e.NewValue);
        }

        /// <summary>
        /// Called after the <see cref="CenterLineBrush"/> value has changed.
        /// </summary>
        /// <param name="newValue">The new value of <see cref="CenterLineBrush"/></param>
        private void OnCenterLineBrushChanged(Brush newValue)
        {
            this._centerLine.Stroke = newValue;
        }

        /// <summary>
        /// Gets or sets a brush used to draw the center line separating left and right levels.
        /// </summary>
        [Category("Brushes")]
        public Brush CenterLineBrush
        {
            get => (Brush)this.GetValue(CenterLineBrushProperty);
            set => this.SetValue(CenterLineBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CenterLineThickness" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CenterLineThicknessProperty =
            DependencyProperty.Register("CenterLineThickness", typeof(double), typeof(Waveform),
                new UIPropertyMetadata(1.0d, OnCenterLineThicknessChanged, OnCoerceCenterLineThickness));

        private static object OnCoerceCenterLineThickness(DependencyObject o, object value)
        {
            return (o as Waveform)?.OnCoerceCenterLineThickness((double)value) ?? value;
        }

        /// <summary>
        /// Coerces the value of <see cref="CenterLineThickness"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="CenterLineThickness"/></param>
        /// <returns>The adjusted value of <see cref="CenterLineThickness"/></returns>
        private double OnCoerceCenterLineThickness(double value)
        {
            return Math.Max(value, 0.0d);
        }

        private static void OnCenterLineThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as Waveform)?.OnCenterLineThicknessChanged((double)e.NewValue);
        }

        /// <summary>
        /// Called after the <see cref="CenterLineThickness"/> value has changed.
        /// </summary>
        /// <param name="newValue">The new value of <see cref="CenterLineThickness"/></param>
        private void OnCenterLineThicknessChanged(double newValue)
        {
            this._centerLine.StrokeThickness = newValue;
        }

        /// <summary>
        /// Gets or sets the thickness of the center line separating left and right levels.
        /// </summary>
        [Category("Widths")]
        public double CenterLineThickness
        {
            get => (double)this.GetValue(CenterLineThicknessProperty);
            set => this.SetValue(CenterLineThicknessProperty, value);
        }

        public static readonly DependencyProperty WaveformResolutionProperty =
            DependencyProperty.Register("WaveformResolution", typeof(int), typeof(Waveform),
                new UIPropertyMetadata(2000, OnWaveformResolutionChanged, OnCoerceWaveformResolution));

        private static void OnWaveformResolutionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as Waveform)?._redrawObservable.Increment();
        }

        private static object OnCoerceWaveformResolution(DependencyObject d, object basevalue)
        {
            var desiredWaveformResolution = (int)basevalue;
            return Math.Max(1000, Math.Min(16000, desiredWaveformResolution));
        }

        /// <summary>
        /// Controls the density and accuracy of the generated waveform. Higher number = more detail.
        /// </summary>
        [Category("Display")]
        public int WaveformResolution
        {
            get => (int)this.GetValue(WaveformResolutionProperty);
            set => this.SetValue(WaveformResolutionProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AutoScaleWaveformCache" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty AutoScaleWaveformCacheProperty
            = DependencyProperty.Register("AutoScaleWaveformCache", typeof(bool), typeof(Waveform),
                new UIPropertyMetadata(false, OnAutoScaleWaveformCacheChanged));

        private static void OnAutoScaleWaveformCacheChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as Waveform)?.OnAutoScaleWaveformCacheChanged();
        }

        /// <summary>
        /// Called after the <see cref="AutoScaleWaveformCache"/> value has changed.
        /// </summary>
        private void OnAutoScaleWaveformCacheChanged()
        {
            this.UpdateWaveformCacheScaling();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the waveform should attempt to autoscale
        /// its render buffer in size.
        /// </summary>
        /// <remarks>
        /// If true, the control will attempt to set the waveform's bitmap cache
        /// at a resolution based on the sum of all ScaleTransforms applied
        /// in the control's visual tree heirarchy. This can make the waveform appear
        /// less blurry if a ScaleTransform is applied at a higher level.
        /// The only ScaleTransforms that are considered here are those that have 
        /// uniform vertical and horizontal scaling (generally used to "zoom in"
        /// on a window or controls).
        /// </remarks>
        [Category("Display")]
        public bool AutoScaleWaveformCache
        {
            get => (bool)this.GetValue(AutoScaleWaveformCacheProperty);
            set => this.SetValue(AutoScaleWaveformCacheProperty, value);
        }

        private IEnumerable<DependencyObject> ThisAndParentsOf(DependencyObject element)
        {
            if (element == null)
                return new List<DependencyObject>();
            var list = new List<DependencyObject> { element };
            list.AddRange(this.ThisAndParentsOf(VisualTreeHelper.GetParent(element)));
            return list;
        }

        private double AdjustedByTransformM11(Transform transform)
        {
            return (transform != null) &&
(Math.Abs(transform.Value.M12) < 0.001) &&
(Math.Abs(transform.Value.M21) < 0.001) &&
(Math.Abs(transform.Value.OffsetX) < 0.001) &&
(Math.Abs(transform.Value.OffsetY) < 0.001) &&
(Math.Abs(transform.Value.M11 - transform.Value.M22) < 0.001)
? transform.Value.M11
: 1.0;
        }

        private double TotalTransformScaleFn()
        {
            return this.ThisAndParentsOf(this).Where(element => element is Visual)
.Select(visual => this.AdjustedByTransformM11(VisualTreeHelper.GetTransform((Visual)visual)))
.Aggregate(1.0, (acc, x) => acc * x);
        }

        // ReSharper disable once UnusedMember.Local
        private double TotalTransformScale()
        {
            double totalTransform = 1.0d;
            DependencyObject currentVisualTreeElement = this;
            do
            {
                if (currentVisualTreeElement is Visual visual)
                {
                    Transform transform = VisualTreeHelper.GetTransform(visual);
                    // This condition is a way of determining if it
                    // was a uniform scale transform. Is there some better way?
                    if ((transform != null) &&
                        (Math.Abs(transform.Value.M12) < 0.001) &&
                        (Math.Abs(transform.Value.M21) < 0.001) &&
                        (Math.Abs(transform.Value.OffsetX) < 0.001) &&
                        (Math.Abs(transform.Value.OffsetY) < 0.001) &&
                        (Math.Abs(transform.Value.M11 - transform.Value.M22) < 0.001))
                    {
                        totalTransform *= transform.Value.M11;
                    }
                }
                currentVisualTreeElement = VisualTreeHelper.GetParent(currentVisualTreeElement);
            } while (currentVisualTreeElement != null);
            return totalTransform;
        }

        private void UpdateWaveformCacheScaling()
        {
            if (this.MainCanvas == null)
                return;
            BitmapCache waveformCache = (BitmapCache)this.MainCanvas.CacheMode;
            if (!this.AutoScaleWaveformCache)
            {
                waveformCache.RenderAtScale = 1.0d;
                return;
            }
            double totalTransformScale = this.TotalTransformScaleFn();
            if (Math.Abs(waveformCache.RenderAtScale - totalTransformScale) > 0.001)
                waveformCache.RenderAtScale = totalTransformScale;
        }


        protected override void OnTuneChanged()
        {
            this._redrawObservable.Increment();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.MainCanvas = this.GetTemplateChild("PART_Waveform") as Canvas;
            Debug.Assert(this.MainCanvas != null, nameof(this.MainCanvas) + " != null");
            this.MainCanvas.CacheMode = new BitmapCache();
            // Used to make the transparent regions clickable.
            this.MainCanvas.Background = new SolidColorBrush(Colors.Transparent);

            this.MainCanvas.Children.Add(this._centerLine);
            this.MainCanvas.Children.Add(this._leftPath);
            this.MainCanvas.Children.Add(this._rightPath);

            if (this.CenterLineBrush != null)
            {
                this._centerLine.X1 = 0;
                this._centerLine.X2 = this.MainCanvas.RenderSize.Width;
                this._centerLine.Y1 = this.MainCanvas.RenderSize.Height;
                this._centerLine.Y2 = this.MainCanvas.RenderSize.Height;
            }
            this.UpdateWaveformCacheScaling();

            var context = SynchronizationContext.Current;
            if (context != null && this._redrawDisposable == null)
            {
                this._redrawDisposable = this._redrawObservable.Throttle(TimeSpan.FromMilliseconds(100))
                    .ObserveOn(context)
                    .Subscribe(i => this.Render());
            }
        }

        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            base.OnTemplateChanged(oldTemplate, newTemplate);
            this.MainCanvas.Children.Clear();
            this._lastRenderedToDimensions = null;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this._redrawObservable.Increment();
        }


        private static float[] CreateFloats([NotNull]byte[] bytes)
        {
            var floats = new float[bytes.Length / 4];
            Buffer.BlockCopy(bytes, 0, floats, 0, bytes.Length);
            return floats;
        }

        private bool ShouldRedraw()
        {
            return this.MainCanvas != null && (
              this._lastRenderedToDimensions == null ||
              this.Tune.Name() != this._lastRenderedToDimensions.Tune.Name() ||
              this.Tune.TotalTime() != this._lastRenderedToDimensions.Tune.TotalTime() ||
              !this._waveformDimensions.Equals(this._lastRenderedToDimensions.Dimensions));
        }

        /// <summary>
        /// Show the waveform
        /// </summary>
        protected override void Render()
        {
            this.MeasureArea();
            if (!this.ShouldRedraw())
                return;
            this.Clear();
            var section = new WaveformSection(this._coverageArea, this.Tune, this.WaveformResolution);
            var centerHeight = this.MainCanvas.RenderSize.Height / 2.0d;
            var waveformFloats = CreateFloats(this.Tune.WaveformData());
            var availableWidth = this.MainCanvas.RenderSize.Width - this._waveformDimensions.RightMargin();
            var leftWaveformPolyLine = new PolyLineSegment();
            var rightWaveformPolyLine = new PolyLineSegment();
            Point StartPoint() => new Point(this._waveformDimensions.LeftMargin(), centerHeight);
            this._leftPath.Data = new PathGeometry(new[] { new PathFigure(StartPoint(), new[] { leftWaveformPolyLine }, false) });
            this._rightPath.Data = new PathGeometry(new[] { new PathFigure(StartPoint(), new[] { rightWaveformPolyLine }, false) });
            var renderWaveform = new WaveformRenderingProgress(this._waveformDimensions, section, this.MainCanvas, leftWaveformPolyLine, rightWaveformPolyLine);
            this._centerLine.X1 = this._waveformDimensions.LeftMargin();
            this._centerLine.X2 = availableWidth;
            var halfHeight = this.MainCanvas.RenderSize.Height / 2.0d;
            this._centerLine.Y1 = halfHeight;
            this._centerLine.Y2 = halfHeight;
            this.MainCanvas.Children.Add(this._leftPath);
            this.MainCanvas.Children.Add(this._rightPath);
            this.MainCanvas.Children.Add(this._centerLine);

            // The following section adds an empty space before the beginning of the waveform with leading dashes
            if (this.Tune.TotalTime().TotalSeconds > 0)
            {
                this.CreateDashedPadding(0, this._waveformDimensions.LeftMargin(), this._leftSideOffsetDashes);
                if (this._coverageArea.Includes(1.0))
                    this.CreateDashedPadding(availableWidth, this._waveformDimensions.RightMargin(), this._rightSideOffsetDashes);
            }
            var observable = waveformFloats.Length > 0
                             ? new CachedWaveformObservable(waveformFloats, section)
                             : this.Tune.WaveformStream();
            this._waveformBuildDisposable = observable.ObserveOn(this._uiContext)
                .Buffer(20)
                .Subscribe(
                    renderWaveform.DrawWfPointByPoint,
                    renderWaveform.CompleteWaveform);
            var resolution = this.WaveformResolution; // can't inline this because it cannot be accessed safely from another thread
            Task.Run(() => observable.Waveform(resolution));
            this._lastRenderedToDimensions = new RenderedToDimensions(this.Tune, this._waveformDimensions);
        }

        private Line DrawDash(int i, double centerPos, double startPos, int dashSize, int inInBetweenDashesSpace)
        {
            Line dash = new Line
            {
                Stroke = CenterLineBrush,
                StrokeThickness = CenterLineThickness,
                X1 = i == 0 ? startPos : startPos + i * dashSize + i * inInBetweenDashesSpace
            };
            dash.X2 = dash.X1 + dashSize;
            dash.Y1 = centerPos;
            dash.Y2 = centerPos;
            return dash;
        }

        private void CreateDashedPadding(double startPos, double spaceInPx, List<Line> dashes)
        {
            const int minDashSize = 3;
            const int maxDashCount = 5;
            const int minInBetweenDashesSpace = 3;
            int dashSize = minDashSize;
            int dashCount = Math.Min(maxDashCount, (int)Math.Floor(this._waveformDimensions.LeftMargin() / dashSize));
            var dashTotalWidth = dashCount * minDashSize;
            dashSize += Math.Max(0, (int)Math.Floor(((spaceInPx - dashTotalWidth - ((dashCount - 1) * minInBetweenDashesSpace)) / dashCount)));
            int inInBetweenDashesSpace = Math.Max(minInBetweenDashesSpace, (int)Math.Floor((this._waveformDimensions.LeftMargin() - (dashSize * dashCount)) / dashCount));
            var centerPos = this.MainCanvas.RenderSize.Height / 2;
            var lines = Enumerable.Range(0, dashCount)
                .Select(i => this.DrawDash(i, centerPos, startPos, dashSize, inInBetweenDashesSpace));
            lines.ForEach(dash =>
            {
                dashes.Add(dash);
                this.MainCanvas.Children.Add(dash);
            });
        }

        public void Clear()
        {
            this._waveformBuildDisposable?.Dispose();
            this.MainCanvas.Children.Clear();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this._redrawDisposable?.Dispose();
            Unloaded -= this.OnUnloaded;
        }
    }
}