using ReactivePlayer.UI.WPF.ViewModels;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
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
            var when_DataContextChanged_And_ViewLoaded =
                this.Events().DataContextChanged
                .And(this.Events().Loaded)
                .Then((dataContextChangedEventArgs, loadedEventArgs) => dataContextChangedEventArgs);

            Observable
                .When(when_DataContextChanged_And_ViewLoaded)
                .Subscribe(dataContextChangedEventArgs =>
                {
                    if (dataContextChangedEventArgs.OldValue != null)
                    {
                        // remove event handler from old viewmodel
                    }

                    if (this.ViewModel != null)
                    {
                        this.AttachSeekingCommandsToSlider(this.PlaybackPositionSlider);
                    }
                })
                .DisposeWith(this._disposables);

            this.InitializeComponent();
        }

        #region IViewFor

        public PlaybackTimelineViewModel ViewModel
        {
            get => this.DataContext as PlaybackTimelineViewModel;
            set => this.DataContext = value ?? throw new ArgumentNullException(nameof(value));
        }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as PlaybackTimelineViewModel);
        }

        #endregion

        #region time slider

        // TODO: who disposes this shit? caliburn? autofac?
        private IConnectableObservable<Unit> _whenDragStartedSubscriber;
        //private IConnectableObservable<long> _whenDragDeltaSubscriber;
        private IConnectableObservable<long> _whenDragCompletedSubscriber;
        //private IDisposable _dragDeltaSubscription;
        //private IDisposable _dragCompletedSubscription;

        private void AttachSeekingCommandsToSlider(Slider slider)
        {
            Track sliderTrack = null;
            Thumb sliderThumb = null;

            sliderTrack = slider.Template.FindName("PART_Track", slider) as Track;
            sliderThumb = sliderTrack?.Thumb;

            if (sliderThumb == null)
                return;

            this._whenDragStartedSubscriber = Observable
                .FromEventPattern<DragStartedEventHandler, DragStartedEventArgs>(
                h => sliderThumb.DragStarted += h,
                h => sliderThumb.DragStarted -= h)
                //.Do(x => Debug.WriteLine("Drag started"))
                //.Select(_ => Convert.ToInt64(slider.Value))
                .Select(_ => Unit.Default)
                .Publish();

            //this._whenDragDeltaSubscriber = Observable
            //    .FromEventPattern<DragDeltaEventHandler, DragDeltaEventArgs>(
            //    h => sliderThumb.DragDelta += h,
            //    h => sliderThumb.DragDelta -= h)
            //    //.Do(x => Debug.WriteLine("Drag delta"))
            //    .Select(_ => Convert.ToInt64(slider.Value))
            //    .DistinctUntilChanged()
            //    .Throttle(TimeSpan.FromMilliseconds(250))
            //    .Publish();

            this._whenDragCompletedSubscriber = Observable
                .FromEventPattern<DragCompletedEventHandler, DragCompletedEventArgs>(
                h => sliderThumb.DragCompleted += h,
                h => sliderThumb.DragCompleted -= h)
                //.Do(x => Debug.WriteLine("Drag completed"))
                .Select(_ => Convert.ToInt64(slider.Value))
                //.Select(_ => Unit.Default)
                //.Take(1)
                .Publish();

            // ------------------

            //this._whenDragStartedSubscriber
            //    .Subscribe(_ => this._dragDeltaSubscription = this._whenDragDeltaSubscriber.Connect().DisposeWith(this._disposables))
            //    .DisposeWith(this._disposables);
            //this._whenDragStartedSubscriber
            //    .Subscribe(_ => this._dragCompletedSubscription = this._whenDragCompletedSubscriber.Connect().DisposeWith(this._disposables))
            //    .DisposeWith(this._disposables);

            // --------------

            this._whenDragStartedSubscriber
                .InvokeCommand(this.ViewModel, vm => vm.StartSeeking)
                .DisposeWith(this._disposables);

            //this._whenDragDeltaSubscriber
            //   .InvokeCommand(this.ViewModel, vm => vm.SeekTo)
            //   .DisposeWith(this._disposables);

            //this._whenDragCompletedSubscriber
            //    .InvokeCommand(this.ViewModel, vm => vm.SeekTo)
            //    .DisposeWith(this._disposables);

            this._whenDragCompletedSubscriber
                .InvokeCommand(this.ViewModel, vm => vm.EndSeeking)
                .DisposeWith(this._disposables);

            this._whenDragStartedSubscriber.Connect();
            //this._whenDragDeltaSubscriber.Connect();
            this._whenDragCompletedSubscriber.Connect();

            //this._whenDragCompletedSubscriber
            //    .Subscribe(_ => this._dragDeltaSubscription = null)
            //    .DisposeWith(this._disposables);

            //this._whenDragCompletedSubscriber
            //    .Subscribe(_ =>
            //    {
            //        this._dragDeltaSubscription?.Dispose();
            //        this._dragCompletedSubscription = null;
            //    })
            //    .DisposeWith(this._disposables);            
        }

        #endregion

        #region waveform timeline

        //private void AttachPlaybackServiceToTimeline()
        //{
        //    if (this.WaveformTimeline == null)
        //        return;

        //    this.WaveformTimeline.RegisterSoundPlayer(this._waveformTimelineSoundPlayer);
        //}

        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}