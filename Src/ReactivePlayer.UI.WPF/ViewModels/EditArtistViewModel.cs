using Caliburn.Micro.ReactiveUI;

namespace ReactivePlayer.UI.Wpf.ViewModels
{
    public class EditArtistViewModel : ReactiveScreen
    //, IEditableObject
    //, ISupportsValidation
    {
        #region constants & fields

        //private readonly Func<string, bool> _isUniqueCheckerFunc;

        #endregion

        #region ctors

        public EditArtistViewModel(
            //Func<string, bool> isUniqueCheckerFunc,
            string artistName)
        {
            //this._isUniqueCheckerFunc = isUniqueCheckerFunc;

            //this.WhenAnyValue(x => x.Value)
            //    .Subscribe(s =>
            //    {
            //        this.UpdateErrorForProperty(nameof(this.Value), this._getStringValidationErrorMessage(s));
            //    })
            //    .DisposeWith(this._disposables);

            //this.ValidationRule(
            //    x => x.Value,
            //    x =>
            //    {
            //        return this._isUniqueCheckerFunc(x);
            //    },
            //    isValid => this._isUniqueCheckerFunc(isValid));

            this.ArtistName = artistName;
        }

        #endregion

        #region properties

        private string _artistName;
        public string ArtistName
        {
            get { return this._artistName; }
            set { this.Set(ref this._artistName, value); }
        }

        #endregion
    }
}