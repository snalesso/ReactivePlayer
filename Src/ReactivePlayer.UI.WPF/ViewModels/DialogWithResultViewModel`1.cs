using Caliburn.Micro.ReactiveUI;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class DialogWithResultViewModel<T> : ReactiveScreen
    {
        #region constants & fields
        #endregion

        #region ctors

        public DialogWithResultViewModel(T initialResultValue)
        {
            this.Result = initialResultValue;
        }

        #endregion

        #region properties

        public bool IsConfirmed { get; } = false;

        public virtual T Result { get; }

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion
    }
}