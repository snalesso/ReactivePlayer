using ReactivePlayer.UI.WPF.ViewModels;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ReactivePlayer.UI.WPF.Views
{
    /// <summary>
    /// Interaction logic for PlaybackTimelineView.xaml
    /// </summary>
    public partial class PlaybackTimelineView : UserControl, IViewFor<PlaybackTimelineViewModel>, IDisposable
    {
        public PlaybackTimelineView()
        {
            //this.Events().Initialized.Take(1).Subscribe(a => this.ConfigureTrackbar(this.PlaybackPositionSlider));

            this.InitializeComponent();
        }

        #region IViewFor

        private PlaybackTimelineViewModel _viewModel;
        public PlaybackTimelineViewModel ViewModel
        {
            get => this._viewModel;
            set => this._viewModel = value ?? throw new ArgumentNullException(nameof(value)); // TODO: localize
        }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as PlaybackTimelineViewModel);
        }

        #endregion

        #region time slider

        private IConnectableObservable<long> _dragDeltaSubscriber;
        private IConnectableObservable<long> _dragCompletedSubscriber;
        private IDisposable _dragDeltaSubscription;
        private IDisposable _dragCompletedSubscription;

        private void ConfigureTrackbar(Slider slider)
        {
            var a = slider.Template.FindName("Track", slider);
            var b = slider.Template.FindName("PART_Track", slider);
            var thumb = (a as Track)?.Thumb;
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
                    .InvokeCommand(this.ViewModel, vm => vm.SeekTo)
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
                    .InvokeCommand(this.ViewModel, vm => vm.SeekTo)
                    .DisposeWith(this._disposables);
                this._dragCompletedSubscriber
                    //.Do(_ => Debug.WriteLine($"Drag\tCompleted\tvm.EndSeeking"))
                    //.Select(_ => Unit.Default)
                    .InvokeCommand(this.ViewModel, vm => vm.EndSeeking)
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
                    //.Select(_ => Unit.Default)
                    .InvokeCommand(this.ViewModel, vm => vm.StartSeeking)
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

        #endregion

        #region IDisposable Support

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                this.disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}