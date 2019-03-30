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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFSoundVisualizationLib
{
    /// <summary>
    /// A digital LED clock display control with 
    /// hours, minutes, seconds, and hundredths of a second.
    /// </summary>
    [DisplayName("Digital Clock")]
    [Description("Displays timespans as a digital LED clock.")]
    [ToolboxItem(true)]   
    [TemplatePart(Name = "PART_ClockGrid", Type = typeof(Grid))]    
    public class DigitalClock : Control
    {
        #region Fields
        private readonly Rectangle hourRectangle1 = new Rectangle();
        private readonly Rectangle hourRectangle2 = new Rectangle();
        private readonly Rectangle separatorRectangle1 = new Rectangle();
        private readonly Rectangle minuteRectangle1 = new Rectangle();
        private readonly Rectangle minuteRectangle2 = new Rectangle();
        private readonly Rectangle separatorRectangle2 = new Rectangle();
        private readonly Rectangle secondRectangle1 = new Rectangle();
        private readonly Rectangle secondRectangle2 = new Rectangle();
        private readonly Rectangle decimalRectangle = new Rectangle();
        private readonly Rectangle subSecondRectangle1 = new Rectangle();
        private readonly Rectangle subSecondRectangle2 = new Rectangle();

        private readonly VisualBrush oneBrush = new VisualBrush();
        private readonly VisualBrush twoBrush = new VisualBrush();
        private readonly VisualBrush threeBrush = new VisualBrush();
        private readonly VisualBrush fourBrush = new VisualBrush();
        private readonly VisualBrush fiveBrush = new VisualBrush();
        private readonly VisualBrush sixBrush = new VisualBrush();
        private readonly VisualBrush sevenBrush = new VisualBrush();
        private readonly VisualBrush eightBrush = new VisualBrush();
        private readonly VisualBrush nineBrush = new VisualBrush();
        private readonly VisualBrush zeroBrush = new VisualBrush();
        private readonly VisualBrush decimalBrush = new VisualBrush();
        private readonly VisualBrush colonBrush = new VisualBrush();

        private Grid clockGrid;
        #endregion

        #region Dependency Properties
        #region Time
        /// <summary>
        /// Identifies the <see cref="Time" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(TimeSpan), typeof(DigitalClock), new UIPropertyMetadata(TimeSpan.Zero, OnTimeChanged, OnCoerceTime));

        private static object OnCoerceTime(DependencyObject o, object value)
        {
            DigitalClock bigClock = o as DigitalClock;
            if (bigClock != null)
                return bigClock.OnCoerceTime((TimeSpan)value);
            else
                return value;
        }

        private static void OnTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DigitalClock bigClock = o as DigitalClock;
            if (bigClock != null)
                bigClock.OnTimeChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="Time"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="Time"/></param>
        /// <returns>The adjusted value of <see cref="Time"/></returns>
        protected virtual TimeSpan OnCoerceTime(TimeSpan value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="Time"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="Time"/></param>
        /// <param name="newValue">The new value of <see cref="Time"/></param>
        protected virtual void OnTimeChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            this.SplitDigits();
        }

        /// <summary>
        /// Gets or sets the time to be displayed in the Digital Clock.
        /// </summary>
        public TimeSpan Time
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)this.GetValue(TimeProperty);
            }
            set
            {
                this.SetValue(TimeProperty, value);
            }
        }
        #endregion

        #region ShowHours
        /// <summary>
        /// Identifies the <see cref="ShowHours" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowHoursProperty = DependencyProperty.Register("ShowHours", typeof(bool), typeof(DigitalClock), new UIPropertyMetadata(true, OnShowHoursChanged, OnCoerceShowHours));

        private static object OnCoerceShowHours(DependencyObject o, object value)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                return digitalClock.OnCoerceShowHours((bool)value);
            else
                return value;
        }

        private static void OnShowHoursChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                digitalClock.OnShowHoursChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="ShowHours"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="ShowHours"/></param>
        /// <returns>The adjusted value of <see cref="ShowHours"/></returns>
        protected virtual bool OnCoerceShowHours(bool value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="ShowHours"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="ShowHours"/></param>
        /// <param name="newValue">The new value of <see cref="ShowHours"/></param>
        protected virtual void OnShowHoursChanged(bool oldValue, bool newValue)
        {
            this.FormatClockLayout();
            this.SplitDigits();
        }

        /// <summary>
        /// Gets or sets whether the digital clock will show the hours portion
        /// in the digital clock display. This is useful if the times displayed
        /// are always less than an hour.
        /// </summary>
        public bool ShowHours
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)this.GetValue(ShowHoursProperty);
            }
            set
            {
                this.SetValue(ShowHoursProperty, value);
            }
        }
        #endregion

        #region ShowSubSeconds
        /// <summary>
        /// Identifies the <see cref="ShowSubSeconds" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowSubSecondsProperty = DependencyProperty.Register("ShowSubSeconds", typeof(bool), typeof(DigitalClock), new UIPropertyMetadata(false, OnShowSubSecondsChanged, OnCoerceShowSubSeconds));

        private static object OnCoerceShowSubSeconds(DependencyObject o, object value)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                return digitalClock.OnCoerceShowSubSeconds((bool)value);
            else
                return value;
        }

        private static void OnShowSubSecondsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                digitalClock.OnShowSubSecondsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="ShowSubSeconds"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="ShowSubSeconds"/></param>
        /// <returns>The adjusted value of <see cref="ShowSubSeconds"/></returns>
        protected virtual bool OnCoerceShowSubSeconds(bool value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="ShowSubSeconds"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="ShowSubSeconds"/></param>
        /// <param name="newValue">The new value of <see cref="ShowSubSeconds"/></param>
        protected virtual void OnShowSubSecondsChanged(bool oldValue, bool newValue)
        {
            this.FormatClockLayout();
            this.SplitDigits();
        }

        /// <summary>
        /// Gets or sets whether fractions of a second are displayed in the digital
        /// clock display.
        /// </summary>
        public bool ShowSubSeconds
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)this.GetValue(ShowSubSecondsProperty);
            }
            set
            {
                this.SetValue(ShowSubSecondsProperty, value);
            }
        }

        #endregion
        #endregion

        #region Constructors
        static DigitalClock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DigitalClock), new FrameworkPropertyMetadata(typeof(DigitalClock)));
        }
        #endregion

        #region Template Overrides
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code
        /// or internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.clockGrid != null)
                this.clockGrid.Children.Clear();

            this.clockGrid = this.GetTemplateChild("PART_ClockGrid") as Grid;

            this.oneBrush.Visual = this.FindResource("One") as Visual;
            this.oneBrush.Stretch = Stretch.Uniform;
            this.oneBrush.AlignmentY = AlignmentY.Center;
            RenderOptions.SetCachingHint(this.oneBrush, CachingHint.Cache);
            this.twoBrush.Visual = this.FindResource("Two") as Visual;
            this.twoBrush.Stretch = Stretch.Uniform;
            this.twoBrush.AlignmentY = AlignmentY.Center;
            RenderOptions.SetCachingHint(this.twoBrush, CachingHint.Cache);
            this.threeBrush.Visual = this.FindResource("Three") as Visual;
            this.threeBrush.Stretch = Stretch.Uniform;
            this.threeBrush.AlignmentY = AlignmentY.Center;
            RenderOptions.SetCachingHint(this.threeBrush, CachingHint.Cache);
            this.fourBrush.Visual = this.FindResource("Four") as Visual;
            this.fourBrush.Stretch = Stretch.Uniform;
            this.fourBrush.AlignmentY = AlignmentY.Center;
            RenderOptions.SetCachingHint(this.fourBrush, CachingHint.Cache);
            this.fiveBrush.Visual = this.FindResource("Five") as Visual;
            this.fiveBrush.Stretch = Stretch.Uniform;
            this.fiveBrush.AlignmentY = AlignmentY.Center;
            RenderOptions.SetCachingHint(this.fiveBrush, CachingHint.Cache);
            this.sixBrush.Visual = this.FindResource("Six") as Visual;
            this.sixBrush.Stretch = Stretch.Uniform;
            this.sixBrush.AlignmentY = AlignmentY.Center;
            RenderOptions.SetCachingHint(this.sixBrush, CachingHint.Cache);
            this.sevenBrush.Visual = this.FindResource("Seven") as Visual;
            this.sevenBrush.Stretch = Stretch.Uniform;
            this.sevenBrush.AlignmentY = AlignmentY.Center;
            RenderOptions.SetCachingHint(this.sevenBrush, CachingHint.Cache);
            this.eightBrush.Visual = this.FindResource("Eight") as Visual;
            this.eightBrush.Stretch = Stretch.Uniform;
            this.eightBrush.AlignmentY = AlignmentY.Center;
            RenderOptions.SetCachingHint(this.eightBrush, CachingHint.Cache);
            this.nineBrush.Visual = this.FindResource("Nine") as Visual;
            this.nineBrush.Stretch = Stretch.Uniform;
            this.nineBrush.AlignmentY = AlignmentY.Center;
            RenderOptions.SetCachingHint(this.nineBrush, CachingHint.Cache);
            this.zeroBrush.Visual = this.FindResource("Zero") as Visual;
            this.zeroBrush.Stretch = Stretch.Uniform;
            this.zeroBrush.AlignmentY = AlignmentY.Center;
            RenderOptions.SetCachingHint(this.zeroBrush, CachingHint.Cache);
            this.decimalBrush.Visual = this.FindResource("Decimal") as Visual;
            this.decimalBrush.Stretch = Stretch.Uniform;
            this.decimalBrush.AlignmentY = AlignmentY.Bottom;
            RenderOptions.SetCachingHint(this.decimalBrush, CachingHint.Cache);
            this.colonBrush.Visual = this.FindResource("Colon") as Visual;
            this.colonBrush.Stretch = Stretch.Uniform;
            this.colonBrush.AlignmentY = AlignmentY.Center;
            RenderOptions.SetCachingHint(this.colonBrush, CachingHint.Cache);

            this.separatorRectangle1.Fill = this.GetDigitElement(':');
            this.separatorRectangle2.Fill = this.GetDigitElement(':');
            this.decimalRectangle.Fill = this.GetDigitElement('.');

            this.FormatClockLayout();
            this.SplitDigits();
        }

        /// <summary>
        /// Called whenever the control's template changes. 
        /// </summary>
        /// <param name="oldTemplate">The old template</param>
        /// <param name="newTemplate">The new template</param>
        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            base.OnTemplateChanged(oldTemplate, newTemplate);
        }
        #endregion

        #region Private Utility Methods
        private void FormatClockLayout()
        {
            if (this.clockGrid == null)
                return;

            ColumnDefinitionCollection clockColumns = this.clockGrid.ColumnDefinitions;
            clockColumns.Clear();
            this.clockGrid.Children.Clear();

            Thickness digitMargin = new Thickness(5.0d);
            int columnIndex = 0;
            double gridLength = 0;
            double gridHeight = 115;

            if (this.ShowHours)
            {
                // First hours digit
                ColumnDefinition hours1Column = new ColumnDefinition();
                hours1Column.Width = new GridLength(50, GridUnitType.Star);
                clockColumns.Add(hours1Column);
                this.hourRectangle1.Margin = digitMargin;
                Grid.SetColumn(this.hourRectangle1, columnIndex);
                this.clockGrid.Children.Add(this.hourRectangle1);
                gridLength += 60;
                columnIndex++;

                // Second hours digit
                ColumnDefinition hours2Column = new ColumnDefinition();
                hours2Column.Width = new GridLength(50, GridUnitType.Star);
                clockColumns.Add(hours2Column);
                this.hourRectangle2.Margin = digitMargin;
                Grid.SetColumn(this.hourRectangle2, columnIndex);
                this.clockGrid.Children.Add(this.hourRectangle2);
                gridLength += 60;
                columnIndex++;

                // Hours separator
                ColumnDefinition seaparator1Column = new ColumnDefinition();
                seaparator1Column.Width = new GridLength(20, GridUnitType.Star);
                clockColumns.Add(seaparator1Column);
                this.separatorRectangle1.Margin = digitMargin;
                Grid.SetColumn(this.separatorRectangle1, columnIndex);
                this.clockGrid.Children.Add(this.separatorRectangle1);
                gridLength += 30;
                columnIndex++;
            }

            // First minutes digit            
            ColumnDefinition minutes1Column = new ColumnDefinition();
            minutes1Column.Width = new GridLength(50, GridUnitType.Star);
            clockColumns.Add(minutes1Column);
            this.minuteRectangle1.Margin = digitMargin;
            Grid.SetColumn(this.minuteRectangle1, columnIndex);
            this.clockGrid.Children.Add(this.minuteRectangle1);
            gridLength += 60;
            columnIndex++;

            // Second minutes digit
            ColumnDefinition minutes2Column = new ColumnDefinition();
            minutes2Column.Width = new GridLength(50, GridUnitType.Star);
            clockColumns.Add(minutes2Column);
            this.minuteRectangle2.Margin = digitMargin;
            Grid.SetColumn(this.minuteRectangle2, columnIndex);
            this.clockGrid.Children.Add(this.minuteRectangle2);
            gridLength += 60;
            columnIndex++;

            // Minutes separator
            ColumnDefinition seaparator2Column = new ColumnDefinition();
            seaparator2Column.Width = new GridLength(20, GridUnitType.Star);
            clockColumns.Add(seaparator2Column);
            this.separatorRectangle2.Margin = digitMargin;
            Grid.SetColumn(this.separatorRectangle2, columnIndex);
            this.clockGrid.Children.Add(this.separatorRectangle2);
            gridLength += 30;
            columnIndex++;

            // First seconds digit
            ColumnDefinition seconds1Column = new ColumnDefinition();
            seconds1Column.Width = new GridLength(50, GridUnitType.Star);
            clockColumns.Add(seconds1Column);
            this.secondRectangle1.Margin = digitMargin;
            Grid.SetColumn(this.secondRectangle1, columnIndex);
            this.clockGrid.Children.Add(this.secondRectangle1);
            gridLength += 60;
            columnIndex++;

            // Second seconds digit
            ColumnDefinition seconds2Column = new ColumnDefinition();
            seconds2Column.Width = new GridLength(50, GridUnitType.Star);
            clockColumns.Add(seconds2Column);
            this.secondRectangle2.Margin = digitMargin;
            Grid.SetColumn(this.secondRectangle2, columnIndex);
            this.clockGrid.Children.Add(this.secondRectangle2);
            gridLength += 60;
            columnIndex++;

            if (this.ShowSubSeconds)
            {
                // Subseconds decimal point
                ColumnDefinition subSecondsDecimalColumn = new ColumnDefinition();
                subSecondsDecimalColumn.Width = new GridLength(20, GridUnitType.Star);
                clockColumns.Add(subSecondsDecimalColumn);
                this.decimalRectangle.Margin = digitMargin;
                Grid.SetColumn(this.decimalRectangle, columnIndex);
                this.clockGrid.Children.Add(this.decimalRectangle);
                gridLength += 30;
                columnIndex++;

                // First subseconds digit
                ColumnDefinition subSeconds1Column = new ColumnDefinition();
                subSeconds1Column.Width = new GridLength(50, GridUnitType.Star);
                clockColumns.Add(subSeconds1Column);
                this.subSecondRectangle1.Margin = digitMargin;
                Grid.SetColumn(this.subSecondRectangle1, columnIndex);
                this.clockGrid.Children.Add(this.subSecondRectangle1);
                gridLength += 60;
                columnIndex++;

                // Second subseconds digit
                ColumnDefinition subSeconds2Column = new ColumnDefinition();
                subSeconds2Column.Width = new GridLength(50, GridUnitType.Star);
                clockColumns.Add(subSeconds2Column);
                this.subSecondRectangle2.Margin = digitMargin;
                Grid.SetColumn(this.subSecondRectangle2, columnIndex);
                this.clockGrid.Children.Add(this.subSecondRectangle2);
                gridLength += 60;
                columnIndex++;
            }

            this.clockGrid.Width = gridLength;
            this.clockGrid.Height = gridHeight;
        }

        private void SplitDigits()
        {
            if (this.clockGrid == null)
                return;

            const string timeFormat = "00";

            string hoursString = this.Time.Hours.ToString(timeFormat);
            this.hourRectangle1.Fill = this.GetDigitElement(hoursString[0]);
            this.hourRectangle2.Fill = this.GetDigitElement(hoursString[1]);

            string minutesString = this.Time.Minutes.ToString(timeFormat);
            this.minuteRectangle1.Fill = this.GetDigitElement(minutesString[0]);
            this.minuteRectangle2.Fill = this.GetDigitElement(minutesString[1]);

            string secondsString = this.Time.Seconds.ToString(timeFormat);
            this.secondRectangle1.Fill = this.GetDigitElement(secondsString[0]);
            this.secondRectangle2.Fill = this.GetDigitElement(secondsString[1]);

            string subSecondsString = (this.Time.Milliseconds / 1000.0d).ToString("0.00");
            this.subSecondRectangle1.Fill = this.GetDigitElement(subSecondsString[2]);
            this.subSecondRectangle2.Fill = this.GetDigitElement(subSecondsString[3]);
        }

        private VisualBrush GetDigitElement(char digitChar)
        {
            switch (digitChar)
            {
                case '1':
                    return this.oneBrush;
                case '2':
                    return this.twoBrush;
                case '3':
                    return this.threeBrush;
                case '4':
                    return this.fourBrush;
                case '5':
                    return this.fiveBrush;
                case '6':
                    return this.sixBrush;
                case '7':
                    return this.sevenBrush;
                case '8':
                    return this.eightBrush;
                case '9':
                    return this.nineBrush;
                case '0':
                    return this.zeroBrush;
                case '.':
                    return this.decimalBrush;
                case ':':
                    return this.colonBrush;
            }
            return null;
        }
        #endregion
    }
}
