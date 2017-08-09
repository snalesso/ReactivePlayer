using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ReactivePlayer.UI.WPF.Application.Views
{
    public abstract class ReactiveUserControlViewFor<TViewModel> : UserControl, IViewFor<TViewModel>, IDisposable
        where TViewModel : class
    {
        private readonly CompositeDisposable _disposables;
        //private readonly ObservableAsPropertyHelper<object> WhenDataContextChanged

        public ReactiveUserControlViewFor()
        {
            this._viewModelSubject = new BehaviorSubject<TViewModel>(this.DataContext as TViewModel);
            // when .DataContext changes => update .ViewModel
            this.Events().DataContextChanged
                .Subscribe(dc => this._viewModelSubject.OnNext(dc.NewValue as TViewModel))
                .DisposeWith(this._disposables);
            this.WhenViewModelChanged = this._viewModelSubject.AsObservable().DistinctUntilChanged();
        }

        #region IViewFor

        private readonly BehaviorSubject<TViewModel> _viewModelSubject;
        public TViewModel ViewModel
        {
            get => this._viewModelSubject.Value;
            set => this.DataContext = value; // ?? throw new ArgumentNullException(nameof(value))); // TODO: localize
        }
        public IObservable<TViewModel> WhenViewModelChanged { get; }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as TViewModel);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}