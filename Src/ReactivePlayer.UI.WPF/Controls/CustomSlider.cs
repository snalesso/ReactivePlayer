using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ReactivePlayer.UI.Wpf.Controls
{
    public abstract class CustomSlider : Control //RangeBase
    {
        protected Canvas sliderCanvas;
        protected Rectangle sliderTrack;
        protected Rectangle sliderBar;
        protected Button sliderButton;
        protected bool isCalculating;
        protected bool isDragging;

        public bool ChangeValueWhileDragging
        {
            get => Convert.ToBoolean(this.GetValue(ChangeValueWhileDraggingProperty));
            set => this.SetValue(ChangeValueWhileDraggingProperty, value);
        }

        public static readonly DependencyProperty ChangeValueWhileDraggingProperty = DependencyProperty.Register(
            nameof(ChangeValueWhileDragging),
            typeof(bool),
            typeof(CustomSlider),
            new PropertyMetadata(false));

        public double Minimum
        {
            get => Convert.ToDouble(this.GetValue(MinimumProperty));
            set => this.SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            nameof(Minimum),
            typeof(double),
            typeof(CustomSlider),
            new PropertyMetadata(0.0, OnMinMaxChanged));

        public double Maximum
        {
            get => Convert.ToDouble(this.GetValue(MaximumProperty));
            set => this.SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            nameof(Maximum),
            typeof(double),
            typeof(CustomSlider),
            new PropertyMetadata(100.0, OnMinMaxChanged));

        public double Value
        {
            get => Convert.ToDouble(this.GetValue(ValueProperty));
            set => this.SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(CustomSlider),
            new PropertyMetadata(0.0, OnValueChanged));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public double Position
        {
            get => Convert.ToDouble(this.GetValue(PositionProperty));
            set => this.SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            nameof(Position),
            typeof(double),
            typeof(CustomSlider),
            new PropertyMetadata(0.0));

        public Brush TrackBackground
        {
            get => (Brush)this.GetValue(TrackBackgroundProperty);
            set => this.SetValue(TrackBackgroundProperty, value);
        }

        public static readonly DependencyProperty TrackBackgroundProperty = DependencyProperty.Register(
            nameof(TrackBackground),
            typeof(Brush),
            typeof(CustomSlider),
            new PropertyMetadata(null));

        public Brush BarBackground
        {
            get => (Brush)this.GetValue(BarBackgroundProperty);
            set => this.SetValue(BarBackgroundProperty, value);
        }

        public static readonly DependencyProperty BarBackgroundProperty = DependencyProperty.Register(
            nameof(BarBackground),
            typeof(Brush),
            typeof(CustomSlider),
            new PropertyMetadata(null));

        public Brush ButtonBackground
        {
            get => (Brush)this.GetValue(ButtonBackgroundProperty);
            set => this.SetValue(ButtonBackgroundProperty, value);
        }

        public static readonly DependencyProperty ButtonBackgroundProperty = DependencyProperty.Register(
            nameof(ButtonBackground),
            typeof(Brush),
            typeof(CustomSlider),
            new PropertyMetadata(null));

        public event EventHandler ValueChanged = delegate { };

        protected void OnValueChanged()
        {
            this.ValueChanged(this, null);
        }

        private const string PART_Canvas = nameof(PART_Canvas);
        private const string PART_Track = nameof(PART_Track);
        private const string PART_Bar = nameof(PART_Bar);
        private const string PART_Button = nameof(PART_Button);

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.sliderCanvas = (Canvas)this.GetTemplateChild(CustomSlider.PART_Canvas);
            this.sliderTrack = (Rectangle)this.GetTemplateChild(CustomSlider.PART_Track);
            this.sliderBar = (Rectangle)this.GetTemplateChild(CustomSlider.PART_Bar);
            this.sliderButton = (Button)this.GetTemplateChild(CustomSlider.PART_Button);

            this.sliderCanvas.SizeChanged += this.SizeChangedHandler;
            this.sliderCanvas.Loaded += this.LoadedHandler;

            if (this.sliderButton != null)
            {
                this.sliderButton.PreviewMouseDown += this.SliderButton_PreviewMouseDown;
                this.sliderButton.PreviewMouseUp += this.SliderButton_PreviewMouseUp;
                this.sliderButton.PreviewMouseMove += this.SliderButton_PreviewMouseMove;
            }

            if (this.sliderCanvas != null)
            {
                this.sliderCanvas.PreviewMouseUp += this.SliderCanvas_PreviewMouseUp;
            }
        }

        protected abstract void UpdatePosition();
        protected abstract void CalculatePosition();
        protected abstract void CalculateValue();

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

        private static void OnMinMaxChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CustomSlider slider = sender as CustomSlider;
            slider.CalculatePosition();
        }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CustomSlider slider = sender as CustomSlider;
            slider.CalculatePosition();
            slider.OnValueChanged();
        }
    }
}