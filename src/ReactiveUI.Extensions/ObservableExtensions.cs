using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveUI.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IObservable{T}"/>.
    /// </summary>
    public static class ObservableExtensions
    {
        public static IObservable<Unit> ToUnit<T>(this IObservable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.Select(_ => Unit.Default);
        }
    }
}