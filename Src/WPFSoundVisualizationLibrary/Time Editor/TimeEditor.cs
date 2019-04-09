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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFSoundVisualizationLib
{
    /// <summary>
    /// A control that allows the user to
    /// see and edit TimeSpans.
    /// </summary>
    [DisplayName("Time Editor")]
    [Description("Displays and edits time spans.")]
    [ToolboxItem(true)]    
    [TemplatePart(Name = "PART_HoursTextBox", Type = typeof(TextBox)),
    TemplatePart(Name = "PART_MinutesTextBox", Type = typeof(TextBox)),
    TemplatePart(Name = "PART_SecondsTextBox", Type = typeof(TextBox)),
    TemplatePart(Name = "PART_SpinUpButton", Type = typeof(Button)),
    TemplatePart(Name = "PART_SpinDownButton", Type = typeof(Button))]
    public class TimeEditor : Control
    {
        #region  Fields
        private bool updatingValue;
        private TextBox hoursTextBox;
        private TextBox minutesTextBox;
        private TextBox secondsTextBox;
        private ButtonBase spinUpButton;
        private ButtonBase spinDownButton;
        private string hoursMask;
        private string minutesMask;
        private string secondsMask;
        private bool hoursLoaded;
        private bool minutesLoaded;
        private bool secondsLoaded;
        private TimeEditFields activeField = TimeEditFields.Seconds;
        #endregion

        #region Enums
        private enum TimeEditFields
        {
            Hours,
            Minutes,
            Seconds
        }
        #endregion

        #region Constructors
        static TimeEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeEditor), new FrameworkPropertyMetadata(typeof(TimeEditor)));
        }

        /// <summary>
        /// Creates an instance of TimeEditor
        /// </summary>
        public TimeEditor()
        {
            this.UpdateMasks();            
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

            this.hoursTextBox = this.GetTemplateChild("PART_HoursTextBox") as TextBox;
            this.minutesTextBox = this.GetTemplateChild("PART_MinutesTextBox") as TextBox;
            this.secondsTextBox = this.GetTemplateChild("PART_SecondsTextBox") as TextBox;
            this.spinUpButton = this.GetTemplateChild("PART_SpinUpButton") as Button;
            this.spinDownButton = this.GetTemplateChild("PART_SpinDownButton") as Button;

            if (this.hoursTextBox != null)
            {
                this.hoursTextBox.PreviewTextInput += this.HoursPreviewTextInput;
                this.hoursTextBox.LostFocus += this.HoursLostFocus;
                this.hoursTextBox.PreviewKeyDown += this.HoursKeyDown;
                this.hoursTextBox.TextChanged += this.HoursTextChanged;
                this.hoursTextBox.GotFocus += this.HoursGotFocus;
            }
            if (this.minutesTextBox != null)
            {
                this.minutesTextBox.PreviewTextInput += this.MinutesPreviewTextInput;
                this.minutesTextBox.LostFocus += this.MinutesLostFocus;
                this.minutesTextBox.PreviewKeyDown += this.MinutesKeyDown;
                this.minutesTextBox.TextChanged += this.MinutesTextChanged;
                this.minutesTextBox.GotFocus += this.MinutesGotFocus;
            }
            if (this.secondsTextBox != null)
            {
                this.secondsTextBox.PreviewTextInput += this.SecondsPreviewTextInput;
                this.secondsTextBox.LostFocus += this.SecondsLostFocus;
                this.secondsTextBox.PreviewKeyDown += this.SecondsKeyDown;
                this.secondsTextBox.TextChanged += this.SecondsTextChanged;
                this.secondsTextBox.GotFocus += this.SecondsGotFocus;
            }

            if (this.spinUpButton != null)
            {
                this.spinUpButton.Focusable = false;
                this.spinUpButton.Click += this.SpinUpClick;
            }
            if (this.spinDownButton != null)
            {
                this.spinDownButton.Focusable = false;
                this.spinDownButton.Click += this.SpinDownClick;
            }

            this.CopyValuesToTextBoxes();
            this.UpdateReadOnly();
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

        #region Dependency Properties
        #region IsReadOnly
        /// <summary>
        /// Identifies the <see cref="IsReadOnly" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TimeEditor), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsReadOnlyChanged), new CoerceValueCallback(OnCoerceIsReadOnly)));

        private static object OnCoerceIsReadOnly(DependencyObject o, object value)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                return timeEditor.OnCoerceIsReadOnly((bool)value);
            else
                return value;
        }

        private static void OnIsReadOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                timeEditor.OnIsReadOnlyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="IsReadOnly"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="IsReadOnly"/></param>
        /// <returns>The adjusted value of <see cref="IsReadOnly"/></returns>
        protected virtual bool OnCoerceIsReadOnly(bool value)
        {            
            return value;
        }

        /// <summary>
        /// Called after the <see cref="IsReadOnly"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="IsReadOnly"/></param>
        /// <param name="newValue">The new value of <see cref="IsReadOnly"/></param>
        protected virtual void OnIsReadOnlyChanged(bool oldValue, bool newValue)
        {
            this.UpdateReadOnly();
        }

        /// <summary>
        /// Gets or sets whether the TimeEditor is readonly
        /// </summary>
        public bool IsReadOnly
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)this.GetValue(IsReadOnlyProperty);
            }
            set
            {
                this.SetValue(IsReadOnlyProperty, value);
            }
        }
        
        #endregion

        #region Minimum
        /// <summary>
        /// Identifies the <see cref="Minimum" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(TimeSpan), typeof(TimeEditor), new UIPropertyMetadata(TimeSpan.Zero, OnMinimumChanged, OnCoerceMinimum));

        private static object OnCoerceMinimum(DependencyObject o, object value)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                return timeEditor.OnCoerceMinimum((TimeSpan)value);
            else
                return value;
        }

        private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                timeEditor.OnMinimumChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="Minimum"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="Minimum"/></param>
        /// <returns>The adjusted value of <see cref="Minimum"/></returns>
        protected virtual TimeSpan OnCoerceMinimum(TimeSpan value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="Minimum"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="Minimum"/></param>
        /// <param name="newValue">The new value of <see cref="Minimum"/></param>
        protected virtual void OnMinimumChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            this.CoerceValue(MaximumProperty);
            this.CoerceValue(ValueProperty);
            this.UpdateMasks();
        }

        /// <summary>
        /// Gets or sets the minimum value that is allowed by the TimeEditor
        /// </summary>
        public TimeSpan Minimum
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)this.GetValue(MinimumProperty);
            }
            set
            {
                this.SetValue(MinimumProperty, value);
            }
        }
        #endregion

        #region Maximum
        /// <summary>
        /// Identifies the <see cref="Maximum" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(TimeSpan), typeof(TimeEditor), new UIPropertyMetadata(TimeSpan.MaxValue, OnMaximumChanged, OnCoerceMaximum));

        private static object OnCoerceMaximum(DependencyObject o, object value)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                return timeEditor.OnCoerceMaximum((TimeSpan)value);
            else
                return value;
        }

        private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                timeEditor.OnMaximumChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="Maximum"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="Maximum"/></param>
        /// <returns>The adjusted value of <see cref="Maximum"/></returns>
        protected virtual TimeSpan OnCoerceMaximum(TimeSpan value)
        {
            if (value < this.Minimum)
                return this.Minimum;
            return value;
        }

        /// <summary>
        /// Called after the <see cref="Maximum"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="Maximum"/></param>
        /// <param name="newValue">The new value of <see cref="Maximum"/></param>
        protected virtual void OnMaximumChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            this.UpdateMasks();
            this.NeedsHour = (newValue >= TimeSpan.FromHours(1.0d));
        }

        /// <summary>
        /// Gets or sets the maximum value that is allowed by the TimeEditor
        /// </summary>
        public TimeSpan Maximum
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)this.GetValue(MaximumProperty);
            }
            set
            {
                this.SetValue(MaximumProperty, value);
            }
        }
        #endregion

        #region Value
        /// <summary>
        /// Identifies the <see cref="Value" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(TimeSpan), typeof(TimeEditor), new UIPropertyMetadata(TimeSpan.Zero, OnValueChanged, OnCoerceValue));

        private static object OnCoerceValue(DependencyObject o, object value)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                return timeEditor.OnCoerceValue((TimeSpan)value);
            else
                return value;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                timeEditor.OnValueChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="Value"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="Value"/></param>
        /// <returns>The adjusted value of <see cref="Value"/></returns>
        protected virtual TimeSpan OnCoerceValue(TimeSpan value)
        {
            if (value < this.Minimum)
                return this.Minimum;
            if (value > this.Maximum)
                return this.Maximum;
            return value;
        }

        /// <summary>
        /// Called after the <see cref="Value"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="Value"/></param>
        /// <param name="newValue">The new value of <see cref="Value"/></param>
        protected virtual void OnValueChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            if (!this.updatingValue) // Avoid reentrance
            {
                this.updatingValue = true;
                this.Hours = newValue.Hours;
                this.Minutes = newValue.Minutes;
                this.Seconds = newValue.Seconds + (newValue.Milliseconds / 1000.0d);
                this.CopyValuesToTextBoxes();
                this.updatingValue = false;
            }
        }

        /// <summary>
        /// Gets or sets the value of the TimeEditor
        /// </summary>
        public TimeSpan Value
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)this.GetValue(ValueProperty);
            }
            set
            {
                this.SetValue(ValueProperty, value);
            }
        }

        #endregion

        #region Hours
        /// <summary>
        /// Identifies the <see cref="Hours" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty HoursProperty = DependencyProperty.Register("Hours", typeof(int), typeof(TimeEditor), new UIPropertyMetadata(0, OnHoursChanged, OnCoerceHours));

        private static object OnCoerceHours(DependencyObject o, object value)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                return timeEditor.OnCoerceHours((int)value);
            else
                return value;
        }

        private static void OnHoursChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                timeEditor.OnHoursChanged((int)e.OldValue, (int)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="Hours"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="Hours"/></param>
        /// <returns>The adjusted value of <see cref="Hours"/></returns>
        protected virtual int OnCoerceHours(int value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="Hours"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="Hours"/></param>
        /// <param name="newValue">The new value of <see cref="Hours"/></param>
        protected virtual void OnHoursChanged(int oldValue, int newValue)
        {
            if (this.updatingValue)
                return;

            const int maxHours = 23;
            if (newValue > maxHours)
                newValue = maxHours;
            
            TimeSpan value = new TimeSpan(0, newValue, this.Minutes, (int)this.Seconds, ((int)(this.Seconds * 1000)) % 1000);
            value = TimeSpan.FromSeconds(Math.Min(Math.Max(value.TotalSeconds, this.Minimum.TotalSeconds), this.Maximum.TotalSeconds));
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets just the hours portion of the <see cref="Value"/>
        /// </summary>
        public int Hours
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)this.GetValue(HoursProperty);
            }
            set
            {
                this.SetValue(HoursProperty, value);
            }
        }
        #endregion

        #region Minutes
        /// <summary>
        /// Identifies the <see cref="Minutes" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty MinutesProperty = DependencyProperty.Register("Minutes", typeof(int), typeof(TimeEditor), new UIPropertyMetadata(0, OnMinutesChanged, OnCoerceMinutes));

        private static object OnCoerceMinutes(DependencyObject o, object value)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                return timeEditor.OnCoerceMinutes((int)value);
            else
                return value;
        }

        private static void OnMinutesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                timeEditor.OnMinutesChanged((int)e.OldValue, (int)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="Minutes"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="Minutes"/></param>
        /// <returns>The adjusted value of <see cref="Minutes"/></returns>
        protected virtual int OnCoerceMinutes(int value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="Minutes"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="Minutes"/></param>
        /// <param name="newValue">The new value of <see cref="Minutes"/></param>
        protected virtual void OnMinutesChanged(int oldValue, int newValue)
        {
            if (this.updatingValue)
                return;

            const int maxMinutes = 59;
            if (newValue > maxMinutes)
                newValue = maxMinutes;

            TimeSpan value = new TimeSpan(0, this.Hours, newValue, (int)this.Seconds, ((int)(this.Seconds * 1000)) % 1000);
            value = TimeSpan.FromSeconds(Math.Min(Math.Max(value.TotalSeconds, this.Minimum.TotalSeconds), this.Maximum.TotalSeconds));
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets just the hours portion of the <see cref="Minutes"/>
        /// </summary>
        public int Minutes
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)this.GetValue(MinutesProperty);
            }
            set
            {
                this.SetValue(MinutesProperty, value);
            }
        }
        #endregion

        #region Seconds
        /// <summary>
        /// Identifies the <see cref="Seconds" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SecondsProperty = DependencyProperty.Register("Seconds", typeof(double), typeof(TimeEditor), new UIPropertyMetadata(0.0d, OnSecondsChanged, OnCoerceSeconds));

        private static object OnCoerceSeconds(DependencyObject o, object value)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                return timeEditor.OnCoerceSeconds((double)value);
            else
                return value;
        }

        private static void OnSecondsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                timeEditor.OnSecondsChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="Seconds"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="Seconds"/></param>
        /// <returns>The adjusted value of <see cref="Seconds"/></returns>
        protected virtual double OnCoerceSeconds(double value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="Seconds"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="Seconds"/></param>
        /// <param name="newValue">The new value of <see cref="Seconds"/></param>
        protected virtual void OnSecondsChanged(double oldValue, double newValue)
        {
            if (this.updatingValue)
                return;

            const double maxSeconds = 60.0d;
            if (newValue > maxSeconds)
                newValue = maxSeconds - 0.01;

            TimeSpan value = new TimeSpan(0, this.Hours, this.Minutes, (int)newValue, ((int)(newValue * 1000)) % 1000);
            value = TimeSpan.FromSeconds(Math.Min(Math.Max(value.TotalSeconds, this.Minimum.TotalSeconds), this.Maximum.TotalSeconds));
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets just the seconds portion of the <see cref="Value"/>
        /// </summary>
        public double Seconds
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)this.GetValue(SecondsProperty);
            }
            set
            {
                this.SetValue(SecondsProperty, value);
            }
        }
        #endregion

        #region CaretBrush
        /// <summary>
        /// Identifies the <see cref="CaretBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CaretBrushProperty = DependencyProperty.Register("CaretBrush", typeof(Brush), typeof(TimeEditor), new UIPropertyMetadata(null, OnCaretBrushChanged, OnCoerceCaretBrush));

        private static object OnCoerceCaretBrush(DependencyObject o, object value)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                return timeEditor.OnCoerceCaretBrush((Brush)value);
            else
                return value;
        }

        private static void OnCaretBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                timeEditor.OnCaretBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="CaretBrush"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="CaretBrush"/></param>
        /// <returns>The adjusted value of <see cref="CaretBrush"/></returns>
        protected virtual Brush OnCoerceCaretBrush(Brush value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="CaretBrush"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="CaretBrush"/></param>
        /// <param name="newValue">The new value of <see cref="CaretBrush"/></param>
        protected virtual void OnCaretBrushChanged(Brush oldValue, Brush newValue)
        {
            
        }

        /// <summary>
        /// Gets or sets the brush used by the caret in text fields.
        /// </summary>
        public Brush CaretBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)this.GetValue(CaretBrushProperty);
            }
            set
            {
                this.SetValue(CaretBrushProperty, value);
            }
        }
        #endregion

        #region NeedsHour
        /// <summary>
        /// Identifies the <see cref="NeedsHour" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty NeedsHourProperty = DependencyProperty.Register("NeedsHour", typeof(bool), typeof(TimeEditor), new UIPropertyMetadata(false, OnNeedsHourChanged, OnCoerceNeedsHour));

        private static object OnCoerceNeedsHour(DependencyObject o, object value)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                return timeEditor.OnCoerceNeedsHour((bool)value);
            else
                return value;
        }
        
        private static void OnNeedsHourChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TimeEditor timeEditor = o as TimeEditor;
            if (timeEditor != null)
                timeEditor.OnNeedsHourChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="NeedsHour"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="NeedsHour"/></param>
        /// <returns>The adjusted value of <see cref="NeedsHour"/></returns>
        protected virtual bool OnCoerceNeedsHour(bool value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="NeedsHour"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="NeedsHour"/></param>
        /// <param name="newValue">The new value of <see cref="NeedsHour"/></param>
        protected virtual void OnNeedsHourChanged(bool oldValue, bool newValue)
        {
            
        }

        /// <summary>
        /// Gets whether the <see cref="Maximum"/> value is greater than the 1 hour
        /// to indicate whether the hour field needs to be displayed.
        /// </summary>
        public bool NeedsHour
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)this.GetValue(NeedsHourProperty);
            }
            protected set
            {
                this.SetValue(NeedsHourProperty, value);
            }
        }

        #endregion
        #endregion

        #region Routed Events
        #region Editing        
        /// <summary>
        /// Identifies the <see cref="Editing"/> routed event.
        /// </summary>
        public static readonly RoutedEvent EditingEvent = EventManager.RegisterRoutedEvent(
            "Editing", RoutingStrategy.Bubble, typeof(TimeEditorEventHandler), typeof(TimeEditor));

        /// <summary>
        /// Occurs when the time editor's values are being edited.
        /// </summary>
        public event TimeEditorEventHandler Editing
        {
            add { this.AddHandler(EditingEvent, value); }
            remove { this.RemoveHandler(EditingEvent, value); }
        }

        private TimeEditorEventArgs RaiseEditingEvent(TimeEditorAction action, TimeEditorField activeField)
        {
            TimeEditorEventArgs timeEditorArgs = new TimeEditorEventArgs(TimeEditor.EditingEvent, action, activeField);
            this.RaiseEvent(timeEditorArgs);
            return timeEditorArgs;
        }
        #endregion
        #endregion

        #region Event Handlers
        #region Buttons
        private void SpinUpClick(object sender, RoutedEventArgs e)
        {
            switch (this.activeField)
            {
                case TimeEditFields.Hours:
                    if(!this.RaiseEditingEvent(TimeEditorAction.UpPressed, TimeEditorField.Hours).Handled)
                        this.IncrementHours();            
                    break;
                case TimeEditFields.Minutes:
                    if (!this.RaiseEditingEvent(TimeEditorAction.UpPressed, TimeEditorField.Minutes).Handled)
                        this.IncrementMinutes();
                    break;
                case TimeEditFields.Seconds:
                    if (!this.RaiseEditingEvent(TimeEditorAction.UpPressed, TimeEditorField.Seconds).Handled)
                        this.IncrementSeconds();
                    break;
            }
        }

        private void SpinDownClick(object sender, RoutedEventArgs e)
        {
            switch (this.activeField)
            {
                case TimeEditFields.Hours:
                    if (!this.RaiseEditingEvent(TimeEditorAction.DownPressed, TimeEditorField.Hours).Handled)
                        this.DecrementHours();
                    break;
                case TimeEditFields.Minutes:
                    if (!this.RaiseEditingEvent(TimeEditorAction.DownPressed, TimeEditorField.Minutes).Handled)
                        this.DecrementMinutes();
                    break;
                case TimeEditFields.Seconds:
                    if (!this.RaiseEditingEvent(TimeEditorAction.DownPressed, TimeEditorField.Seconds).Handled)
                        this.DecrementSeconds();
                    break;
            }
        }
        #endregion

        #region Key Presses
        private void HoursKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Tab:
                    if (!this.RaiseEditingEvent(TimeEditorAction.ValueEntered, TimeEditorField.Hours).Handled)
                        this.ValidateHours();
                    else
                        this.CopyValuesToTextBoxes();
                    break;
                case Key.Up:
                    if (!this.RaiseEditingEvent(TimeEditorAction.UpPressed, TimeEditorField.Hours).Handled)
                        this.IncrementHours();
                    break;
                case Key.Down:
                    if (!this.RaiseEditingEvent(TimeEditorAction.DownPressed, TimeEditorField.Hours).Handled)
                        this.DecrementHours();
                    break;
                case Key.Right:
                    if (this.hoursTextBox.CaretIndex >= this.hoursTextBox.Text.Length && this.minutesTextBox != null)
                    {
                        this.minutesTextBox.Focus();
                        this.minutesTextBox.CaretIndex = 0;
                        e.Handled = true;
                    }
                    break;
                case Key.Escape:
                    if (!this.RaiseEditingEvent(TimeEditorAction.ValueEntered, TimeEditorField.Hours).Handled)
                    {
                        this.ValidateHours();
                        this.RemoveFocusToTop();
                    }
                    else
                        this.CopyValuesToTextBoxes();
                    break;
                default:
                    // Do nothing
                    break;
            }
        }

        private void MinutesKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Tab:
                    if (!this.RaiseEditingEvent(TimeEditorAction.ValueEntered, TimeEditorField.Minutes).Handled)
                        this.ValidateMinutes();
                    else
                        this.CopyValuesToTextBoxes();
                    break;
                case Key.Up:
                    if (!this.RaiseEditingEvent(TimeEditorAction.UpPressed, TimeEditorField.Minutes).Handled)
                        this.IncrementMinutes();
                    break;
                case Key.Down:
                    if (!this.RaiseEditingEvent(TimeEditorAction.DownPressed, TimeEditorField.Minutes).Handled)
                        this.DecrementMinutes();
                    break;
                case Key.Right:
                    if (this.minutesTextBox.CaretIndex >= this.minutesTextBox.Text.Length && this.secondsTextBox != null)
                    {
                        this.secondsTextBox.Focus();
                        this.secondsTextBox.CaretIndex = 0;
                        e.Handled = true;
                    }
                    break;
                case Key.Left:
                    if (this.minutesTextBox.CaretIndex <= 0 && this.hoursTextBox != null && this.hoursTextBox.IsVisible)
                    {
                        this.hoursTextBox.Focus();
                        this.hoursTextBox.CaretIndex = this.hoursTextBox.Text.Length;
                        e.Handled = true;
                    }
                    break;
                case Key.Escape:
                    if (!this.RaiseEditingEvent(TimeEditorAction.ValueEntered, TimeEditorField.Minutes).Handled)
                    {
                        this.ValidateMinutes();
                        this.RemoveFocusToTop();
                    }
                    else
                        this.CopyValuesToTextBoxes();
                    break;
                default:
                    // Do nothing
                    break;
            }

        }

        private void SecondsKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Tab:
                    if (!this.RaiseEditingEvent(TimeEditorAction.ValueEntered, TimeEditorField.Seconds).Handled)
                        this.ValidateSeconds();
                    else
                        this.CopyValuesToTextBoxes();
                    break;
                case Key.Up:
                    if (!this.RaiseEditingEvent(TimeEditorAction.UpPressed, TimeEditorField.Seconds).Handled)
                        this.IncrementSeconds();
                    break;
                case Key.Down:
                    if (!this.RaiseEditingEvent(TimeEditorAction.DownPressed, TimeEditorField.Seconds).Handled)
                        this.DecrementSeconds();
                    break;
                case Key.Left:
                    if (this.secondsTextBox.CaretIndex <= 0 && this.minutesTextBox != null)
                    {
                        this.minutesTextBox.Focus();
                        this.minutesTextBox.CaretIndex = this.minutesTextBox.Text.Length;
                        e.Handled = true;
                    }
                    break;
                case Key.Escape:
                    if (!this.RaiseEditingEvent(TimeEditorAction.ValueEntered, TimeEditorField.Seconds).Handled)
                    {
                        this.ValidateSeconds();
                        this.RemoveFocusToTop();
                    }
                    else
                        this.CopyValuesToTextBoxes();
                    break;
                default:
                    // Do nothing
                    break;
            }

        }
        #endregion

        #region Text Input
        private void HoursPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string text = this.hoursTextBox.Text.Remove(this.hoursTextBox.SelectionStart, this.hoursTextBox.SelectionLength);
            text = text.Insert(this.hoursTextBox.SelectionStart, e.Text);
            e.Handled = !(Regex.IsMatch(text, this.hoursMask));
            base.OnPreviewTextInput(e);
        }

        private void MinutesPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string text = this.minutesTextBox.Text.Remove(this.minutesTextBox.SelectionStart, this.minutesTextBox.SelectionLength);
            text = text.Insert(this.minutesTextBox.SelectionStart, e.Text);
            e.Handled = !(Regex.IsMatch(text, this.minutesMask));
            base.OnPreviewTextInput(e);
        }

        private void SecondsPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string text = this.secondsTextBox.Text.Remove(this.secondsTextBox.SelectionStart, this.secondsTextBox.SelectionLength);
            text = text.Insert(this.secondsTextBox.SelectionStart, e.Text);
            e.Handled = !(Regex.IsMatch(text, this.secondsMask));
            base.OnPreviewTextInput(e);
        }

        private void SecondsTextChanged(object sender, TextChangedEventArgs e)
        {
            // Workaround for text clearing during initialization in OnTextContainerChanged()
            if (string.IsNullOrEmpty(this.secondsTextBox.Text) && !this.secondsLoaded)
            {
                this.CopyValuesToTextBoxes();
                this.secondsLoaded = true;
            }
        }

        private void MinutesTextChanged(object sender, TextChangedEventArgs e)
        {
            // Workaround for text clearing during initialization in OnTextContainerChanged()
            if (string.IsNullOrEmpty(this.minutesTextBox.Text) && !this.minutesLoaded)
            {
                this.CopyValuesToTextBoxes();
                this.minutesLoaded = true;
            }
        }

        private void HoursTextChanged(object sender, TextChangedEventArgs e)
        {
            // Workaround for text clearing during initialization in OnTextContainerChanged()
            if (string.IsNullOrEmpty(this.hoursTextBox.Text) && !this.hoursLoaded)
            {
                this.CopyValuesToTextBoxes();
                this.hoursLoaded = true;
            }
        }
        #endregion

        #region Focus
        private void HoursLostFocus(object sender, RoutedEventArgs e)
        {
            this.ValidateHours();
        }

        private void MinutesLostFocus(object sender, RoutedEventArgs e)
        {
            this.ValidateMinutes();
        }

        private void SecondsLostFocus(object sender, RoutedEventArgs e)
        {
            this.ValidateSeconds();
        }

        private void HoursGotFocus(object sender, RoutedEventArgs e)
        {
            this.activeField = TimeEditFields.Hours;
        }

        private void MinutesGotFocus(object sender, RoutedEventArgs e)
        {
            this.activeField = TimeEditFields.Minutes;
        }

        private void SecondsGotFocus(object sender, RoutedEventArgs e)
        {
            this.activeField = TimeEditFields.Seconds;
        }

        /// <summary>
        /// Invoked whenever an unhandled System.Windows.UIElement.GotFocus event reaches
        /// this element in its route.
        /// </summary>
        /// <param name="e">The System.Windows.RoutedEventArgs that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            switch (this.activeField)
            {
                case TimeEditFields.Hours:
                    if (this.hoursTextBox != null)
                        this.hoursTextBox.Focus();
                    break;
                case TimeEditFields.Minutes:
                    if (this.minutesTextBox != null)
                        this.minutesTextBox.Focus();
                    break;
                case TimeEditFields.Seconds:
                    if (this.secondsTextBox != null)
                        this.secondsTextBox.Focus();
                    break;
            }

            base.OnGotFocus(e);
        }
        #endregion
        #endregion

        #region Private Utility Methods
        private void IncrementHours()
        {
            this.Value = this.Value.Add(TimeSpan.FromHours(1));
            this.ValidateHours();
        }

        private void DecrementHours()
        {
            this.Value = this.Value.Subtract(TimeSpan.FromHours(1));
            this.ValidateHours();
        }

        private void IncrementMinutes()
        {
            this.Value = this.Value.Add(TimeSpan.FromMinutes(1));
            this.ValidateMinutes();
        }

        private void DecrementMinutes()
        {
            this.Value = this.Value.Subtract(TimeSpan.FromMinutes(1));
            this.ValidateMinutes();
        }

        private void IncrementSeconds()
        {
            this.Value = this.Value.Add(TimeSpan.FromSeconds(1));
            this.ValidateSeconds();
        }

        private void DecrementSeconds()
        {
            this.Value = this.Value.Subtract(TimeSpan.FromSeconds(1));
            this.ValidateSeconds();
        }

        private void ValidateHours()
        {
            if (this.hoursTextBox == null)
                return;

            int hours = 0;
            int.TryParse(this.hoursTextBox.Text, out hours);
            this.Hours = hours;
            this.CopyValuesToTextBoxes();
        }

        private void ValidateMinutes()
        {
            if (this.minutesTextBox == null)
                return;

            int minutes = 0;
            int.TryParse(this.minutesTextBox.Text, out minutes);
            this.Minutes = minutes;
            this.CopyValuesToTextBoxes();
        }

        private void ValidateSeconds()
        {
            if (this.secondsTextBox == null)
                return;

            double seconds = 0;
            double.TryParse(this.secondsTextBox.Text, out seconds);
            this.Seconds = seconds;
            this.CopyValuesToTextBoxes();
        }

        private void UpdateMasks()
        {
            this.hoursMask = CreateMask(0, this.Minimum.TotalHours, false);
            this.minutesMask = CreateMask(0, Math.Min(60, this.Maximum.TotalMinutes), false);
            this.secondsMask = CreateMask(0, Math.Min(60, this.Maximum.TotalSeconds), true);
            this.CoerceValue(ValueProperty);
        }

        private void UpdateReadOnly()
        {
            if (this.hoursTextBox != null)
                this.hoursTextBox.IsReadOnly = this.IsReadOnly;
            if (this.minutesTextBox != null)
                this.minutesTextBox.IsReadOnly = this.IsReadOnly;
            if (this.secondsTextBox != null)
                this.secondsTextBox.IsReadOnly = this.IsReadOnly;
            if (this.spinUpButton != null)
                this.spinUpButton.IsEnabled = !this.IsReadOnly;
            if (this.spinDownButton != null)
                this.spinDownButton.IsEnabled = !this.IsReadOnly;
        }

        private static string CreateMask(double minValue, double maxValue, bool allowDecimals)
        {
            string maskString = string.Empty;
            bool isNegative = minValue < 0;
            string allowNegativeString = isNegative ? @"-?" : string.Empty;
            string allowDecimalString = allowDecimals ? @"(\.\d{0,2})?" : string.Empty;
            if (maxValue == 0 && minValue == 0)
            {
                maskString = string.Format("^{0}{1}{2}$", allowNegativeString, @"\d{0,2}?", allowDecimalString);
            }
            else
            {
                int minValueLength = isNegative ? ((int)minValue).ToString().Length : ((int)minValue).ToString().Length + 1;
                int numericLength = Math.Max(minValueLength, ((int)maxValue).ToString().Length);
                maskString = string.Format("^{0}{1}{2}{3}{4}$", allowNegativeString, @"\d{0,", numericLength, @"}?", allowDecimalString);
            }
            return maskString;
        }

        private void CopyValuesToTextBoxes()
        {
            if (this.hoursTextBox != null)
                this.hoursTextBox.Text = this.Value.Hours.ToString("00");
            if (this.minutesTextBox != null)
                this.minutesTextBox.Text = this.Value.Minutes.ToString("00");
            if (this.secondsTextBox != null)
                this.secondsTextBox.Text = (this.Value.Seconds + (this.Value.Milliseconds / 1000.0d)).ToString("00.##");
        }

        private void RemoveFocusToTop()
        {
            FrameworkElement parent = this.Parent as FrameworkElement;
            while (parent != null && parent is IInputElement && !((IInputElement)parent).Focusable)
            {
                parent = (FrameworkElement)parent.Parent;
            }

            DependencyObject scope = FocusManager.GetFocusScope(this);
            FocusManager.SetFocusedElement(scope, parent as IInputElement);
        }
        #endregion
    }
}
