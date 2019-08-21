using Caliburn.Micro.ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind
{
    public class TrackViewModel : ReactiveScreen, IDisposable
    {
        private readonly Track _track;

        public TrackViewModel(Track track)
        {
            this._track = track ?? throw new ArgumentNullException(nameof(track));
        }

        public uint Id => this._track.Id;
        public string Location  => this._track.Location;
        public string Title => this._track.Title;
        public DateTime AddedToLibraryDateTime => this._track.AddedToLibraryDateTime;

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool isDisposing)
        {
            if (!this._isDisposed)
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

        public void Dispose()
        {
            this.Dispose(true);
        }

        #endregion
    }
}