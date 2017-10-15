using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ReactivePlayer.UI.WPF.Controls
{
    public abstract class SliderBase : Control
    {
        #region Variables

        protected Canvas sliderCanvas;
        protected Rectangle sliderTrack;
        protected Rectangle sliderBar;
        protected Button sliderButton;
        protected bool isCalculating;
        protected bool isDragging;

        #endregion

        #region Properties

        public static readonly DependencyProperty ChangeValueWhileDraggingProperty = DependencyProperty.Register("ChangeValueWhileDragging", typeof(bool), typeof(SliderBase), new PropertyMetadata(false));
        public bool ChangeValueWhileDragging
        {
            get { return Convert.ToBoolean(GetValue(ChangeValueWhileDraggingProperty)); }
            set { SetValue(ChangeValueWhileDraggingProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(SliderBase), new PropertyMetadata(0.0, MinMaxChangedCallback));
        public double Minimum
        {
            get { return Convert.ToDouble(GetValue(MinimumProperty)); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(SliderBase), new PropertyMetadata(100.0, MinMaxChangedCallback));
        public double Maximum
        {
            get { return Convert.ToDouble(GetValue(MaximumProperty)); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(SliderBase), new PropertyMetadata(0.0, ValueChangedCallback));
        public double Value
        {
            get { return Convert.ToDouble(GetValue(ValueProperty)); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(double), typeof(SliderBase), new PropertyMetadata(0.0));
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double Position
        {
            get { return Convert.ToDouble(GetValue(PositionProperty)); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty TrackBackgroundProperty = DependencyProperty.Register("TrackBackground", typeof(Brush), typeof(SliderBase), new PropertyMetadata(null));
        public Brush TrackBackground
        {
            get { return (Brush)GetValue(TrackBackgroundProperty); }
            set { SetValue(TrackBackgroundProperty, value); }
        }

        public static readonly DependencyProperty BarBackgroundProperty = DependencyProperty.Register("BarBackground", typeof(Brush), typeof(SliderBase), new PropertyMetadata(null));
        public Brush BarBackground
        {
            get { return (Brush)GetValue(BarBackgroundProperty); }
            set { SetValue(BarBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ButtonBackgroundProperty = DependencyProperty.Register("ButtonBackground", typeof(Brush), typeof(SliderBase), new PropertyMetadata(null));
        public Brush ButtonBackground
        {
            get { return (Brush)GetValue(ButtonBackgroundProperty); }
            set { SetValue(ButtonBackgroundProperty, value); }
        }

        #endregion

        #region Events

        public event EventHandler ValueChanged = delegate { };

        protected void OnValueChanged()
        {
            this.ValueChanged(this, null);
        }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.sliderCanvas = (Canvas)GetTemplateChild("PART_Canvas");
            this.sliderTrack = (Rectangle)GetTemplateChild("PART_Track");
            this.sliderBar = (Rectangle)GetTemplateChild("PART_Bar");
            this.sliderButton = (Button)GetTemplateChild("PART_Button");

            this.sliderCanvas.SizeChanged += SizeChangedHandler;
            this.sliderCanvas.Loaded += LoadedHandler;

            if (this.sliderButton != null)
            {
                this.sliderButton.PreviewMouseDown += SliderButton_PreviewMouseDown;
                this.sliderButton.PreviewMouseUp += SliderButton_PreviewMouseUp;
                this.sliderButton.PreviewMouseMove += SliderButton_PreviewMouseMove;
            }

            if (this.sliderCanvas != null)
            {
                this.sliderCanvas.PreviewMouseUp += SliderCanvas_PreviewMouseUp;
            }
        }

        #endregion

        #region Abstract

        protected abstract void UpdatePosition();
        protected abstract void CalculatePosition();
        protected abstract void CalculateValue();

        #endregion

        #region Event Handlers

        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
            this.CalculatePosition();
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            this.CalculatePosition();
        }

        private void SliderButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.isDragging = true;
        }

        private void SliderButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.isDragging = false;
            this.CalculateValue();
        }

        private void SliderButton_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (this.isDragging)
            {
                this.UpdatePosition();

                if (this.ChangeValueWhileDragging)
                {
                    this.CalculateValue();
                }
            }
        }

        private void SliderCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!this.isDragging)
            {
                this.UpdatePosition();
                this.CalculateValue();
            }
        }

        #endregion

        #region Callbacks

        private static void MinMaxChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SliderBase slider = sender as SliderBase;
            slider.CalculatePosition();
        }

        private static void ValueChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SliderBase slider = sender as SliderBase;
            slider.CalculatePosition();
            slider.OnValueChanged();
        }

        #endregion
    }
}