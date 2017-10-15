using Autofac;
using Autofac.Core;
using Caliburn.Micro;

namespace ReactivePlayer.UI.WPF.Composition.Autofac.Modules
{
    public class EventAggregationAutoSubscriptionModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry registry, IComponentRegistration registration)
        {
            registration.Activated += EventAggregationAutoSubscriptionModule.OnComponentActivated;
        }

        private static void OnComponentActivated(object sender, ActivatedEventArgs<object> e)
        {
            var handle = e?.Instance as IHandle;
            if (handle == null) return;

            e.Context.Resolve<IEventAggregator>().Subscribe(handle);
        }
    }
}