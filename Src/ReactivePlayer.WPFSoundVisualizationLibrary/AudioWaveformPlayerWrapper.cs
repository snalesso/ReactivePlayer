using ReactivePlayer.Core.Playback;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFSoundVisualizationLib;

namespace ReactivePlayer.WPFSoundVisualizationLibrary
{
    public class AudioWaveformPlayerWrapper : ReactiveObject, IWaveformPlayer
    {
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;

        public AudioWaveformPlayerWrapper(IAudioPlaybackEngine audioPlaybackEngine)
        {
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));

            this._isPlayingOAPH = this._audioPlaybackEngine.WhenStatusChanged
                .Select(status => status == PlaybackStatus.Playing)
                .ToProperty(this, nameof(this.IsPlaying));
        }

        private double _channelPosition;
        public double ChannelPosition
        {
            get => this._channelPosition;
            set
            {
                if (!this.inChannelSet)
                {
                    this.inChannelSet = true; // Avoid recursion
                    double oldValue = this._channelPosition;
                    double position = Math.Max(0, Math.Min(value, this.ChannelLength));
                    if (!this.inChannelTimerUpdate && this.ActiveStream != null)
                        this.ActiveStream.Position = (long)((position / this.ActiveStream.TotalTime.TotalSeconds) * this.ActiveStream.Length);
                    this._channelPosition = position;
                    if (oldValue != this._channelPosition)
                        this.NotifyPropertyChanged("ChannelPosition");
                    this.inChannelSet = false;
                }
            }
        }

        public double ChannelLength => throw new NotImplementedException();

        public float[] WaveformData => throw new NotImplementedException();

        public TimeSpan SelectionBegin
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public TimeSpan SelectionEnd
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        private readonly ObservableAsPropertyHelper<bool> _isPlayingOAPH;
        public bool IsPlaying => throw new NotImplementedException();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}