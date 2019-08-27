using DynamicData;
using System;
using System.Reactive.Disposables;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind
{
    public class AllTracksViewModel : TracksSubsetViewModel, IDisposable
    {
        public AllTracksViewModel(IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChanges)
            : base(sourceTrackViewModelsChanges)
        {
        }

        public override string Name => "All tracks";

        protected override IObservable<IChangeSet<TrackViewModel, uint>> Filter(IObservable<IChangeSet<TrackViewModel, uint>> trackViewModelsChanges)
        {
            return trackViewModelsChanges;
        }

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        //protected virtual void Dispose(bool isDisposing)
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

            // remove in non-derived class
            base.Dispose(isDisposing);
        }

        #endregion
    }
}