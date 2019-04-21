using Caliburn.Micro.ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class AddAlbumViewModel : ReactiveScreen//, IDataErrorInfo, INotifyDataErrorInfo
    {
        #region ctor

        public AddAlbumViewModel()
        {
        }

        #endregion

        #region properties

        private string _title;
        public string Title
        {
            get { return this._title; }
            set { this.SetAndRaiseIfChanged(ref this._title, value); }
        }

        private IReadOnlyList<string> _authors;
        public IReadOnlyList<string> Authors
        {
            get { return this._authors; }
            set { this.SetAndRaiseIfChanged(ref this._authors, value); }
        }

        private uint? _tracksCount;
        public uint? TracksCount
        {
            get { return this._tracksCount; }
            set { this.SetAndRaiseIfChanged(ref this._tracksCount, value); }
        }

        private uint? _discsCount;
        public uint? DiscsCount
        {
            get { return this._discsCount; }
            set { this.SetAndRaiseIfChanged(ref this._discsCount, value); }
        }

        #endregion

        //#region IDataErrorInfo

        //public string Error => throw new NotImplementedException();

        //public bool HasErrors => throw new NotImplementedException();

        //public string this[string columnName] => throw new NotImplementedException();

        //public IEnumerable GetErrors(string propertyName)
        //{
        //    throw new NotImplementedException();
        //}

        //#endregion

        //#region INotifyDataErrorInfo

        //public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        //#endregion
    }
}