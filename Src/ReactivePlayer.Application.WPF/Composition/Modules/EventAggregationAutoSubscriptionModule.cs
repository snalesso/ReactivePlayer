using Autofac;
using Caliburn.Micro;

namespace ReactivePlayer.Application.WPF.Composition.Modules
{
    public class EventAggregationAutoSubscriptionModule : Autofac.Module
    {
        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry registry,
            Autofac.Core.IComponentRegistration registration)
        {
            registration.Activated += OnComponentActivated;
        }

        private static void OnComponentActivated(object sender, Autofac.Core.ActivatedEventArgs<object> e)
        {
            if (e == null)
                return;
            var handle = e.Instance as IHandle;
            if (handle == null)
                return;
            e.Context.Resolve<IEventAggregator>().Subscribe(handle);
        }
    }
}