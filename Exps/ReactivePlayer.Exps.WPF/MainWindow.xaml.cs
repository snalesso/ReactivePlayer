using ReactivePlayer.Exps.WPF.ViewModels;
using System.Windows;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using System.Reactive.Disposables;

namespace ReactivePlayer.Exps.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;
        private const bool IsLoggingEnabled = false;

        private CompositeDisposable _disposables;

        private readonly IObservable<EventPattern<DragStartedEventArgs>> _whenSliderPositionDragStarted;
        private readonly IObservable<EventPattern<DragDeltaEventArgs>> _whenSliderPositionDragDelta;
        private readonly IObservable<EventPattern<DragCompletedEventArgs>> _whenSliderPositionDragCompleted;

        public MainWindow()
        {
            InitializeComponent();

            this._disposables = new CompositeDisposable();

            this.DataContext = (this._viewModel = new MainWindowViewModel());
            
            var thumb = (this.PlaybackPositionSlider.Template.FindName("PART_Track", this.PlaybackPositionSlider) as Track)?.Thumb;
            if (thumb != null)
            {
                this._whenSliderPositionDragStarted = Observable.FromEventPattern<DragStartedEventHandler, DragStartedEventArgs>(
                    h => thumb.DragStarted += h,
                    h => thumb.DragStarted -= h);
                this._whenSliderPositionDragStarted.InvokeCommand(this._viewModel.StartSeeking).DisposeWith(this._disposables);

                this._whenSliderPositionDragDelta = Observable.FromEventPattern<DragDeltaEventHandler, DragDeltaEventArgs>(
                    h => thumb.DragDelta += h,
                    h => thumb.DragDelta -= h);
                this._whenSliderPositionDragDelta.Select(d => this.PlaybackPositionSlider.Value).InvokeCommand(this._viewModel.SeekTo).DisposeWith(this._disposables);

                this._whenSliderPositionDragCompleted = Observable.FromEventPattern<DragCompletedEventHandler, DragCompletedEventArgs>(
                    h => thumb.DragCompleted += h,
                    h => thumb.DragCompleted -= h);
                this._whenSliderPositionDragCompleted.InvokeCommand(this._viewModel.EndSeeking).DisposeWith(this._disposables);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            (this.DataContext as MainWindowViewModel)?.Close();
        }

        private void PositionSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (MainWindow.IsLoggingEnabled)
                Debug.WriteLine($"{nameof(Thumb)}.{nameof(Thumb.DragStarted)}\t=\t{e.Source}");
        }

        private void PositionSlider_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (MainWindow.IsLoggingEnabled)
                Debug.WriteLine($"{nameof(Thumb)}.{nameof(Thumb.DragDelta)}\t=\t{e.Source}");
        }

        private void PositionSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (MainWindow.IsLoggingEnabled)
                Debug.WriteLine($"{nameof(Thumb)}.{nameof(Thumb.DragCompleted)}\t=\t{e.Source}");
        }
    }
}