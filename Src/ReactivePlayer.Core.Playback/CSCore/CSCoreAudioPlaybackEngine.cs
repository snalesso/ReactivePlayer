using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback.CSCore
{
    // TODO: group status events in single subscriptions grouped by status in order to have Status == Loaded => Duration update before Position update & the inverse when Status == Stopped
    // TODO: buffering support, SingleBlockNotificationStream?? Dopamine docet
    // TODO: annotate class behavior
    // TODO: investigate IWaveSource.AppendSource
    // TODO: investigate IWaveSource.GetLength: how does it calculate duration? How accurate is it guaranteed to be?
    // TODO: add timeout for track loading?
    // TODO: consider using AsyncLock (System.Reactive.Core)
    public class CSCoreAudioPlaybackEngine : IAudioPlaybackEngine // TODO: learn how to handle IDisposable from outside and in general how to handle interfaces which implementations may or may not be IDisposable
    {
        #region constants & fields

        // TODO: benchmark impact high frequency position updates
        //private readonly TimeSpan _positionUpdatesInterval = TimeSpan.FromMilliseconds(100);
        //private readonly SemaphoreSlim _playbackActionsSemaphore = new SemaphoreSlim(1, 1);

        private ISoundOut __soundOut;

        #endregion

        #region ctor

        // TODO: add logger
        public CSCoreAudioPlaybackEngine(/*TimeSpan positionUpdatesInterval = default(TimeSpan)*/)
        {
            this._playerScopeDisposables = new CompositeDisposable();
            this.__playbackScopeDisposables = new CompositeDisposable().DisposeWith(this._playerScopeDisposables);
        }

        #endregion

        #region methods

        #region public

        public void Load(Uri audioSourceLocation)
        {
            try
            {
                // TODO: expose and make selectable internal playback engine
                if (WasapiOut.IsSupportedOnCurrentPlatform)
                    this.__soundOut = new WasapiOut();
                else
                    this.__soundOut = new DirectSoundOut(100);

                var xxx = CodecFactory.Instance.GetCodec(audioSourceLocation);
                this.__soundOut.Initialize(xxx);

                // register playbackScopeDisposables
                //this.__soundOut.WaveSource.DisposeWith(this.__playbackScopeDisposables);
                //this.__soundOut.DisposeWith(this.__playbackScopeDisposables);
            }
            catch (Exception ex)
            {
                // TODO: move to logger
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(Load)}: {ex.Message}");

                switch (ex)
                {
                    case NullReferenceException nse:
                        break;
                    case NotSupportedException nse:
                        break;
                    case ArgumentNullException ane:
                        break;
                    default:
                        break;
                }

                // ensure resources allocated during the failed loading process are released
                // TODO: this might be moved/duped at loading start
                this.__playbackScopeDisposables.Clear();

                throw ex;
            }
            finally
            {
            }
        }

        public void Play()
        {
            try
            {
                this.__soundOut.Play();

                SpinWait.SpinUntil(() => this.__soundOut.PlaybackState == PlaybackState.Playing);

            }
            catch (Exception ex)
            {
                // TODO: move to logger
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(Play)}: {ex.Message}");

                //throw ex;
            }
            finally
            {
            }
        }

        public void Pause()
        {
            try
            {
                this.__soundOut.Pause();
                SpinWait.SpinUntil(() => this.__soundOut.PlaybackState == PlaybackState.Paused);

            }
            catch (Exception ex)
            {
                // TODO: move to logger
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(Pause)}: {ex.Message}");
                //throw;
            }
            finally
            {
            }
        }

        public void Resume()
        {
            try
            {
                this.__soundOut.Resume();
                SpinWait.SpinUntil(() => this.__soundOut.PlaybackState == PlaybackState.Playing);

            }
            catch (Exception ex)
            {
                // TODO: move to logger
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(Resume)}: {ex.Message}");
                //throw;
            }
            finally
            {
            }
        }

        public void Stop()
        {
            try
            {
                // TODO: consider adding the WaitForStopped( timeout )
                // TODO: compare WaitForStopped which uses a WaitHandle https://github.com/filoe/cscore/blob/29410b12ae35321c4556b072c0711a8f289c0544/CSCore/Extensions.cs#L410 vs SpinWait.SpinUntil
                if (this.__soundOut != null && this.__soundOut.PlaybackState !=  PlaybackState.Stopped)
                {
                    this.__soundOut.Stop();
                    this.__soundOut.WaitForStopped();

                    SpinWait.SpinUntil(() => this.__soundOut.PlaybackState == PlaybackState.Stopped);
                }
            }
            catch (Exception ex)
            {
                // TODO: log
                Debug.WriteLine(Environment.NewLine + $"{ex.GetType().Name} thrown in {this.GetType().Name}.{nameof(Stop)}: {ex.Message}");
                //throw;
            }
            finally
            {
            }
        }

        #endregion

        #endregion

        #region IDisposable

        // disposables prefixed by _ (single underscore) live for the entire lifetime of the object instance
        private CompositeDisposable _playerScopeDisposables;
        private object _playerScopeDisposingLock = new object();
        // disposables prefixed by __ (double underscore) are transient and can be disposed and recreated multiple times
        private CompositeDisposable __playbackScopeDisposables;

        // TODO: review implementation, also consider if there's some Interlocked way to do it
        public void Dispose()
        {
            try
            {
                lock (this._playerScopeDisposingLock)
                {
                    if (this._playerScopeDisposables != null && !this._playerScopeDisposables.IsDisposed)
                    {
                        // no need to ISoundOut.Stop() because the CompositeDisposable disposes the SoundOut which internally calls .Stop()
                        this._playerScopeDisposables?.Dispose();
                        this._playerScopeDisposables = null;
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