using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.UI.Collections;
using DynamicData;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ReactivePlayer.UI.WPF.ViewModels
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
            set { this.RaiseAndSetIfChanged(ref this._artistName, value); }
        }

        #endregion
    }
}