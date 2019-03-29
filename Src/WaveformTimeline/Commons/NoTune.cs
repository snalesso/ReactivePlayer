using System;
using System.Reactive.Disposables;
using WaveformTimeline.Contracts;

namespace WaveformTimeline.Commons
{
    /// <summary>
    /// Fall-back "null" implementation of ITune.
    /// </summary>
    public sealed class NoTune : ITune
    {
        public NoTune() : this(0)
        {
        }

        public NoTune(double durationInSeconds)
        {
            this._durationInSeconds = durationInSeconds;
        }

        private readonly double _durationInSeconds;

        public string Name()
        {
            // TODO: localize
            return "No tune";
        }

        public TimeSpan CurrentTime()
        {
            return TimeSpan.Zero;
        }

        public TimeSpan TotalTime()
        {
            return TimeSpan.Zero;
        }

        public TimeSpan Duration()
        {
            return this.TotalTime();
        }

        public byte[] WaveformData()
        {
            return new byte[0];
        }

        public IAudioWaveformStream WaveformStream()
        {
            return new DummyWaveformObservable();
        }

        public double[] Cues()
        {
            return new double[0];
        }

        public double Tempo()
        {
            return 100;
        }

        public bool IsPlaybackOn()
        {
            return false;
        }

        public void Seek(TimeSpan position)
        {
            //no-op   
        }

        public void TrimStart(TimeSpan start)
        {
            //no-op   
        }

        public void TrimEnd(TimeSpan end)
        {
            //no-op   
        }

        public event EventHandler<EventArgs> Transitioned;
        public event EventHandler<EventArgs> TempoShifted;

        private class DummyWaveformObservable : IAudioWaveformStream
        {
            public IDisposable Subscribe(IObserver<float> observer)
            {
                return Disposable.Create(() => { });
            }

            public void Waveform(int resolution)
            {
            }
        }
    }
}
