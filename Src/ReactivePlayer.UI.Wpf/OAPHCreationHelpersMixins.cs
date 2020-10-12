using ReactiveUI;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Concurrency;

// https://github.com/reactiveui/ReactiveUI/blob/main/src/ReactiveUI/ObservableForProperty/OAPHCreationHelperMixin.cs
namespace ReactivePlayer
{
    /// <summary>
    /// A collection of helpers to aid working with observable properties.
    /// </summary>
    public static class IScreenOAPHCreationHelperMixin
    {
        public static ObservableAsPropertyHelper<TRet> ToPropertyCM<TRet>(
            this IObservable<TRet> observable,
            string propertyName)
        {
            return null;
        }

        public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
            this IObservable<TRet> observable,
            TObj obj,
            Expression<Func<TObj, TRet>> property,
            Action<string>? propertyChangedRaiser,
            //Action<string>? propertyChangingRaiser,
            TRet initialValue = default,
            bool deferSubscription = false,
            IScheduler? scheduler = null)
            where TObj : INotifyPropertyChanged
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (propertyChangedRaiser == null)
            {
                throw new ArgumentNullException(nameof(propertyChangedRaiser));
            }

            Expression expression = Reflection.Rewrite(property.Body);

            if (expression.GetParent().NodeType != ExpressionType.Parameter)
            {
                throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty'");
            }

            var name = expression.GetMemberInfo().Name;
            if (expression is IndexExpression)
            {
                name += "[]";
            }

            return new ObservableAsPropertyHelper<TRet>(
                observable,
                _ => propertyChangedRaiser(name),
                default,
                initialValue,
                deferSubscription,
                scheduler);
        }

        public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
            this IObservable<TRet> observable,
            TObj obj,
            string propertyName,
            Action<string>? propertyChangedRaiser,
            TRet initialValue = default,
            bool deferSubscription = false,
            IScheduler? scheduler = null)
            where TObj : INotifyPropertyChanged
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (propertyChangedRaiser == null)
            {
                throw new ArgumentNullException(nameof(propertyChangedRaiser));
            }

            return new ObservableAsPropertyHelper<TRet>(
                observable,
                _ => propertyChangedRaiser(propertyName),
                default,
                initialValue,
                deferSubscription,
                scheduler);
        }

        public static ObservableAsPropertyHelper<TRet> ToProperty<TRet>(
            this IObservable<TRet> observable,
            string propertyName,
            Action<string>? propertyChangedRaiser,
            TRet initialValue = default,
            bool deferSubscription = false,
            IScheduler? scheduler = null)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (propertyChangedRaiser == null)
            {
                throw new ArgumentNullException(nameof(propertyChangedRaiser));
            }

            return new ObservableAsPropertyHelper<TRet>(
                observable,
                _ => propertyChangedRaiser(propertyName),
                default,
                initialValue,
                deferSubscription,
                scheduler);
        }
    }
}
