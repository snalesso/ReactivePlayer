using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WaveformTimeline.Commons;
using WaveformTimeline.Primitives;
using Brush = System.Windows.Media.Brush;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace WaveformTimeline.Controls.Timeline
{
    /// <summary>
    /// Displays the minute / second marks on a timeline corresponding to the total length of the audio stream.
    /// </summary>
    [DisplayName(@"Timeline")]
    [Description("Displays the minute / second marks on a timeline corresponding to the total length of the audio stream.")]
    [ToolboxItem(true)]
    [TemplatePart(Name = "PART_Timeline", Type = typeof(Canvas))]
    public sealed class Timeline : BaseControl
    {
        private readonly RedrawObservable _redrawObservable;
        private IDisposable _redrawDisposable;
        private readonly Line _timelineTickLine = new Line();
        private readonly List<Line> _timeLineTicks = new List<Line>();
        private readonly Rectangle _timelineBackgroundRegion = new Rectangle();
        private readonly List<TextBlock> _timestampTextBlocks = new List<TextBlock>();

        public Timeline()
        {
            this._redrawObservable = new RedrawObservable();
            this.Unloaded += this.OnUnloaded;
        }

        protected override void OnTuneChanged()
        {
            this._redrawObservable.Increment();
        }

        /// <summary>
        /// Identifies the <see cref="TimelineTickBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty TimelineTickBrushProperty = DependencyProperty.Register(
            "TimelineTickBrush",
            typeof(Brush),
            typeof(Timeline),
            new UIPropertyMetadata(new SolidColorBrush(Colors.Black), OnTimelineTickBrushChanged));

        private static void OnTimelineTickBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Timeline)?.OnTimelineTickBrushChanged((Brush)e.NewValue);
        }

        private void OnTimelineTickBrushChanged(Brush newBrush)
        {
            this.MainCanvas.Children.OfType<Line>().ForEach(line => line.Stroke = newBrush);
        }

        /// <summary>
        /// Color of the timeline line and ticks
        /// </summary>
        [Category("Brushes")]
        public Brush TimelineTickBrush
        {
            get => (Brush)this.GetValue(TimelineTickBrushProperty);
            set => this.SetValue(TimelineTickBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MajorTickHeight" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty MajorTickHeightProperty = DependencyProperty.Register(
            "MajorTickHeight",
            typeof(int),
            typeof(Timeline),
            new PropertyMetadata(10));

        /// <summary>
        /// Height of the major tick in px
        /// </summary>
        [Category("Display")]
        public int MajorTickHeight
        {
            get => (int)this.GetValue(MajorTickHeightProperty);
            set => this.SetValue(MajorTickHeightProperty, Math.Max(1, value));
        }

        /// <summary>
        /// Identifies the <see cref="MinorTickHeight" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty MinorTickHeightProperty = DependencyProperty.Register(
            "MinorTickHeight",
            typeof(int),
            typeof(Timeline),
            new PropertyMetadata(3));

        /// <summary>
        /// Height of the minor tick in px
        /// </summary>
        [Category("Display")]
        public int MinorTickHeight
        {
            get => (int)this.GetValue(MinorTickHeightProperty);
            set => this.SetValue(MinorTickHeightProperty, Math.Max(1, value));
        }

        /// <summary>
        /// Identifies the <see cref="EmptyTuneDurationInSeconds" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty EmptyTuneDurationInSecondsProperty = DependencyProperty.Register(
            "EmptyTuneDurationInSeconds",
            typeof(int),
            typeof(Timeline),
            new PropertyMetadata(180));

        /// <summary>
        /// Duration of the empty tune. The client might want to "fake" a duration when no tune is loaded.
        /// </summary>
        public int EmptyTuneDurationInSeconds
        {
            get => (int)this.GetValue(EmptyTuneDurationInSecondsProperty);
            set => this.SetValue(EmptyTuneDurationInSecondsProperty, Math.Max(0, value));
        }

        /// <summary>
        /// Identifies the <see cref="TimelineType" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty TimelineTypeProperty = DependencyProperty.Register(
            "TimelineType",
            typeof(TimelineType),
            typeof(Timeline),
            new PropertyMetadata(TimelineType.Constant));

        /// <summary>
        /// Whether to show the ticks at constant intervals, or show more ticks and marks toward the end
        /// </summary>
        [Category("Display")]
        public TimelineType TimelineType
        {
            get => (TimelineType)this.GetValue(TimelineTypeProperty);
            set => this.SetValue(TimelineTypeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="EndRevealingMark" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty EndRevealingMarkProperty = DependencyProperty.Register(
            "EndRevealingMark",
            typeof(ZeroToOne),
            typeof(Timeline),
            new PropertyMetadata(new ZeroToOne(0.75)));

        /// <summary>
        /// At which percentage of the Tune's length should the timeline show more frequent ticks at marks. Ignored for TimelineType.Constant.
        /// </summary>
        [Category("Display")]
        public ZeroToOne EndRevealingMark
        {
            get => (ZeroToOne)this.GetValue(EndRevealingMarkProperty);
            set => this.SetValue(EndRevealingMarkProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.MainCanvas = this.GetTemplateChild("PART_Timeline") as Canvas;

            Debug.Assert(this.MainCanvas != null, "timeline canvas cannot be null");

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
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            this._redrawObservable.Increment();
        }

        private TextBlock DrawText(string text)
        {
            return new TextBlock
            {
                FontFamily = FontFamily,
                FontStyle = FontStyle,
                FontWeight = FontWeight,
                FontStretch = FontStretch,
                FontSize = FontSize,
                Foreground = Foreground,
                Text = text
            };
        }

        private Line DrawLine(double xLocation, double bottomLoc)
        {
            return new Line
            {
                Stroke = TimelineTickBrush,
                StrokeThickness = 1.0d,
                X1 = xLocation,
                Y1 = bottomLoc,
                X2 = xLocation,
                Y2 = bottomLoc - this.MinorTickHeight
            };
        }

        private Line LinesAtMajorTickAreLonger(Line Line, TimeSpan Second, List<TimeSpan> majorTicksAt, double bottomLoc)
        {
            if (majorTicksAt.Contains(Second))
            {
                Line.Y2 = bottomLoc - this.MajorTickHeight;
            }
            return Line;
        }
        
        private TextBlock WithMargin(TextBlock tb, double loc)
        {
            tb.Margin = new Thickness(loc + 2, 0, 0, 0);
            return tb;
        }

        protected override void MeasureArea()
        {
            base.MeasureArea();

            var tune = this.MainCanvas.RenderSize.Width <= 0.0
                ? new NoTune()
                : this.Tune is NoTune || Math.Abs(new FiniteDouble(this.Tune.TotalTime().TotalSeconds, 0.0d).Value()) < 0.001
                    ? new NoTune(this.EmptyTuneDurationInSeconds) // case of no real tune having been supplied
                    : this.Tune;
            this._coverageArea = new TuneDuration(tune, this.Zoom);
            this._waveformDimensions = new WaveformDimensions(this._coverageArea, this.MainCanvas.RenderSize.Width);
        }

        /// <summary>
        /// Update the timeline ticks and minute / second marks
        /// </summary>
        protected override void Render()
        {
            this.Clear();
            this.MeasureArea();

            var timelineSource = new TimelineSource(this._coverageArea);
            var bottomLoc = this.MainCanvas.RenderSize.Height - 1;
            var firstMark = timelineSource.Beginning;
            var timelineMarkingStrategy = this.TimelineType.Strategy(this._coverageArea, firstMark, this.EndRevealingMark);
            var timelineTickLocation = new TimelineTickLocation(this._coverageArea, this._waveformDimensions);
            var listOfSeconds = timelineSource.Seconds().ToList();
            var majorTicksAt = listOfSeconds.Where(timelineMarkingStrategy.AtMajorTick).ToList();

            this._timelineTickLine.X1 = 0;
            this._timelineTickLine.X2 = this.MainCanvas.RenderSize.Width;
            this._timelineTickLine.Y1 = this.MainCanvas.RenderSize.Height;
            this._timelineTickLine.Y2 = this.MainCanvas.RenderSize.Height;
            this._timelineTickLine.Stroke = this.TimelineTickBrush;
            this._timelineBackgroundRegion.Width = this.MainCanvas.RenderSize.Width;
            this._timelineBackgroundRegion.Height = this.MainCanvas.RenderSize.Height;

            this.MainCanvas.Children.Add(this._timelineTickLine);
            this.MainCanvas.Children.Add(this._timelineBackgroundRegion);

            this._timeLineTicks.AddRange(
                listOfSeconds.Where(
                    timelineMarkingStrategy.AtMinorTick)
                    .Select(sec => (Second: sec, Location: timelineTickLocation.LocationOnXAxis(sec)))
                    .Where(t => this.MainCanvas.RenderSize.Width - t.Location >= 28.0d) // TODO what's this limit of 28.0?
                    .Select(t => this.LinesAtMajorTickAreLonger(this.DrawLine(t.Location, bottomLoc), t.Second, majorTicksAt, bottomLoc)));

            this._timestampTextBlocks.AddRange(
                majorTicksAt
                .Select(sec => (Second: sec, Location: timelineTickLocation.LocationOnXAxis(sec)))
                .Select(sec => this.WithMargin(this.DrawText(timelineSource.TimespanAsString(sec.Second)), sec.Location)));

            this._timeLineTicks.ForEach(line => this.MainCanvas.Children.Add(line));
            this._timestampTextBlocks.ForEach(tb => this.MainCanvas.Children.Add(tb));
        }

        private void Clear()
        {
            this.MainCanvas.Children.Clear(); // clear the canvas
            this._timestampTextBlocks.ForEach(textblock => this.MainCanvas.Children.Remove(textblock));
            this._timestampTextBlocks.Clear();
            this._timeLineTicks.ForEach(line => this.MainCanvas.Children.Remove(line));
            this._timeLineTicks.Clear();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this._redrawDisposable?.Dispose();
            Unloaded -= this.OnUnloaded;
        }
    }
}