using ReactivePlayer.UI.WPF.ViewModels;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;

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

        private IDisposable _addNewCommandSubscription = null;

        #endregion

        #region ctor

        public EditArtistsView()
        {
            this.WhenViewModelChanged = this.Events().DataContextChanged
                .Select(dc => (OldValue: (EditArtistsViewModel)dc.OldValue, NewaValue: (EditArtistsViewModel)dc.NewValue))
                .DistinctUntilChanged();

            var when_ViewModelChanged_And_ViewLoaded =
                this.WhenViewModelChanged
                .And(this.Events().Loaded)
                .Then((viewModelChangedEventArgs, viewLoadedEventArgs) => viewModelChangedEventArgs);

            //this.Events().DataContextChanged.Do(x => Debug.WriteLine("dc changed " + (x.NewValue != null))).Subscribe();

            Observable
                .When(when_ViewModelChanged_And_ViewLoaded)
                .Subscribe(vmcea =>
                {
                    if (vmcea.OldViewModel != null)
                    {
                        this._addNewCommandSubscription?.Dispose();
                    }

                    if (vmcea.NewViewModel != null)
                    {
                        this._addNewCommandSubscription = vmcea.NewViewModel.AddNew.Subscribe(newArtistVM => this.txbArtistName.Focus()).DisposeWith(this._disposables);
                    }
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

        public IObservable<(EditArtistsViewModel OldViewModel, EditArtistsViewModel NewViewModel)> WhenViewModelChanged { get; }

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