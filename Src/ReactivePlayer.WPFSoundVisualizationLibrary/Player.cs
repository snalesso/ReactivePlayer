using ReactivePlayer.Core.Playback;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using WPFSoundVisualizationLib;

namespace ReactivePlayer.WPFSoundVisualizationLibrary
{
    public class Player : ReactiveObject, ISpectrumPlayer
    {
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;

        public Player(IAudioPlaybackEngine audioPlaybackEngine)
        {
            this._audioPlaybackEngine = audioPlaybackEngine;

            this._isPlayingOAPH = this._audioPlaybackEngine
                .WhenStatusChanged
                .Select(status => status == PlaybackStatus.Playing)
                .ToProperty(this, nameof(this.IsPlaying));
        }

        private ObservableAsPropertyHelper<bool> _isPlayingOAPH;
        public bool IsPlaying => this._isPlayingOAPH.Value;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool GetFFTData(float[] fftDataBuffer)
        {
            throw new NotImplementedException();
        }

        public int GetFFTFrequencyIndex(int frequency)
        {
            throw new NotImplementedException();
        }
    }
}