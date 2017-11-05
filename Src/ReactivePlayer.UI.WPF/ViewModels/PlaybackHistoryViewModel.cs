using DynamicData;
using DynamicData.List;
using DynamicData.Kernel;
using DynamicData.Operators;
using DynamicData.ReactiveUI;
using DynamicData.Aggregation;
using DynamicData.Binding;
using ReactivePlayer.Core.Application.Library;
using ReactivePlayer.Core.Application.Playback;
using ReactivePlayer.UI.WPF.ReactiveCaliburnMicro;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class PlaybackHistoryViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly PlaybackQueue _playbackQueue;
        private readonly IReadLibraryService _readLibraryService;

        #endregion

        #region constructors

        public PlaybackHistoryViewModel(
            PlaybackQueue playbackQueue,
            IReadLibraryService readLibraryService)
        {
            this._playbackQueue.Items.Connect().Transform(location => new PlaybackHistoryItemViewModel(location)).Bind(this._items);
        }

        #endregion

        #region properties

        private ReactiveList<PlaybackHistoryItemViewModel> _items = new ReactiveList<PlaybackHistoryItemViewModel>();
        public IReadOnlyReactiveList<PlaybackHistoryItemViewModel> Items => this._items;

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion

        #region IDisposable

        private CompositeDisposable _disposables = new CompositeDisposable();
        private object _disposingLock = new object();

        // TODO: review implementation, also consider if there's some Interlocked way to do it
        public void Dispose()
        {
            try
            {
                lock (this._disposingLock)
                {
                    if (this._disposables != null && !this._disposables.IsDisposed)
                    {
                        this._disposables?.Dispose();
                        this._disposables = null;
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: log
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(Dispose)}: {ex.Message}");
                throw ex;
            }
        }

        #endregion
    }
}