using System;
using System.Reactive.Disposables;

namespace ReactivePlayer.Core
{
    public static class IDisposableMixins
    {
        public static T DisposeWith<T>(this T @this, CompositeDisposable compositeDisposable)
            where T : IDisposable
        {
            compositeDisposable.Add(@this);
            return @this;
        }
    }
    public static class OAPHHelpers
    {

    }
}