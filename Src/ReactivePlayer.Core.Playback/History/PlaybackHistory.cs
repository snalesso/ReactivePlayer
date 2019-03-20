﻿using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;
using DynamicData.Cache;
using DynamicData.Experimental;
using DynamicData.Kernel;
using DynamicData.List;
using DynamicData.Operators;
using DynamicData.PLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback.History
{
    public class PlaybackHistory : IDisposable
    {
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;

        public PlaybackHistory(IAudioPlaybackEngine audioPlaybackEngine)
        {
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));

            this.Entries = this._audioPlaybackEngine.WhenTrackChanged
                .SkipLast(1)
                .Where(t => t!= null)
                .ToObservableChangeSet(3)
                //.Replay(3)
                //.Select(track => new PlaybackHistoryEntry(track))
                .Transform(track => new PlaybackHistoryEntry(track))
                .DisposeMany()
                .AsObservableList()
                .DisposeWith(this._disposables)
                ;

            //this.Entries.CountChanged.Subscribe(c => Debug.WriteLine("items: " + c));
        }

        public IObservableList<PlaybackHistoryEntry> Entries { get; }

        private object _disposingLock = new object();
        private CompositeDisposable _disposables = new CompositeDisposable();

        public void Dispose()
        {
            // TODO: try-catch inside or outside lock?
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
                throw ex;
            }
        }
    }
}