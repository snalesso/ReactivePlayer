// Copyright (C) 2011 - 2012, Jacob Johnston 
//
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions: 
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software. 
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE. 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFSoundVisualizationLib
{
    /// <summary>
    /// A control that displays a stereo waveform and
    /// allows a user to change playback position.
    /// </summary>
    [DisplayName("Waveform Timeline")]
    [Description("Displays a stereo waveform and allows a user to change playback position.")]
    [ToolboxItem(true)]
    [TemplatePart(Name = "PART_Waveform", Type = typeof(Canvas)),
     TemplatePart(Name = "PART_Timeline", Type = typeof(Canvas)),
     TemplatePart(Name = "PART_Repeat", Type = typeof(Canvas)),
     TemplatePart(Name = "PART_Progress", Type = typeof(Canvas))]
    public class WaveformTimeline : Control
    {
        #region Fields
        private IWaveformPlayer soundPlayer;
        private Canvas waveformCanvas;
        private Canvas repeatCanvas;
        private Canvas timelineCanvas;
        private Canvas progressCanvas;
        private readonly Path leftPath = new Path();
        private readonly Path rightPath = new Path();
        private readonly Line centerLine = new Line();
        private readonly Rectangle repeatRegion = new Rectangle();
        private readonly Line progressLine = new Line();
        private readonly Path progressIndicator = new Path();
        private readonly List<Line> timeLineTicks = new List<Line>();
        private readonly Rectangle timelineBackgroundRegion = new Rectangle();
        private readonly List<TextBlock> timestampTextBlocks = new List<TextBlock>();
        private bool isMouseDown;
        private Point mouseDownPoint;
        private Point currentPoint;
        private double startLoopRegion = -1;
        private double endLoopRegion = -1;        
        #endregion

        #region Constants
        private const int mouseMoveTolerance = 3;
        private const int indicatorTriangleWidth = 4;
        private const int majorTickHeight = 10;
        private const int minorTickHeight = 3;
        private const int timeStampMargin = 5;
        #endregion

        #region Dependency Properties
        #region LeftLevelBrush
        /// <summary>
        /// Identifies the <see cref="LeftLevelBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LeftLevelBrushProperty = DependencyProperty.Register("LeftLevelBrush", typeof(Brush), typeof(WaveformTimeline), new UIPropertyMetadata(new SolidColorBrush(Colors.Blue), OnLeftLevelBrushChanged, OnCoerceLeftLevelBrush));

        private static object OnCoerceLeftLevelBrush(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceLeftLevelBrush((Brush)value);
            else
                return value;
        }

        private static void OnLeftLevelBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnLeftLevelBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="LeftLevelBrush"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="LeftLevelBrush"/></param>
        /// <returns>The adjusted value of <see cref="LeftLevelBrush"/></returns>
        protected virtual Brush OnCoerceLeftLevelBrush(Brush value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="LeftLevelBrush"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="LeftLevelBrush"/></param>
        /// <param name="newValue">The new value of <see cref="LeftLevelBrush"/></param>
        protected virtual void OnLeftLevelBrushChanged(Brush oldValue, Brush newValue)
        {
            this.leftPath.Fill = this.LeftLevelBrush;
            this.UpdateWaveform();
        }

        /// <summary>
        /// Gets or sets a brush used to draw the left channel output on the waveform.
        /// </summary>        
        [Category("Brushes")]
        public Brush LeftLevelBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)this.GetValue(LeftLevelBrushProperty);
            }
            set
            {
                this.SetValue(LeftLevelBrushProperty, value);
            }
        }
        #endregion

        #region RightLevelBrush
        /// <summary>
        /// Identifies the <see cref="RightLevelBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty RightLevelBrushProperty = DependencyProperty.Register("RightLevelBrush", typeof(Brush), typeof(WaveformTimeline), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), OnRightLevelBrushChanged, OnCoerceRightLevelBrush));

        private static object OnCoerceRightLevelBrush(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceRightLevelBrush((Brush)value);
            else
                return value;
        }

        private static void OnRightLevelBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnRightLevelBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="RightLevelBrush"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="RightLevelBrush"/></param>
        /// <returns>The adjusted value of <see cref="RightLevelBrush"/></returns>
        protected virtual Brush OnCoerceRightLevelBrush(Brush value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="RightLevelBrush"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="RightLevelBrush"/></param>
        /// <param name="newValue">The new value of <see cref="RightLevelBrush"/></param>
        protected virtual void OnRightLevelBrushChanged(Brush oldValue, Brush newValue)
        {
            this.rightPath.Fill = this.RightLevelBrush;
            this.UpdateWaveform();
        }

        /// <summary>
        /// Gets or sets a brush used to draw the right speaker levels on the waveform.
        /// </summary>
        [Category("Brushes")]
        public Brush RightLevelBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)this.GetValue(RightLevelBrushProperty);
            }
            set
            {
                this.SetValue(RightLevelBrushProperty, value);
            }
        }
        #endregion

        #region ProgressBarBrush
        /// <summary>
        /// Identifies the <see cref="ProgressBarBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ProgressBarBrushProperty = DependencyProperty.Register("ProgressBarBrush", typeof(Brush), typeof(WaveformTimeline), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(0xCD, 0xBA, 0x00, 0xFF)), OnProgressBarBrushChanged, OnCoerceProgressBarBrush));

        private static object OnCoerceProgressBarBrush(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceProgressBarBrush((Brush)value);
            else
                return value;
        }

        private static void OnProgressBarBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnProgressBarBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="ProgressBarBrush"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="ProgressBarBrush"/></param>
        /// <returns>The adjusted value of <see cref="ProgressBarBrush"/></returns>
        protected virtual Brush OnCoerceProgressBarBrush(Brush value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="ProgressBarBrush"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="ProgressBarBrush"/></param>
        /// <param name="newValue">The new value of <see cref="ProgressBarBrush"/></param>
        protected virtual void OnProgressBarBrushChanged(Brush oldValue, Brush newValue)
        {
            this.progressIndicator.Fill = this.ProgressBarBrush;
            this.progressLine.Stroke = this.ProgressBarBrush;

            this.CreateProgressIndicator();
        }

        /// <summary>
        /// Gets or sets a brush used to draw the track progress indicator bar.
        /// </summary>
        [Category("Brushes")]
        public Brush ProgressBarBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)this.GetValue(ProgressBarBrushProperty);
            }
            set
            {
                this.SetValue(ProgressBarBrushProperty, value);
            }
        }
        #endregion

        #region ProgressBarThickness
        /// <summary>
        /// Identifies the <see cref="ProgressBarThickness" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ProgressBarThicknessProperty = DependencyProperty.Register("ProgressBarThickness", typeof(double), typeof(WaveformTimeline), new UIPropertyMetadata(2.0d, OnProgressBarThicknessChanged, OnCoerceProgressBarThickness));

        private static object OnCoerceProgressBarThickness(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceProgressBarThickness((double)value);
            else
                return value;
        }

        private static void OnProgressBarThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnProgressBarThicknessChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="ProgressBarThickness"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="ProgressBarThickness"/></param>
        /// <returns>The adjusted value of <see cref="ProgressBarThickness"/></returns>
        protected virtual double OnCoerceProgressBarThickness(double value)
        {
            value = Math.Max(value, 0.0d);
            return value;
        }

        /// <summary>
        /// Called after the <see cref="ProgressBarThickness"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="ProgressBarThickness"/></param>
        /// <param name="newValue">The new value of <see cref="ProgressBarThickness"/></param>
        protected virtual void OnProgressBarThicknessChanged(double oldValue, double newValue)
        {
            this.progressLine.StrokeThickness = this.ProgressBarThickness;
            this.CreateProgressIndicator();
        }

        /// <summary>
        /// Get or sets the thickness of the progress indicator bar.
        /// </summary>
        [Category("Common")]
        public double ProgressBarThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)this.GetValue(ProgressBarThicknessProperty);
            }
            set
            {
                this.SetValue(ProgressBarThicknessProperty, value);
            }
        }
        #endregion

        #region CenterLineBrush
        /// <summary>
        /// Identifies the <see cref="CenterLineBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CenterLineBrushProperty = DependencyProperty.Register("CenterLineBrush", typeof(Brush), typeof(WaveformTimeline), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), OnCenterLineBrushChanged, OnCoerceCenterLineBrush));

        private static object OnCoerceCenterLineBrush(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceCenterLineBrush((Brush)value);
            else
                return value;
        }

        private static void OnCenterLineBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnCenterLineBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="CenterLineBrush"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="CenterLineBrush"/></param>
        /// <returns>The adjusted value of <see cref="CenterLineBrush"/></returns>
        protected virtual Brush OnCoerceCenterLineBrush(Brush value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="CenterLineBrush"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="CenterLineBrush"/></param>
        /// <param name="newValue">The new value of <see cref="CenterLineBrush"/></param>
        protected virtual void OnCenterLineBrushChanged(Brush oldValue, Brush newValue)
        {
            this.centerLine.Stroke = this.CenterLineBrush;
            this.UpdateWaveform();
        }

        /// <summary>
        /// Gets or sets a brush used to draw the center line separating left and right levels.
        /// </summary>
        [Category("Brushes")]
        public Brush CenterLineBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)this.GetValue(CenterLineBrushProperty);
            }
            set
            {
                this.SetValue(CenterLineBrushProperty, value);
            }
        }
        #endregion

        #region CenterLineThickness
        /// <summary>
        /// Identifies the <see cref="CenterLineThickness" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CenterLineThicknessProperty = DependencyProperty.Register("CenterLineThickness", typeof(double), typeof(WaveformTimeline), new UIPropertyMetadata(1.0d, OnCenterLineThicknessChanged, OnCoerceCenterLineThickness));

        private static object OnCoerceCenterLineThickness(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceCenterLineThickness((double)value);
            else
                return value;
        }

        private static void OnCenterLineThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnCenterLineThicknessChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="CenterLineThickness"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="CenterLineThickness"/></param>
        /// <returns>The adjusted value of <see cref="CenterLineThickness"/></returns>
        protected virtual double OnCoerceCenterLineThickness(double value)
        {
            value = Math.Max(value, 0.0d);
            return value;
        }

        /// <summary>
        /// Called after the <see cref="CenterLineThickness"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="CenterLineThickness"/></param>
        /// <param name="newValue">The new value of <see cref="CenterLineThickness"/></param>
        protected virtual void OnCenterLineThicknessChanged(double oldValue, double newValue)
        {
            this.centerLine.StrokeThickness = this.CenterLineThickness;
            this.UpdateWaveform();
        }

        /// <summary>
        /// Gets or sets the thickness of the center line separating left and right levels.
        /// </summary>
        [Category("Common")]
        public double CenterLineThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)this.GetValue(CenterLineThicknessProperty);
            }
            set
            {
                this.SetValue(CenterLineThicknessProperty, value);
            }
        }
        #endregion

        #region RepeatRegionBrush
        /// <summary>
        /// Identifies the <see cref="RepeatRegionBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty RepeatRegionBrushProperty = DependencyProperty.Register("RepeatRegionBrush", typeof(Brush), typeof(WaveformTimeline), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(0x81, 0xF6, 0xFF, 0x00)), OnRepeatRegionBrushChanged, OnCoerceRepeatRegionBrush));

        private static object OnCoerceRepeatRegionBrush(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceRepeatRegionBrush((Brush)value);
            else
                return value;
        }

        private static void OnRepeatRegionBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnRepeatRegionBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="RepeatRegionBrush"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="RepeatRegionBrush"/></param>
        /// <returns>The adjusted value of <see cref="RepeatRegionBrush"/></returns>
        protected virtual Brush OnCoerceRepeatRegionBrush(Brush value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="RepeatRegionBrush"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="RepeatRegionBrush"/></param>
        /// <param name="newValue">The new value of <see cref="RepeatRegionBrush"/></param>
        protected virtual void OnRepeatRegionBrushChanged(Brush oldValue, Brush newValue)
        {
            this.repeatRegion.Fill = this.RepeatRegionBrush;
            this.UpdateRepeatRegion();
        }

        /// <summary>
        /// Gets or sets a brush used to draw the repeat region on the waveform.
        /// </summary>
        [Category("Brushes")]
        public Brush RepeatRegionBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)this.GetValue(RepeatRegionBrushProperty);
            }
            set
            {
                this.SetValue(RepeatRegionBrushProperty, value);
            }
        }

        #endregion

        #region AllowRepeatRegions
        /// <summary>
        /// Identifies the <see cref="AllowRepeatRegions" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty AllowRepeatRegionsProperty = DependencyProperty.Register("AllowRepeatRegions", typeof(bool), typeof(WaveformTimeline), new UIPropertyMetadata(true, OnAllowRepeatRegionsChanged, OnCoerceAllowRepeatRegions));

        private static object OnCoerceAllowRepeatRegions(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceAllowRepeatRegions((bool)value);
            else
                return value;
        }

        private static void OnAllowRepeatRegionsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnAllowRepeatRegionsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="AllowRepeatRegions"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="AllowRepeatRegions"/></param>
        /// <returns>The adjusted value of <see cref="AllowRepeatRegions"/></returns>
        protected virtual bool OnCoerceAllowRepeatRegions(bool value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="AllowRepeatRegions"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="AllowRepeatRegions"/></param>
        /// <param name="newValue">The new value of <see cref="AllowRepeatRegions"/></param>
        protected virtual void OnAllowRepeatRegionsChanged(bool oldValue, bool newValue)
        {
            if (!newValue && this.soundPlayer != null)
            {
                this.soundPlayer.SelectionBegin = TimeSpan.Zero;
                this.soundPlayer.SelectionEnd = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether repeat regions will be created via mouse drag across the waveform.
        /// </summary>
        [Category("Common")]
        public bool AllowRepeatRegions
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)this.GetValue(AllowRepeatRegionsProperty);
            }
            set
            {
                this.SetValue(AllowRepeatRegionsProperty, value);
            }
        }
        #endregion
        
        #region TimelineTickBrush
        /// <summary>
        /// Identifies the <see cref="TimelineTickBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty TimelineTickBrushProperty = DependencyProperty.Register("TimelineTickBrush", typeof(Brush), typeof(WaveformTimeline), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), OnTimelineTickBrushChanged, OnCoerceTimelineTickBrush));

        private static object OnCoerceTimelineTickBrush(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceTimelineTickBrush((Brush)value);
            else
                return value;
        }

        private static void OnTimelineTickBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnTimelineTickBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="TimelineTickBrush"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="TimelineTickBrush"/></param>
        /// <returns>The adjusted value of <see cref="TimelineTickBrush"/></returns>
        protected virtual Brush OnCoerceTimelineTickBrush(Brush value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="TimelineTickBrush"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="TimelineTickBrush"/></param>
        /// <param name="newValue">The new value of <see cref="TimelineTickBrush"/></param>
        protected virtual void OnTimelineTickBrushChanged(Brush oldValue, Brush newValue)
        {
            this.UpdateTimeline();
        }

        /// <summary>
        /// Gets or sets a brush used to draw the tickmarks on the timeline.
        /// </summary>
        [Category("Brushes")]
        public Brush TimelineTickBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)this.GetValue(TimelineTickBrushProperty);
            }
            set
            {
                this.SetValue(TimelineTickBrushProperty, value);
            }
        }
        #endregion

        #region AutoScaleWaveformCache
        /// <summary>
        /// Identifies the <see cref="AutoScaleWaveformCache" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty AutoScaleWaveformCacheProperty = DependencyProperty.Register("AutoScaleWaveformCache", typeof(bool), typeof(WaveformTimeline), new UIPropertyMetadata(false, OnAutoScaleWaveformCacheChanged, OnCoerceAutoScaleWaveformCache));

        private static object OnCoerceAutoScaleWaveformCache(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceAutoScaleWaveformCache((bool)value);
            else
                return value;
        }

        private static void OnAutoScaleWaveformCacheChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnAutoScaleWaveformCacheChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="AutoScaleWaveformCache"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="AutoScaleWaveformCache"/></param>
        /// <returns>The adjusted value of <see cref="AutoScaleWaveformCache"/></returns>
        protected virtual bool OnCoerceAutoScaleWaveformCache(bool value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="AutoScaleWaveformCache"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="AutoScaleWaveformCache"/></param>
        /// <param name="newValue">The new value of <see cref="AutoScaleWaveformCache"/></param>
        protected virtual void OnAutoScaleWaveformCacheChanged(bool oldValue, bool newValue)
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
        [Category("Common")]
        public bool AutoScaleWaveformCache
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)this.GetValue(AutoScaleWaveformCacheProperty);
            }
            set
            {
                this.SetValue(AutoScaleWaveformCacheProperty, value);
            }
        }
        #endregion
        #endregion

        #region Template Overrides
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code
        /// or internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.waveformCanvas = this.GetTemplateChild("PART_Waveform") as Canvas;
            this.waveformCanvas.CacheMode = new BitmapCache();

            // Used to make the transparent regions clickable.
            this.waveformCanvas.Background = new SolidColorBrush(Colors.Transparent);

            this.waveformCanvas.Children.Add(this.centerLine);
            this.waveformCanvas.Children.Add(this.leftPath);
            this.waveformCanvas.Children.Add(this.rightPath);

            this.timelineCanvas = this.GetTemplateChild("PART_Timeline") as Canvas;
            this.timelineCanvas.Children.Add(this.timelineBackgroundRegion);
            this.timelineCanvas.SizeChanged += this.timelineCanvas_SizeChanged;

            this.repeatCanvas = this.GetTemplateChild("PART_Repeat") as Canvas;
            this.repeatCanvas.Children.Add(this.repeatRegion);

            this.progressCanvas = this.GetTemplateChild("PART_Progress") as Canvas;
            this.progressCanvas.Children.Add(this.progressIndicator);
            this.progressCanvas.Children.Add(this.progressLine);

            this.UpdateWaveformCacheScaling();            
        }            

        /// <summary>
        /// Called whenever the control's template changes. 
        /// </summary>
        /// <param name="oldTemplate">The old template</param>
        /// <param name="newTemplate">The new template</param>
        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            base.OnTemplateChanged(oldTemplate, newTemplate);
            if(this.waveformCanvas != null)
                this.waveformCanvas.Children.Clear();
            if (this.timelineCanvas != null)
            {
                this.timelineCanvas.SizeChanged -= this.timelineCanvas_SizeChanged;
                this.timelineCanvas.Children.Clear();
            }
            if(this.repeatCanvas != null)
                this.repeatCanvas.Children.Clear();
            if(this.progressCanvas != null)
                this.progressCanvas.Children.Clear();
        }
        #endregion

        #region Constructor
        static WaveformTimeline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaveformTimeline), new FrameworkPropertyMetadata(typeof(WaveformTimeline)));
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Register a sound player from which the waveform timeline
        /// can get the necessary playback data.
        /// </summary>
        /// <param name="soundPlayer">A sound player that provides waveform data through the IWaveformPlayer interface methods.</param>
        public void RegisterSoundPlayer(IWaveformPlayer soundPlayer)
        {
            this.soundPlayer = soundPlayer;
            soundPlayer.PropertyChanged += this.soundPlayer_PropertyChanged;
        }
        #endregion

        #region Event Handlers
        private void soundPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectionBegin":
                    this.startLoopRegion = this.soundPlayer.SelectionBegin.TotalSeconds;
                    this.UpdateRepeatRegion();
                    break;
                case "SelectionEnd":
                    this.endLoopRegion = this.soundPlayer.SelectionEnd.TotalSeconds;
                    this.UpdateRepeatRegion();
                    break;
                case "WaveformData":
                    this.UpdateWaveform();
                    break;
                case "ChannelPosition":
                    this.UpdateProgressIndicator();
                    break;
                case "ChannelLength":
                    this.startLoopRegion = -1;
                    this.endLoopRegion = -1;
                    this.UpdateAllRegions();
                    break;
            }
        }

        private void timelineCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateTimeline();
        }       
        #endregion

        #region Event Overrides
        /// <summary>
        /// Raises the SizeChanged event, using the specified information as part of the eventual event data. 
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.UpdateWaveformCacheScaling();
            this.UpdateAllRegions();
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonDown routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.CaptureMouse();
            this.isMouseDown = true;
            this.mouseDownPoint = e.GetPosition(this.waveformCanvas);
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonUp routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);            
            if (!this.isMouseDown)
                return;

            bool updateRepeatRegion = false;
            this.isMouseDown = false;
            this.ReleaseMouseCapture();
            if (Math.Abs(this.currentPoint.X - this.mouseDownPoint.X) < mouseMoveTolerance)
            {
                if (this.PointInRepeatRegion(this.mouseDownPoint))
                {
                    double position = (this.currentPoint.X / this.RenderSize.Width) * this.soundPlayer.ChannelLength;
                    this.soundPlayer.ChannelPosition = Math.Min(this.soundPlayer.ChannelLength, Math.Max(0, position));
                }
                else
                {
                    this.soundPlayer.SelectionBegin = TimeSpan.Zero;
                    this.soundPlayer.SelectionEnd = TimeSpan.Zero;
                    double position = (this.currentPoint.X / this.RenderSize.Width) * this.soundPlayer.ChannelLength;
                    this.soundPlayer.ChannelPosition = Math.Min(this.soundPlayer.ChannelLength, Math.Max(0, position));
                    this.startLoopRegion = -1;
                    this.endLoopRegion = -1;
                    updateRepeatRegion = true;
                }
            }
            else
            {
                this.soundPlayer.SelectionBegin = TimeSpan.FromSeconds(this.startLoopRegion);
                this.soundPlayer.SelectionEnd = TimeSpan.FromSeconds(this.endLoopRegion);
                double position = this.startLoopRegion;
                this.soundPlayer.ChannelPosition = Math.Min(this.soundPlayer.ChannelLength, Math.Max(0, position));
                updateRepeatRegion = true;
            }

            if (updateRepeatRegion)
                this.UpdateRepeatRegion();
        }

        /// <summary>
        /// Invoked when an unhandled Mouse.MouseMove attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.currentPoint = e.GetPosition(this.waveformCanvas);

            if (this.isMouseDown && this.AllowRepeatRegions)
            {
                if (Math.Abs(this.currentPoint.X - this.mouseDownPoint.X) > mouseMoveTolerance)
                {
                    if (this.mouseDownPoint.X < this.currentPoint.X)
                    {
                        this.startLoopRegion = (this.mouseDownPoint.X / this.RenderSize.Width) * this.soundPlayer.ChannelLength;
                        this.endLoopRegion = (this.currentPoint.X / this.RenderSize.Width) * this.soundPlayer.ChannelLength;
                    }
                    else
                    {
                        this.startLoopRegion = (this.currentPoint.X / this.RenderSize.Width) * this.soundPlayer.ChannelLength;
                        this.endLoopRegion = (this.mouseDownPoint.X / this.RenderSize.Width) * this.soundPlayer.ChannelLength;
                    }
                }
                else
                {
                    this.startLoopRegion = -1;
                    this.endLoopRegion = -1;
                }
                this.UpdateRepeatRegion();
            }
        }        
        #endregion

        #region Private Utiltiy Methods
        private void UpdateWaveformCacheScaling()
        {
            if (this.waveformCanvas == null)
                return;

            BitmapCache waveformCache = (BitmapCache)this.waveformCanvas.CacheMode;
            if (this.AutoScaleWaveformCache)
            {
                double totalTransformScale = this.GetTotalTransformScale();
                if (waveformCache.RenderAtScale != totalTransformScale)
                    waveformCache.RenderAtScale = totalTransformScale;
            }
            else
            {
                waveformCache.RenderAtScale = 1.0d;
            }
        }

        private bool PointInRepeatRegion(Point point)
        {
            if (this.soundPlayer.ChannelLength == 0)
                return false;

            double regionLeft = (this.soundPlayer.SelectionBegin.TotalSeconds / this.soundPlayer.ChannelLength) * this.RenderSize.Width;
            double regionRight = (this.soundPlayer.SelectionEnd.TotalSeconds / this.soundPlayer.ChannelLength) * this.RenderSize.Width;

            return (point.X >= regionLeft && point.X < regionRight);
        }

        private double GetTotalTransformScale()
        {
            double totalTransform = 1.0d;
            DependencyObject currentVisualTreeElement = this;
            do
            {
                Visual visual = currentVisualTreeElement as Visual;
                if (visual != null)
                {
                    Transform transform = VisualTreeHelper.GetTransform(visual);

                    // This condition is a way of determining if it
                    // was a uniform scale transform. Is there some better way?
                    if ((transform != null) &&
                        (transform.Value.M12 == 0) &&
                        (transform.Value.M21 == 0) &&
                        (transform.Value.OffsetX == 0) &&
                        (transform.Value.OffsetY == 0) &&
                        (transform.Value.M11 == transform.Value.M22))
                    {
                        totalTransform *= transform.Value.M11;
                    }
                }
                currentVisualTreeElement = VisualTreeHelper.GetParent(currentVisualTreeElement);
            }
            while (currentVisualTreeElement != null);

            return totalTransform;
        }

        private void UpdateAllRegions()
        {
            this.UpdateRepeatRegion();
            this.CreateProgressIndicator();
            this.UpdateTimeline();
            this.UpdateWaveform();
        }

        private void UpdateRepeatRegion()
        {
            if (this.soundPlayer == null || this.repeatCanvas == null)
                return;

            double startPercent = this.startLoopRegion / this.soundPlayer.ChannelLength;
            double startXLocation = startPercent * this.repeatCanvas.RenderSize.Width;
            double endPercent = this.endLoopRegion / this.soundPlayer.ChannelLength;
            double endXLocation = endPercent * this.repeatCanvas.RenderSize.Width;

            if (this.soundPlayer.ChannelLength == 0 || 
                endXLocation <= startXLocation)
            {
                this.repeatRegion.Width = 0;
                this.repeatRegion.Height = 0;
                return;
            }

            this.repeatRegion.Margin = new Thickness(startXLocation, 0, 0, 0);
            this.repeatRegion.Width = endXLocation - startXLocation;
            this.repeatRegion.Height = this.repeatCanvas.RenderSize.Height;
        }

        private void UpdateTimeline()
        {
            if (this.soundPlayer == null || this.timelineCanvas == null)
                return;

            foreach (TextBlock textblock in this.timestampTextBlocks)
            {
                this.timelineCanvas.Children.Remove(textblock);
            }
            this.timestampTextBlocks.Clear();

            foreach (Line line in this.timeLineTicks)
            {
                this.timelineCanvas.Children.Remove(line);
            }
            this.timeLineTicks.Clear();

            double bottomLoc = this.timelineCanvas.RenderSize.Height - 1;

            this.timelineBackgroundRegion.Width = this.timelineCanvas.RenderSize.Width;
            this.timelineBackgroundRegion.Height = this.timelineCanvas.RenderSize.Height;

            double minorTickDuration = 1.00d; // Major tick = 5 seconds, Minor tick = 1.00 second
            double majorTickDuration = 5.00d;
            if (this.soundPlayer.ChannelLength >= 120.0d) // Major tick = 1 minute, Minor tick = 15 seconds.
            {
                minorTickDuration = 15.0d;
                majorTickDuration = 60.0d;
            }
            else if (this.soundPlayer.ChannelLength >= 60.0d) // Major tick = 30 seconds, Minor tick = 5.0 seconds.
            {
                minorTickDuration = 5.0d;
                majorTickDuration = 30.0d;
            }
            else if (this.soundPlayer.ChannelLength >= 30.0d) // Major tick = 10 seconds, Minor tick = 2.0 seconds.
            {
                minorTickDuration = 2.0d;
                majorTickDuration = 10.0d;
            }

            if (this.soundPlayer.ChannelLength < minorTickDuration)
                return;

            int minorTickCount = (int)(this.soundPlayer.ChannelLength / minorTickDuration);
            for (int i = 1; i <= minorTickCount; i++)
            {
                Line timelineTick = new Line()
                {
                    Stroke = TimelineTickBrush,
                    StrokeThickness = 1.0d
                };
                if (i % (majorTickDuration / minorTickDuration) == 0) // Draw Large Ticks and Timestamps at minute marks
                {
                    double xLocation = ((i * minorTickDuration) / this.soundPlayer.ChannelLength) * this.timelineCanvas.RenderSize.Width;

                    bool drawTextBlock = false;
                    double lastTimestampEnd;
                    if (this.timestampTextBlocks.Count != 0)
                    {
                        TextBlock lastTextBlock = this.timestampTextBlocks[this.timestampTextBlocks.Count - 1];
                        lastTimestampEnd = lastTextBlock.Margin.Left + lastTextBlock.ActualWidth;
                    }
                    else
                        lastTimestampEnd = 0;

                    if (xLocation > lastTimestampEnd + timeStampMargin)
                        drawTextBlock = true;

                    // Flag that we're at the end of the timeline such 
                    // that there is not enough room for the text to draw.
                    bool isAtEndOfTimeline = (this.timelineCanvas.RenderSize.Width - xLocation < 28.0d);

                    if (drawTextBlock)
                    {
                        timelineTick.X1 = xLocation;
                        timelineTick.Y1 = bottomLoc;
                        timelineTick.X2 = xLocation;
                        timelineTick.Y2 = bottomLoc - majorTickHeight;

                        if (isAtEndOfTimeline)
                            continue;

                        TimeSpan timeSpan = TimeSpan.FromSeconds(i * minorTickDuration);
                        TextBlock timestampText = new TextBlock()
                        {
                            Margin = new Thickness(xLocation + 2, 0, 0, 0),
                            FontFamily = this.FontFamily,
                            FontStyle = this.FontStyle,
                            FontWeight = this.FontWeight,
                            FontStretch = this.FontStretch,
                            FontSize = this.FontSize,
                            Foreground = this.Foreground,
                            Text = (timeSpan.TotalHours >= 1.0d) ? string.Format(@"{0:hh\:mm\:ss}", timeSpan) : string.Format(@"{0:mm\:ss}", timeSpan)
                        };
                        this.timestampTextBlocks.Add(timestampText);
                        this.timelineCanvas.Children.Add(timestampText);
                        this.UpdateLayout(); // Needed so that we know the width of the textblock.
                    }
                    else // If still on the text block, draw a minor tick mark instead of a major.
                    {
                        timelineTick.X1 = xLocation;
                        timelineTick.Y1 = bottomLoc;
                        timelineTick.X2 = xLocation;
                        timelineTick.Y2 = bottomLoc - minorTickHeight;
                    }
                }
                else // Draw small ticks
                {
                    double xLocation = ((i * minorTickDuration) / this.soundPlayer.ChannelLength) * this.timelineCanvas.RenderSize.Width;
                    timelineTick.X1 = xLocation;
                    timelineTick.Y1 = bottomLoc;
                    timelineTick.X2 = xLocation;
                    timelineTick.Y2 = bottomLoc - minorTickHeight;
                }
                this.timeLineTicks.Add(timelineTick);
                this.timelineCanvas.Children.Add(timelineTick);
            }
        }

        private void CreateProgressIndicator()
        {
            if (this.soundPlayer == null || this.timelineCanvas == null || this.progressCanvas == null)
                return;

            const double xLocation = 0.0d;

            this.progressLine.X1 = xLocation;
            this.progressLine.X2 = xLocation;
            this.progressLine.Y1 = this.timelineCanvas.RenderSize.Height;
            this.progressLine.Y2 = this.progressCanvas.RenderSize.Height;

            PolyLineSegment indicatorPolySegment = new PolyLineSegment();
            indicatorPolySegment.Points.Add(new Point(xLocation, this.timelineCanvas.RenderSize.Height));
            indicatorPolySegment.Points.Add(new Point(xLocation - indicatorTriangleWidth, this.timelineCanvas.RenderSize.Height - indicatorTriangleWidth));
            indicatorPolySegment.Points.Add(new Point(xLocation + indicatorTriangleWidth, this.timelineCanvas.RenderSize.Height - indicatorTriangleWidth));
            indicatorPolySegment.Points.Add(new Point(xLocation, this.timelineCanvas.RenderSize.Height));
            PathGeometry indicatorGeometry = new PathGeometry();
            PathFigure indicatorFigure = new PathFigure();
            indicatorFigure.Segments.Add(indicatorPolySegment);
            indicatorGeometry.Figures.Add(indicatorFigure);

            this.progressIndicator.Data = indicatorGeometry;
            this.UpdateProgressIndicator();
        }

        private void UpdateProgressIndicator()
        {
            if (this.soundPlayer == null || this.progressCanvas == null)
                return;

            double xLocation = 0.0d;
            if (this.soundPlayer.ChannelLength != 0)
            {
                double progressPercent = this.soundPlayer.ChannelPosition / this.soundPlayer.ChannelLength;
                xLocation = progressPercent * this.progressCanvas.RenderSize.Width;
            }
            this.progressLine.Margin = new Thickness(xLocation, 0, 0, 0);
            this.progressIndicator.Margin = new Thickness(xLocation, 0, 0, 0);
        }

        private void UpdateWaveform()
        {
            const double minValue = 0;
            const double maxValue = 1.5;
            const double dbScale = (maxValue - minValue);

            if (this.soundPlayer == null || this.soundPlayer.WaveformData == null || this.waveformCanvas == null ||
                this.waveformCanvas.RenderSize.Width < 1 || this.waveformCanvas.RenderSize.Height < 1)
                return;

            double leftRenderHeight;
            double rightRenderHeight;

            int pointCount = (int)(this.soundPlayer.WaveformData.Length / 2.0d);
            double pointThickness = this.waveformCanvas.RenderSize.Width / pointCount;
            double waveformSideHeight = this.waveformCanvas.RenderSize.Height / 2.0d;
            double centerHeight = waveformSideHeight;

            if (this.CenterLineBrush != null)
            {
                this.centerLine.X1 = 0;
                this.centerLine.X2 = this.waveformCanvas.RenderSize.Width;
                this.centerLine.Y1 = centerHeight;
                this.centerLine.Y2 = centerHeight;
            }

            if (this.soundPlayer.WaveformData != null && this.soundPlayer.WaveformData.Length > 1)
            {
                PolyLineSegment leftWaveformPolyLine = new PolyLineSegment();
                leftWaveformPolyLine.Points.Add(new Point(0, centerHeight));

                PolyLineSegment rightWaveformPolyLine = new PolyLineSegment();
                rightWaveformPolyLine.Points.Add(new Point(0, centerHeight));

                double xLocation = 0.0d;
                for (int i = 0; i < this.soundPlayer.WaveformData.Length; i += 2)
                {
                    xLocation = (i / 2) * pointThickness;
                    leftRenderHeight = ((this.soundPlayer.WaveformData[i] - minValue) / dbScale) * waveformSideHeight;
                    leftWaveformPolyLine.Points.Add(new Point(xLocation, centerHeight - leftRenderHeight));
                    rightRenderHeight = ((this.soundPlayer.WaveformData[i + 1] - minValue) / dbScale) * waveformSideHeight;
                    rightWaveformPolyLine.Points.Add(new Point(xLocation, centerHeight + rightRenderHeight));
                }

                leftWaveformPolyLine.Points.Add(new Point(xLocation, centerHeight));
                leftWaveformPolyLine.Points.Add(new Point(0, centerHeight));
                rightWaveformPolyLine.Points.Add(new Point(xLocation, centerHeight));
                rightWaveformPolyLine.Points.Add(new Point(0, centerHeight));

                PathGeometry leftGeometry = new PathGeometry();
                PathFigure leftPathFigure = new PathFigure();
                leftPathFigure.Segments.Add(leftWaveformPolyLine);
                leftGeometry.Figures.Add(leftPathFigure);
                PathGeometry rightGeometry = new PathGeometry();
                PathFigure rightPathFigure = new PathFigure();
                rightPathFigure.Segments.Add(rightWaveformPolyLine);
                rightGeometry.Figures.Add(rightPathFigure);

                this.leftPath.Data = leftGeometry;
                this.rightPath.Data = rightGeometry;
            }
            else
            {
                this.leftPath.Data = null;
                this.rightPath.Data = null;
            }
        }
        #endregion
    }
}
