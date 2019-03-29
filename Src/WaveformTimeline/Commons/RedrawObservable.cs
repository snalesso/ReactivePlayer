using System;
using System.Collections.Generic;

namespace WaveformTimeline.Commons
{
    /// <summary>
    /// Helper used in the main control to observe, and throttle, events that can trigger re-rendering.
    /// </summary>
    internal sealed class RedrawObservable : IObservable<int>
    {
        private int _counter;
        private readonly List<IObserver<int>> _observers = new List<IObserver<int>>();

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<int>> _allObservers;
            private readonly IObserver<int> _observer;

            public Unsubscriber(List<IObserver<int>> allObservers, IObserver<int> observer)
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

        public void Increment()
        {
            this._counter++;
            this._observers.ForEach(o => o.OnNext(this._counter));
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            if (!this._observers.Contains(observer))
                this._observers.Add(observer);
            return new Unsubscriber(this._observers, observer);
        }
    }
}