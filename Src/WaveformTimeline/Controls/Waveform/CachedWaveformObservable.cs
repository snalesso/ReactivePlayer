using System;
using System.Collections.Generic;
using System.Linq;
using WaveformTimeline.Contracts;

namespace WaveformTimeline.Controls.Waveform
{
    internal sealed class CachedWaveformObservable : IAudioWaveformStream
    {
        public CachedWaveformObservable(float[] waveformFloats, WaveformSection section)
        {
            this._waveformFloats = waveformFloats;
            this._section = section;
            this._cachedIndexes = Enumerable.Range(section.Start, section.End - section.Start).ToList();
        }

        private readonly float[] _waveformFloats;
        private readonly WaveformSection _section;
        private readonly List<int> _cachedIndexes;
        private readonly List<IObserver<float>> _observers = new List<IObserver<float>>();

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<float>> _allObservers;
            private readonly IObserver<float> _observer;

            public Unsubscriber(List<IObserver<float>> allObservers, IObserver<float> observer)
            {
                this._allObservers = allObservers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (this._observer != null && this._allObservers.Contains(this._observer))
                    this._allObservers.Remove(this._observer);
            }
        }

        public IDisposable Subscribe(IObserver<float> observer)
        {
            if (!this._observers.Contains(observer))
            {
                this._observers.Add(observer);
            }

            return new Unsubscriber(this._observers, observer);
        }

        public void Waveform(int resolution)
        {
            var maxLength = this._waveformFloats.Length;
            if (this._section.Start > maxLength || this._section.End > maxLength)
                // TODO: localize
                throw new IndexOutOfRangeException("Input array does not match the provided WaveformSection");

            this._cachedIndexes.ForEach(i => this._observers.ForEach(o => o.OnNext(this._waveformFloats[i])));
        }
    }
}
