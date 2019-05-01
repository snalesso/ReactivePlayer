using ReactivePlayer.UI.WPF.ViewModels;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReactivePlayer.UI.WPF.Views
{
    //// https://github.com/reactiveui/ReactiveUI/blob/master/src/ReactiveUI/Platforms/windows-common/ReactiveUserControl.cs
    //public abstract class EditArtistsViewBase : ReactiveUserControl<EditArtistsViewModel> { }

    /// <summary>
    /// Interaction logic for EditArtistsView.xaml
    /// </summary>
    public partial class EditArtistsView : UserControl, IViewFor<EditArtistsViewModel>, IDisposable
    {
        #region constants & fields

        private readonly CompositeDisposable _viewModelSubscriptionsDisposables;
        //private IDisposable _viewModel_AddNew_Subscription;
        //private IDisposable _artistNameTextBox_KeyPressed_Esc_Subscription;
        //private IDisposable _artistNameTextBox_KeyPressed_Enter_Subscription;

        #endregion

        #region ctor

        public EditArtistsView()
        {
            this._viewModelSubscriptionsDisposables = new CompositeDisposable().DisposeWith(this._disposables);

            this.WhenViewModelChanged = this.Events().DataContextChanged
                .Select(dc => (EditArtistsViewModel)dc.NewValue)
                .DistinctUntilChanged();

            var when_ViewLoaded_And_ViewModelChanged =
                this.Events().Loaded
                .And(this.WhenViewModelChanged)
                .Then((viewLoadedEventArgs, newViewModel) => newViewModel);

            //this.Events().DataContextChanged.Do(x => Debug.WriteLine("dc changed " + (x.NewValue != null))).Subscribe();

            Observable
                .When(when_ViewLoaded_And_ViewModelChanged)
                .Subscribe(newViewModel =>
                {
                    this._viewModelSubscriptionsDisposables?.Clear();

                    if (newViewModel == null)
                        return;

                    // when the viewmodel executes the add, focus the txtbox again
                    newViewModel.AddNew.Subscribe(_ => this.txbNewArtistName.Focus()).DisposeWith(this._viewModelSubscriptionsDisposables);

                    // when txt new artist name receives enter, execute the add
                     Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(
                        h => this.txbNewArtistName.KeyDown += h,
                        h => this.txbNewArtistName.KeyDown -= h)
                        .Where(key => key.EventArgs.Key == Key.Enter)
                        .Select(_ => Unit.Default)
                        .InvokeCommand(newViewModel, x => x.AddNew)
                        .DisposeWith(this._viewModelSubscriptionsDisposables);
                })
                .DisposeWith(this._disposables);

            this.InitializeComponent();
        }

        #endregion

        #region IViewFor

        /// <summary>
        /// The view model dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(EditArtistsViewModel),
                typeof(EditArtistsView),
                new PropertyMetadata(null));

        public EditArtistsViewModel ViewModel
        {
            get => (EditArtistsViewModel)this.DataContext;
            set => this.DataContext = value;
        }

        public IObservable<EditArtistsViewModel> WhenViewModelChanged { get; }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (EditArtistsViewModel)value;
        }

        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                this._isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}