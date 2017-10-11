using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Application
{
    public static class ObservableChangeSet
    {
        public static IObservable<IChangeSet<T>> Create<T>(Func<ISourceList<T>, Task> subscribe)
        {
            if (subscribe == null) throw new ArgumentNullException(nameof(subscribe));

            return Observable.Create<IChangeSet<T>>(async observer =>
            {
                var list = new SourceList<T>();

                try
                {
                    await subscribe(list);
                    // TODO: vNext
                    // list.OnCompleted();
                }
                catch (Exception e)
                {
                    observer.OnError(e);
                }

                return new CompositeDisposable(list.Connect().SubscribeSafe(observer), list, Disposable.Create(observer.OnCompleted));
            });
        }
    }
}