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
using System.Reactive.Subjects;

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
        
        public MainWindow()
        {
            InitializeComponent();

            this._disposables = new CompositeDisposable();

            this.DataContext = (this._viewModel = new MainWindowViewModel());

            this.Loaded += this.MainWindow_Loaded;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            (this.DataContext as MainWindowViewModel)?.Close();
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.HookSliderTimeHandler(this.PlaybackPositionSlider);
        }

        private IConnectableObservable<long> _dragDeltaSubscriber;
        private IConnectableObservable<long> _dragCompletedSubscriber;
        private IDisposable _dragDeltaSubscription;
        private IDisposable _dragCompletedSubscription;

        private void HookSliderTimeHandler(Slider slider)
        {
            var thumb = (slider.Template.FindName("PART_Track", slider) as Track)?.Thumb;
            if (thumb != null)
            {
                this._dragDeltaSubscriber = Observable
                    .FromEventPattern<DragDeltaEventHandler, DragDeltaEventArgs>(
                    h => thumb.DragDelta += h,
                    h => thumb.DragDelta -= h)
                    .Select(_ => Convert.ToInt64(slider.Value))
                    .DistinctUntilChanged()
                    .Throttle(TimeSpan.FromMilliseconds(100))
                    //.Do(ticks => Debug.WriteLine($"Drag\tDelta\tThrottled value {ticks}"))
                    .Publish();
                this._dragDeltaSubscriber
                    .Do(ticks => Debug.WriteLine($"Drag\tDelta\tSeekTo({ticks})"))
                    .InvokeCommand(this._viewModel, vm => vm.SeekTo)
                    .DisposeWith(this._disposables);

                this._dragCompletedSubscriber = Observable
                    .FromEventPattern<DragCompletedEventHandler, DragCompletedEventArgs>(
                    h => thumb.DragCompleted += h,
                    h => thumb.DragCompleted -= h)
                    .Select(_ => Convert.ToInt64(slider.Value))
                    .DistinctUntilChanged()
                    .Take(1)
                    .Publish();
                this._dragCompletedSubscriber
                    //.Do(_ => Debug.WriteLine($"Drag\tCompleted\tUnsubscribing from DragDelta (to ignore throttled value after DragCompleted value)"))
                    .Subscribe(_ => this._dragDeltaSubscription?.Dispose())
                    .DisposeWith(this._disposables);
                this._dragCompletedSubscriber
                    .Do(ticks => Debug.WriteLine($"Drag\tCompleted\tSeekTo({ticks}"))
                    .InvokeCommand(this._viewModel, vm => vm.SeekTo)
                    .DisposeWith(this._disposables);
                this._dragCompletedSubscriber
                    //.Do(_ => Debug.WriteLine($"Drag\tCompleted\tvm.EndSeeking"))
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this._viewModel, vm => vm.EndSeeking)
                    .DisposeWith(this._disposables);
                this._dragCompletedSubscriber
                    //.Do(_ => Debug.WriteLine($"Drag\tCompleted\tSetting to null DragDelta subscription"))
                    .Subscribe(_ => this._dragDeltaSubscription = null)
                    .DisposeWith(this._disposables);
                this._dragCompletedSubscriber
                    //.Do(_ => Debug.WriteLine($"Drag\tCompleted\tSetting to null DragCompleted subscription"))
                    .Subscribe(_ => this._dragCompletedSubscription = null)
                    .DisposeWith(this._disposables);

                var start = Observable
                    .FromEventPattern<DragStartedEventHandler, DragStartedEventArgs>(
                    h => thumb.DragStarted += h,
                    h => thumb.DragStarted -= h)
                    .Select(_ => Convert.ToInt64(slider.Value))
                    .DistinctUntilChanged();
                start
                    //.Do(_ => Debug.WriteLine($"Drag\tStarted\tvm.StartSeeking"))
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this._viewModel, vm => vm.StartSeeking)
                    .DisposeWith(this._disposables);
                start
                    //.Do(_ => Debug.WriteLine($"Drag\tStarted\tSubscribing to DragDelta"))
                    .Subscribe(_ => this._dragDeltaSubscription = this._dragDeltaSubscriber.Connect().DisposeWith(this._disposables))
                    .DisposeWith(this._disposables);
                start
                    //.Do(l => Debug.WriteLine($"Drag\tStarted\tSubscribing to DragCompleted"))
                    .Subscribe(_ => this._dragCompletedSubscription = this._dragCompletedSubscriber.Connect().DisposeWith(this._disposables))
                    .DisposeWith(this._disposables);
            }
        }
    }
}