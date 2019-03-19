using CSCore.Codecs;
using CSCore.SoundOut;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.Core.Playback.CSCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCore.SimpleControlsSync
{
    public class ShellViewModel : ReactiveObject, IDisposable
    {
        private const string DefaultSongFilePath = @"D:\Music\Productions\300 Hz - 1.5 s.mp3";

        private CompositeDisposable _disposables = new CompositeDisposable();
        //private ISoundOut _soundOut;
        //CSCorePlayer _csCorePlayer = new CSCorePlayer();
        CSCoreAudioPlaybackEngine _audioPlaybackEngineAsync = new CSCoreAudioPlaybackEngine();

        public ShellViewModel()
        {
            //this.DoNothing = ReactiveCommand.Create(() => { }).DisposeWith(this._disposables);

            this.Load = ReactiveCommand.CreateFromTask(
                (Uri location) =>
                {
                    return this._audioPlaybackEngineAsync.LoadAsync(location);
                }
                , Observable.CombineLatest(
                    this._audioPlaybackEngineAsync.WhenCanLoadChanged,
                    this.WhenAny(x => x.SelectedTrack, x => x.Value != null), //.StartWith(this.SelectedTrack != null),
                    (canLoad, isTrackSelected) => canLoad && isTrackSelected)
                )
                .DisposeWith(this._disposables);
            this.PlayFile = ReactiveCommand.CreateFromTask(
               async (Uri location) =>
                {
                    await this._audioPlaybackEngineAsync.StopAsync();
                    await this._audioPlaybackEngineAsync.LoadAndPlayAsync(location);
                }
                , Observable.CombineLatest(
                    this._audioPlaybackEngineAsync.WhenCanLoadChanged,
                    this._audioPlaybackEngineAsync.WhenCanPlayChanged,
                    this.WhenAny(x => x.SelectedTrack, x => x.Value != null), //.StartWith(this.SelectedTrack != null),
                    (canLoad, canPlay, isTrackSelected) => (canLoad || canPlay) && isTrackSelected)
                )
                .DisposeWith(this._disposables);

            this.Play = ReactiveCommand.CreateFromTask(
                () =>
                {
                    return this._audioPlaybackEngineAsync.PlayAsync();
                }
                , this._audioPlaybackEngineAsync.WhenCanPlayChanged
                )
                .DisposeWith(this._disposables);

            this.Pause = ReactiveCommand.CreateFromTask(
                () =>
                {
                    return this._audioPlaybackEngineAsync.PauseAsync();
                }
                , this._audioPlaybackEngineAsync.WhenCanPauseChanged
                )
                .DisposeWith(this._disposables);

            this.Resume = ReactiveCommand.CreateFromTask(
                () =>
                {
                    return this._audioPlaybackEngineAsync.ResumeAsync();
                }
                , this._audioPlaybackEngineAsync.WhenCanResumeChanged
                )
                .DisposeWith(this._disposables);

            this.Stop = ReactiveCommand.CreateFromTask(
                () =>
                {
                    return this._audioPlaybackEngineAsync.StopAsync();
                }
                , this._audioPlaybackEngineAsync.WhenCanStopChanged
                )
                .DisposeWith(this._disposables);

            this._durationOAPH = this._audioPlaybackEngineAsync
                .WhenDurationChanged
                .Do(duration => Debug.WriteLine($"{nameof(ShellViewModel)}.{nameof(this.Duration)}: {duration.ToString()}"))
                .ToProperty(this, nameof(this.Duration))
                .DisposeWith(this._disposables);

            this._durationAsTicksOAPH = this._audioPlaybackEngineAsync
                .WhenDurationChanged
                .Select(p => p != null && p.HasValue ? Convert.ToUInt64(p.Value.Ticks) : 0UL)
                //.Do(duration => Debug.WriteLine($"{nameof(ShellViewModel)}.{nameof(this.DurationAsTicks)}: {duration.ToString()}"))
                .ToProperty(this, nameof(this.DurationAsTicks))
                .DisposeWith(this._disposables);

            this._positionOAPH = this._audioPlaybackEngineAsync
                .WhenPositionChanged
                .Do(position => Debug.WriteLine($"{nameof(ShellViewModel)}.{nameof(this.Position)}: {position.ToString()}"))
                .ToProperty(this, nameof(this.Position))
                .DisposeWith(this._disposables);

            this._positionAsTicksOAPH = this._audioPlaybackEngineAsync
                .WhenPositionChanged
                .Select(p => p != null && p.HasValue ? Convert.ToUInt64(p.Value.Ticks) : 0UL)
                //.Do(position => Debug.WriteLine($"{nameof(ShellViewModel)}.{nameof(this.PositionAsTicks)}: {position.ToString()}"))
                .ToProperty(this, nameof(this.PositionAsTicks))
                .DisposeWith(this._disposables);

            this._volumeOAPH = this._audioPlaybackEngineAsync
                .WhenVolumeChanged
                //.Do(position => Debug.WriteLine($"{nameof(ShellViewModel)}.{nameof(this.PositionAsTicks)}: {position.ToString()}"))
                .ToProperty(this, nameof(this.Volume))
                .DisposeWith(this._disposables);

            this.Tracks = new ReadOnlyObservableCollection<Uri>(new ObservableCollection<Uri>(new Uri[] {
                new Uri(DefaultSongFilePath),
                new Uri(@"D:\Music\Brownian noise.flac"),
                new Uri(@"D:\Music\Ed Sheeran - Castle on the hill.mp3"),
                new Uri(@"D:\Music\Santana - Just feel better.mp3"),
            }));
        }

        private ObservableAsPropertyHelper<TimeSpan?> _positionOAPH;
        public TimeSpan? Position => this._positionOAPH.Value;

        private ObservableAsPropertyHelper<ulong> _positionAsTicksOAPH;
        public ulong PositionAsTicks => this._positionAsTicksOAPH.Value;

        private ObservableAsPropertyHelper<ulong> _durationAsTicksOAPH;
        public ulong DurationAsTicks => this._durationAsTicksOAPH.Value;

        private ObservableAsPropertyHelper<TimeSpan?> _durationOAPH;
        public TimeSpan? Duration => this._durationOAPH.Value;

        private ObservableAsPropertyHelper<float> _volumeOAPH;
        public float Volume
        {
            get => this._volumeOAPH.Value;
            set => this._audioPlaybackEngineAsync.Volume = value;
        }

        public ReadOnlyObservableCollection<Uri> Tracks { get; }

        private Uri _selectedTrack;
        public Uri SelectedTrack
        {
            get => this._selectedTrack;
            set => this.RaiseAndSetIfChanged(ref this._selectedTrack, value);
        }

        //public ReactiveCommand<Unit, Unit> DoNothing { get; }
        public ReactiveCommand<Uri, Unit> Load { get; }
        public ReactiveCommand<Uri, Unit> PlayFile { get; }
        public ReactiveCommand<Unit, Unit> Play { get; }
        public ReactiveCommand<Unit, Unit> Pause { get; }
        public ReactiveCommand<Unit, Unit> Resume { get; }
        public ReactiveCommand<Unit, Unit> Stop { get; }

        public void Dispose()
        {
            this._disposables.Dispose();
        }
    }
}