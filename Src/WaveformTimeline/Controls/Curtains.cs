using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WaveformTimeline.Primitives;

namespace WaveformTimeline.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// Shows how the leading and trailing portions of a track can / is being cut off.
    /// Allows those portions to be defined, resized,
    /// and informs the encapsulated ITune instance about it.
    /// </summary>
    [DisplayName(@"Curtains")]
    [Description("Shows how the leading and trailing portions of a track can / is being cut off. Allows those portions to be defined, resized, and informs the encapsulated ITune instance about it.")]
    [ToolboxItem(true)]
    [TemplatePart(Name = "PART_Curtains", Type = typeof(Canvas)),
     TemplatePart(Name = "PART_LeftCurtain", Type = typeof(Canvas)),
     TemplatePart(Name = "PART_RightCurtain", Type = typeof(Canvas)),
     TemplatePart(Name = "PART_CueMarks", Type = typeof(Canvas))]
    internal sealed class Curtains : BaseControl
    {
        private Canvas _cueMarksCanvas;
        private Canvas _leftSideCurtain;
        private Canvas _rightSideCurtain;
        private readonly List<ZeroToOne> _cuePoints = new List<ZeroToOne>();
        private readonly List<Shape> _cuePointMarks = new List<Shape>();
        private readonly List<Line> _cuePointLines = new List<Line>();
        private readonly Brush _transparentBrush = new SolidColorBrush { Color = Color.FromScRgb(0, 0, 0, 0), Opacity = 0 };
        private readonly Dictionary<double, Shape> _cueMap = new Dictionary<double, Shape>();
        private ZeroToOne _selectedCuePoint;
        private Shape _selectedCuePointMark;
        private Line _selectedCuePointLine;
        private double _lastKnownGoodX;
        private Canvas _animatedCurtain;
        private bool _isMouseDown;

        /// <summary>
        /// Identifies the <see cref="CueMarkBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CueMarkBrushProperty = DependencyProperty.Register(
            nameof(CueMarkBrush),
            typeof(Brush),
            typeof(Curtains),
            new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(0xCD, 0xBA, 0x00, 0xFF)), OnCueMarkBrushChanged));

        private static void OnCueMarkBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as Curtains)?.OnCueMarkBrushChanged();
        }

        /// <summary>
        /// Called after the <see cref="Curtains.CueMarkBrush"/> value has changed.
        /// </summary>
        private void OnCueMarkBrushChanged()
        {
            this.Render();
        }

        /// <summary>
        /// Gets or sets a brush used to draw cue mark triangles.
        /// </summary>
        [Category("Brushes")]
        public Brush CueMarkBrush
        {
            get => (Brush)this.GetValue(CueMarkBrushProperty);
            set => this.SetValue(CueMarkBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CueBarBackgroundBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CueBarBackgroundBrushProperty
            = DependencyProperty.Register(
                nameof(CueBarBackgroundBrush),
                typeof(Brush),
                typeof(Curtains),
                new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(0xCD, 0xBA, 0x00, 0xFF)), OnCueBarBackgroundBrushChanged));

        private static void OnCueBarBackgroundBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as Curtains)?.OnCueBarBackgroundBrushChanged((Brush)e.NewValue);
        }

        /// <summary>
        /// Called after the <see cref="CueBarBackgroundBrush"/> value has changed.
        /// </summary>
        /// <param name="newValue">The new value of <see cref="CueBarBackgroundBrush"/></param>
        private void OnCueBarBackgroundBrushChanged(Brush newValue)
        {
            if (this._cueMarksCanvas != null)
                this._cueMarksCanvas.Background = newValue;
        }

        /// <summary>
        /// Gets or sets a brush used to draw the track progress indicator bar.
        /// </summary>
        [Category("Brushes")]
        public Brush CueBarBackgroundBrush
        {
            get => (Brush)this.GetValue(CueBarBackgroundBrushProperty);
            set => this.SetValue(CueBarBackgroundBrushProperty, value);
        }

        public static readonly DependencyProperty CueMarkAccentBrushProperty = DependencyProperty.Register(
            nameof(CueMarkAccentBrush),
            typeof(Brush),
            typeof(Curtains),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 0, 0))));

        [Category("Brushes")]
        public Brush CueMarkAccentBrush
        {
            get => (Brush)this.GetValue(CueMarkAccentBrushProperty);
            set => this.SetValue(CueMarkAccentBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ShowCueMarks"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowCueMarksProperty = DependencyProperty.Register(
            nameof(ShowCueMarks),
            typeof(bool),
            typeof(Curtains),
            new UIPropertyMetadata(true, OnShowCueMarksChanged));

        private static void OnShowCueMarksChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as Curtains)?.Render();
        }

        /// <summary>
        /// Whether to show the bar with cue marks
        /// Default: True
        /// </summary>
        [Category("Display")]
        public bool ShowCueMarks
        {
            get => (bool)this.GetValue(ShowCueMarksProperty);
            set => this.SetValue(ShowCueMarksProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="EnableCueMarksRepositioning"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableCueMarksRepositioningProperty = DependencyProperty.Register(
            nameof(EnableCueMarksRepositioning),
            typeof(bool),
            typeof(Curtains),
            new UIPropertyMetadata(true, OnEnableCueMarksRepositioningChanged));

        private static void OnEnableCueMarksRepositioningChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as Curtains)?.Render();
        }

        /// <summary>
        /// Whether to enable moving the cue marks around
        /// Default: False
        /// </summary>
        [Category("Playback")]
        public bool EnableCueMarksRepositioning
        {
            get => (bool)this.GetValue(EnableCueMarksRepositioningProperty);
            set => this.SetValue(EnableCueMarksRepositioningProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.MainCanvas = this.GetTemplateChild("PART_Curtains") as Canvas;
            this._cueMarksCanvas = this.GetTemplateChild("PART_CueMarks") as Canvas;
            this._leftSideCurtain = this.GetTemplateChild("PART_LeftCurtain") as Canvas;
            this._rightSideCurtain = this.GetTemplateChild("PART_RightCurtain") as Canvas;
            if (this._leftSideCurtain != null)
            {
                this._leftSideCurtain.Width = 0;
            }
            if (this._rightSideCurtain != null)
            {
                this._rightSideCurtain.Width = 0;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.Render();
        }

        protected override void OnTuneChanged()
        {
            this._cuePoints.Clear();
            this._cuePoints.AddRange(this.Tune.Cues().Select(d => new ZeroToOne(new FiniteDouble(d))));
            this.Render();
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonDown routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!this.EnableCueMarksRepositioning)
            {
                this.AfterMouseLeftButtonDown();
                return;
            }
            this.CurtainMoving();
            this.AfterMouseLeftButtonDown();
        }

        /// <summary>
        /// A utility method
        /// </summary>
        private void AfterMouseLeftButtonDown()
        {
            //CaptureMouse();
            this._isMouseDown = true;
        }

        /// <summary>
        /// Move a cue point, or a repeat region
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!this._isMouseDown)
                return;

            var currentPoint = e.GetPosition(this.MainCanvas);
            /* sanitization */
            if (currentPoint.X < this._waveformDimensions.LeftMargin())
            {
                currentPoint.X = this._waveformDimensions.LeftMargin();
            }
            if (currentPoint.X > this.MainCanvas.RenderSize.Width - this._waveformDimensions.RightMargin())
            {
                currentPoint.X = this.MainCanvas.RenderSize.Width - this._waveformDimensions.RightMargin();
            }

            var leftCorner = currentPoint.X - (this._cueMarksCanvas.RenderSize.Height / 2.5d);
            var rightCorner = currentPoint.X + (this._cueMarksCanvas.RenderSize.Height / 2.5d);
            if (this.EnableCueMarksRepositioning
                && leftCorner >= 0 && rightCorner <= this.MainCanvas.RenderSize.Width)
            {
                this.MoveCuePoint(currentPoint, leftCorner, rightCorner);
            }
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonUp routed event reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            //ReleaseMouseCapture();
            if (!this._isMouseDown || !this.EnableCueMarksRepositioning)
                return;

            this._isMouseDown = false;
            this.MeasureArea();
            this.CurtainMoved(this._waveformDimensions.PercentOfCompleteWaveform(e.GetPosition(this.MainCanvas).X));
        }

        protected override void Render()
        {
            this.Clear();
            this.MeasureArea();
            if (!this.ShowCueMarks || this._cuePoints.Count == 0 || this._leftSideCurtain == null || this._rightSideCurtain == null || this.MainCanvas == null || this._waveformDimensions.AreEmpty())
                return;

            double minCuePoint = Math.Max(this._cuePoints.Min(), 0);
            if (this._coverageArea.Includes(minCuePoint))
            {
                this._leftSideCurtain.Margin = new Thickness(this._waveformDimensions.LeftMargin(), 0, 0, 0);
                this._leftSideCurtain.Width = Math.Max(0, this._waveformDimensions.PositionOnCompleteWaveform(minCuePoint));
            }

            double maxCuePoint = this._cuePoints.Count == 1 ? 1 : Math.Min(this._cuePoints.Max(), 1);
            if (this._coverageArea.Includes(maxCuePoint))
            {
                double rightSideCurtainLeftX = this._waveformDimensions.LeftMargin() + this._waveformDimensions.AbsoluteLocationToRendered(this._waveformDimensions.PositionOnCompleteWaveform((ZeroToOne)maxCuePoint));
                double rightSideCurtainRightX = this._waveformDimensions.LeftMargin() + this._waveformDimensions.Width();
                this._rightSideCurtain.Width = Math.Max(0,
                    (rightSideCurtainRightX - rightSideCurtainLeftX));
                this._rightSideCurtain.Margin = new Thickness(0, 0, this._waveformDimensions.RightMargin(), 0);
            }

            this._cuePoints.Select(cue => (Cue: cue, Location:
                    new FiniteDouble(this._waveformDimensions.LeftMargin() + this._waveformDimensions.PositionOnCompleteWaveform(cue))))
                .Select(t => (Cue: t.Cue, Location:
                    new FiniteDouble(this._waveformDimensions.AbsoluteLocationToRendered(t.Location))))
                .Select(t => (Line: this.DrawLine(t.Cue, t.Location), Polygon: this.DrawPolygon(t.Cue, t.Location, this._cueMarksCanvas.RenderSize.Height / 2.5d)))
                .ForEach(this.AddCurtain);
        }

        private void AddCurtain((Line Line, Polygon Polygon) t)
        {
            this._cuePointLines.Add(t.Line);
            this.MainCanvas.Children.Add(t.Line);
            this._cuePointMarks.Add(t.Polygon);
            this._cueMarksCanvas.Children.Add(t.Polygon);
        }

        private Polygon DrawPolygon(double cp, double xLocation, double centerOffset)
        {
            Polygon cue = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(xLocation, 0), // top
                    new Point(xLocation - centerOffset, this._cueMarksCanvas.RenderSize.Height / 2), // left middle
                    new Point(xLocation - centerOffset, this._cueMarksCanvas.RenderSize.Height), // left bottom
                    new Point(xLocation + centerOffset, this._cueMarksCanvas.RenderSize.Height),  // right
                    new Point(xLocation + centerOffset, this._cueMarksCanvas.RenderSize.Height / 2)  // right
                }
            };
            Style cueStyle = (Style)Application.Current.FindResource("CueMarkPolygonStyle");
            Style invisibleCueStyle = (Style)Application.Current.FindResource("InvisibleCueMarkPolygonStyle");
            cue.Style = this._coverageArea.Includes(cp) ? cueStyle : invisibleCueStyle;
            return cue;
        }

        private Line DrawLine(double cp, double xLocation)
        {
            return new Line
            {
                Stroke = this._coverageArea.Includes(cp) ? this.CueMarkBrush : this._transparentBrush,
                StrokeThickness = 1.0d,
                X1 = xLocation,
                X2 = xLocation,
                Y1 = 0,
                Y2 = this.MainCanvas.RenderSize.Height
            };
        }

        public void Clear()
        {
            this._cuePointMarks.ForEach(mark => this._cueMarksCanvas.Children.Remove(mark));
            this._cuePointMarks.Clear();
            this._cuePointLines.ForEach(line => this.MainCanvas.Children.Remove(line));
            this._cuePointLines.Clear();
            if (this._leftSideCurtain != null)
                this._leftSideCurtain.Width = 0;
            if (this._rightSideCurtain != null)
                this._rightSideCurtain.Width = 0;
        }

        private void CurtainMoving()
        {
            var cueMarkSelected = this._cuePointMarks.FirstOrDefault(cue => cue.IsMouseOver);
            if (cueMarkSelected == null)
            {
                this._selectedCuePointMark = this._selectedCuePointLine = null;
                this._animatedCurtain = null;
                this._selectedCuePoint = 0.0d;
                this._lastKnownGoodX = 0.0d;
                this._cueMap.Clear();
                return;
            }

            this._selectedCuePointMark = cueMarkSelected;
            this._selectedCuePointMark.Fill = this.CueMarkAccentBrush;
            this._selectedCuePointLine = this._cuePointLines[this._cuePointMarks.IndexOf(this._selectedCuePointMark)];

            this._cueMap.Clear();
            for (int i = 0; i < this._cuePoints.Count && i < this._cuePointMarks.Count; i++)
            {
                if (this._cueMap.ContainsKey(this._cuePoints[i]))
                    continue;
                this._cueMap.Add(this._cuePoints[i], this._cuePointMarks[i]);

                if (ReferenceEquals(this._cuePointMarks[i], cueMarkSelected))
                    this._selectedCuePoint = this._cuePoints[i]; // that's the cue point whose shape is being moved
            }

            // figure out which cue point we are moving
            this._animatedCurtain = this._cuePoints.Count == 1
                ? this._leftSideCurtain
                : (this._selectedCuePoint < this._cuePoints.Max() ? this._leftSideCurtain : this._rightSideCurtain);
        }

        private void MoveCuePoint(Point currentPoint, double leftCorner, double rightCorner)
        {
            if (this._selectedCuePointMark == null)
                return;

            this._lastKnownGoodX = currentPoint.X;
            ((Polygon)this._selectedCuePointMark).Points = new PointCollection()
            {
                new Point(currentPoint.X, 0),
                new Point(leftCorner, this._cueMarksCanvas.RenderSize.Height / 2),
                new Point(leftCorner, this._cueMarksCanvas.RenderSize.Height),
                new Point(rightCorner, this._cueMarksCanvas.RenderSize.Height),
                new Point(rightCorner, this._cueMarksCanvas.RenderSize.Height / 2)
            };

            this._selectedCuePointLine.X1 = this._lastKnownGoodX;
            this._selectedCuePointLine.X2 = this._lastKnownGoodX;
            this._selectedCuePointLine.Y1 = 0;
            this._selectedCuePointLine.Y2 = this.MainCanvas.RenderSize.Height;
            if (ReferenceEquals(this._animatedCurtain, this._leftSideCurtain))
            {
                this._leftSideCurtain.Margin = new Thickness(this._waveformDimensions.LeftMargin(), 0, 0, 0);
                this._leftSideCurtain.Width = Math.Max(0, this._lastKnownGoodX - this._waveformDimensions.LeftMargin());
            }
            else
            {
                this._rightSideCurtain.Width = Math.Max(0,
                    this.MainCanvas.RenderSize.Width - this._lastKnownGoodX - this._waveformDimensions.RightMargin());
            }
        }

        /// <summary>
        /// Moved the curtain, add a new cue
        /// </summary>
        /// <param name="newCue"></param>
        private void CurtainMoved(ZeroToOne newCue)
        {
            if (this._selectedCuePointMark == null || !(this._lastKnownGoodX > 0.0d))
                return;

            this._cuePoints.Remove(this._selectedCuePoint);
            this.AddCuePoint(newCue);
            this.Render();
            this._animatedCurtain = null;
            this._selectedCuePointMark = this._selectedCuePointLine = null;
            this._selectedCuePoint = 0d;
            this._lastKnownGoodX = 0.0d;
        }

        /// <summary>
        /// Add a cue point at the specified position on the timeline
        /// </summary>
        /// <param name="pos"></param>
        private void AddCuePoint(ZeroToOne pos)
        {
            if (!this._cuePoints.Contains(pos))
                this._cuePoints.Add(pos);

            this.SyncTrackStartEndTimes(this._cuePoints.ToArray());
        }

        private void SyncTrackStartEndTimes(ZeroToOne[] inputs)
        {
            var values = inputs.OrderBy(x => x).ToArray();
            if (values.Length != 2 || values[1] < values[0])
                return;

            this.Tune.TrimStart(TimeSpan.FromTicks((long)(Math.Min(Math.Max(0d, values[0]), values[1]) * this.Tune.Duration().Ticks)));
            this.Tune.TrimEnd(TimeSpan.FromTicks((long)(Math.Min(Math.Max(values[0], values[1]), values[1]) * this.Tune.Duration().Ticks)));
        }
    }
}