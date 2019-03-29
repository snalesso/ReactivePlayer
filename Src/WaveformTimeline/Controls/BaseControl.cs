using System;
using System.Windows;
using System.Windows.Controls;
using WaveformTimeline.Commons;
using WaveformTimeline.Contracts;
using WaveformTimeline.Primitives;

namespace WaveformTimeline.Controls
{
    public abstract class BaseControl : Control
    {
        protected Canvas MainCanvas
        {
            get => this._mainCanvas;
            set { if (value != null) this._mainCanvas = value; }
        }

        protected TuneDuration _coverageArea;
        protected WaveformDimensions _waveformDimensions;
        private Canvas _mainCanvas = new Canvas();

        public static readonly DependencyProperty TuneProperty = DependencyProperty.Register(
                nameof(Tune),
                typeof(ITune),
                typeof(BaseControl),
                new UIPropertyMetadata(new NoTune(), OnTuneChanged));

        private static void OnTuneChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as BaseControl)?.OnTuneChanged();
        }

        protected abstract void OnTuneChanged();

        public ITune Tune
        {
            get => (ITune)this.GetValue(TuneProperty) ?? new NoTune(); // no null acceptable
            set => this.SetValue(TuneProperty, value ?? new NoTune());
        }

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
            nameof(Zoom),
            typeof(double),
            typeof(BaseControl),
            new PropertyMetadata(1.0d, OnZoomChanged, OnCoerceZoom));

        private static void OnZoomChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as BaseControl)?.Render();
        }

        private static object OnCoerceZoom(DependencyObject o, object value)
        {
            return Math.Max(1.0, new FiniteDouble((double)value, 1.0).Value());
        }

        public double Zoom
        {
            get => (double)this.GetValue(ZoomProperty);
            set => this.SetValue(ZoomProperty, value);
        }

        protected virtual void MeasureArea()
        {
            this._coverageArea = new TuneDuration(this.Tune, this.Zoom);
            this._waveformDimensions = new WaveformDimensions(this._coverageArea, this.MainCanvas.RenderSize.Width);
        }

        protected abstract void Render();
    }
}