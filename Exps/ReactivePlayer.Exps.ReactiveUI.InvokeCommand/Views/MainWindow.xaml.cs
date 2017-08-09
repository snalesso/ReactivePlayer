using ReactivePlayer.Exps.ReactiveUI.InvokeCommand.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReactivePlayer.Exps.ReactiveUI.InvokeCommand.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainWindowViewModel>
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();

                this._viewModelSubject = new BehaviorSubject<MainWindowViewModel>(
                    this.DataContext as MainWindowViewModel
                    );
                this.WhenViewModelChanged = this._viewModelSubject.AsObservable().DistinctUntilChanged();

                this
                    .Events()
                    .DataContextChanged
                    .Select(dc => dc.NewValue as MainWindowViewModel)
                    .Do(vm => Debug.WriteLine($"{nameof(this.DataContextChanged)} {(vm == null ? "!=" : "==")} null"))
                    //.Where(vm => vm != null)
                    //.Do(vm => Debug.WriteLine($"{nameof(this.DataContextChanged)} != null"))
                    .Subscribe(this._viewModelSubject.OnNext);

                this.Events()
                    .Loaded
                    .Take(1)
                    .Subscribe(ea =>
                    {
                        this.WhenViewModelChanged
                            .Do(vm => Debug.WriteLine($"{nameof(this.WhenViewModelChanged)} {(vm == null ? "!=" : "==")} null"))
                            .Where(vm => vm != null)
                            .Do(vm => Debug.WriteLine($"{nameof(this.WhenViewModelChanged)} != null"))
                            .Select(vm => Unit.Default)
                            .InvokeCommand(this, t => t.ViewModel.SetTitle);
                        //this.ViewModel.SetTitle.Execute();
                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                throw;
            }
        }

        #region IViewFor

        private readonly ISubject<MainWindowViewModel> _viewModelSubject;
        public MainWindowViewModel ViewModel
        {
            get => this.DataContext as MainWindowViewModel;
            set => this.DataContext = value;
        }

        public IObservable<MainWindowViewModel> WhenViewModelChanged { get; }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as MainWindowViewModel);
        }

        #endregion
    }
}