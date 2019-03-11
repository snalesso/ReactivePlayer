using CSCore.CoreAudioAPI;
using CSCore.Ffmpeg;
using CSCore.MediaFoundation;
using CSCore.SoundOut;
using CSCore.Streams;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSCore.SimpleControlsSync
{
    public class CSCorePlayer : IDisposable
    {
        // Singleton
        private static CSCorePlayer instance;

        // ISPectrumPlayer
        private List<EventHandler<SingleBlockReadEventArgs>> inputStreamList = new List<EventHandler<SingleBlockReadEventArgs>>();

        // IPlayer
        private string filename;
        private bool canPlay;
        private bool canPause;
        private bool canStop;

        // Output device
        private bool useAllAvailableChannels = false;
        private int latency = 100; // Default is 100
        private bool eventSync = false; // Default is False
        private AudioClientShareMode audioClientShareMode = AudioClientShareMode.Shared; // Default is Shared
        private SingleBlockNotificationStream notificationSource;
        private float volume = 1.0F;
        private ISoundOut soundOut;
        Stream audioStream;

        // Equalizer
        private double[] filterValues;

        // Flags
        private bool isPlaying;
        private bool supportsWindowsMediaFoundation = false;

        // To detect redundant calls
        private bool disposedValue = false;

        public CSCorePlayer()
        {
            this.canPlay = true;
            this.canPause = false;
            this.canStop = false;
        }

        public static CSCorePlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CSCorePlayer();
                }
                return instance;
            }
        }

        public SingleBlockNotificationStream NotificationSource => this.notificationSource;

        public bool SupportsWindowsMediaFoundation
        {
            get { return this.supportsWindowsMediaFoundation; }
            set { this.supportsWindowsMediaFoundation = value; }
        }

        public string Filename
        {
            get { return this.filename; }
        }

        public bool CanPlay
        {
            get { return this.canPlay; }
        }

        public bool CanPause
        {
            get { return this.canPause; }
        }

        public bool CanStop
        {
            get { return this.canStop; }
        }

        public event EventHandler PlaybackFinished = delegate { };
        public event PlaybackInterruptedEventHandler PlaybackInterrupted = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        public void SetPlaybackSettings(int latency, bool eventMode, bool exclusiveMode, double[] filterValues, bool useAllAvailableChannels)
        {
            this.useAllAvailableChannels = useAllAvailableChannels;
            this.latency = latency;
            this.eventSync = eventMode;
            this.filterValues = filterValues;

            if (exclusiveMode)
            {
                this.audioClientShareMode = AudioClientShareMode.Exclusive;
            }
            else
            {
                this.audioClientShareMode = AudioClientShareMode.Shared;
            }
        }

        public TimeSpan GetCurrentTime()
        {
            // Make sure soundOut is not stopped, otherwise we get a NullReferenceException in CSCore.
            if (this.soundOut != null && this.soundOut.PlaybackState != PlaybackState.Stopped && this.soundOut.WaveSource != null)
            {
                return this.soundOut.WaveSource.GetPosition();
            }

            return new TimeSpan(0);
        }

        public TimeSpan GetTotalTime()
        {
            // Make sure soundOut is not stopped, otherwise we get a NullReferenceException in CSCore.
            if (this.soundOut != null && this.soundOut.PlaybackState != PlaybackState.Stopped && this.soundOut.WaveSource != null)
            {
                return this.soundOut.WaveSource.GetLength();
            }

            return new TimeSpan(0);
        }

        public float GetVolume()
        {
            return this.soundOut.Volume;
        }

        public void Pause()
        {
            if (this.CanPause)
            {
                try
                {
                    this.soundOut.Pause();

                    this.IsPlaying = false;

                    this.canPlay = true;
                    this.canPause = false;
                    this.canStop = true;
                }
                catch (Exception)
                {
                    this.Stop();
                    throw;
                }
            }
        }

        public bool Resume()
        {
            if (this.CanPlay)
            {
                try
                {
                    this.soundOut.Play();

                    this.IsPlaying = true;

                    this.canPlay = false;
                    this.canPause = true;
                    this.canStop = true;
                    return true;
                }
                catch (Exception)
                {
                    this.Stop();
                    throw;
                }
            }

            return false;
        }
        
        public void Play(string filename)
        {
            this.filename = filename;

            this.IsPlaying = true;

            this.canPlay = false;
            this.canPause = true;
            this.canStop = true;

            this.InitializeSoundOut(this.GetCodec(this.filename));
            this.soundOut.Play();
        }

        private IWaveSource GetCodec(string filename)
        {
            IWaveSource waveSource = null;
            bool useFfmpegDecoder = true;

            // FfmpegDecoder doesn't support WMA lossless. If Windows Media Foundation is available,
            // we can use MediaFoundationDecoder for WMA files, which supports WMA lossless.
            if (this.supportsWindowsMediaFoundation && Path.GetExtension(filename).ToLower().Equals(".wma"))
            {
                try
                {
                    waveSource = new MediaFoundationDecoder(filename);
                    useFfmpegDecoder = false;
                }
                catch (Exception)
                {
                }
            }

            if (useFfmpegDecoder)
            {
                // waveSource = new FfmpegDecoder(this.filename);

                // On some systems, files with special characters (e.g. "æ", "ø") can't be opened by FfmpegDecoder.
                // This exception is thrown: avformat_open_input returned 0xfffffffe: No such file or directory. 
                // StackTrace: at CSCore.Ffmpeg.FfmpegCalls.AvformatOpenInput(AVFormatContext** formatContext, String url)
                // This issue can't be reproduced for now, so we're using a stream as it works in all cases.
                // See: https://github.com/digimezzo/Dopamine/issues/746
                // And: https://github.com/filoe/cscore/issues/344
                this.audioStream = File.OpenRead(filename);
                waveSource = new FfmpegDecoder(this.audioStream);
            }

            // If the SampleRate < 32000, make it 32000. The Equalizer's maximum frequency is 16000Hz.
            // The sample rate has to be bigger than 2 * frequency.
            if (waveSource.WaveFormat.SampleRate < 32000) waveSource = waveSource.ChangeSampleRate(32000);

            return waveSource
                .ToSampleSource()
                .ToWaveSource();
        }

        public void SetVolume(float volume)
        {
            try
            {
                if (volume >= 0)
                {
                    this.volume = volume;
                }
                else
                {
                    this.volume = 0;
                }

                if (this.soundOut != null)
                {
                    this.soundOut.Volume = volume;
                }
            }
            catch (Exception)
            {
                // Swallow
            }
        }

        public void Skip(int gotoSeconds)
        {
            try
            {
                this.soundOut.WaveSource.SetPosition(new TimeSpan(0, 0, gotoSeconds));
            }
            catch (Exception)
            {
                // Swallow
            }
        }

        public void Stop()
        {
            this.CloseSoundOut();

            if (this.CanStop)
            {
                this.IsPlaying = false;

                this.canPlay = true;
                this.canPause = false;
                this.canStop = false;
            }
        }

        private void InitializeSoundOut(IWaveSource soundSource)
        {
            // Create SoundOut
            if (this.supportsWindowsMediaFoundation)
            {
                this.soundOut = new WasapiOut(this.eventSync, this.audioClientShareMode, this.latency, ThreadPriority.Highest);

                // Map stereo or mono file to all channels
                ((WasapiOut)this.soundOut).UseChannelMixingMatrices = this.useAllAvailableChannels;

                    // If no output device was provided, we're playing on the default device.
                    // In such case, we want to detect when the default device changes.
                    // This is done by setting stream routing options
                    ((WasapiOut)this.soundOut).StreamRoutingOptions = StreamRoutingOptions.All;

                // Initialize SoundOut 
                this.notificationSource = new SingleBlockNotificationStream(soundSource.ToSampleSource());
                this.soundOut.Initialize(this.notificationSource.ToWaveSource(16));

                if (this.inputStreamList.Count != 0)
                {
                    foreach (var inputStream in this.inputStreamList)
                    {
                        this.notificationSource.SingleBlockRead += inputStream;
                    }
                }
            }
            else
            {
                this.soundOut = new DirectSoundOut(this.latency, ThreadPriority.Highest);

                // Initialize SoundOut
                // Spectrum analyzer performance is only acceptable with WasapiOut,
                // so we're not setting a notificationSource for DirectSoundOut
                this.soundOut.Initialize(soundSource);
            }

            this.soundOut.Stopped += this.SoundOutStoppedHandler;
            this.soundOut.Volume = this.volume;
        }

        private void NotifyPropertyChanged(string info)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        private void CloseSoundOut()
        {
            // soundOut
            if (this.soundOut != null)
            {
                try
                {
                    if (this.notificationSource != null)
                    {
                        foreach (var inputStream in this.inputStreamList)
                        {
                            this.notificationSource.SingleBlockRead -= inputStream;
                        }
                    }

                    // Remove the handler because we don't want to trigger this.soundOut.Stopped()
                    // when manually stopping the player. That event should only be triggered
                    // when CSCore reaches the end of the Track by itself.
                    this.soundOut.Stopped -= this.SoundOutStoppedHandler;
                    this.soundOut.Stop();

                    if (this.soundOut.WaveSource != null) this.soundOut.WaveSource.Dispose();

                    this.soundOut.Dispose();
                    this.soundOut = null;
                }
                catch (Exception)
                {
                    //Swallow
                }
            }

            // audioStream
            if (this.audioStream != null)
            {
                try
                {
                    this.audioStream.Dispose();
                }
                catch (Exception)
                {
                    //Swallow
                }
            }
        }

        public bool IsPlaying
        {
            get { return this.isPlaying; }
            set
            {
                this.isPlaying = value;
                this.NotifyPropertyChanged("IsPlaying");
            }
        }
        
        public void SoundOutStoppedHandler(object sender, PlaybackStoppedEventArgs e)
        {
            try
            {
                if (e.Exception != null)
                {
                    if (PlaybackInterrupted != null)
                    {
                        this.PlaybackInterrupted(this, new PlaybackInterruptedEventArgs { Message = e.Exception.Message });
                    }
                }
                else
                {
                    if (PlaybackFinished != null)
                    {
                        this.PlaybackFinished(this, new EventArgs());
                    }
                }
            }
            catch (Exception)
            {
                // Do nothing. It might be that we get in this handler when the application is closed.
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.CloseSoundOut();
                }

                this.disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            this.Dispose(true);
        }
    }

    public delegate void PlaybackInterruptedEventHandler(object sender, PlaybackInterruptedEventArgs e);

    public class PlaybackInterruptedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}