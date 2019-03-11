using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caliburn.Micro.ReactiveUI
{
    /// <summary>
    /// An implementation of <see cref="IConductor"/> that holds on to and activates only one item at a time.
    /// </summary>
    public partial class ReactiveConductor<T> : ReactiveConductorBaseWithActiveItem<T> where T : class
    {
        /// <summary>
        /// Activates the specified item.
        /// </summary>
        /// <param name="item">The item to activate.</param>
        public override void ActivateItem(T item)
        {
            if (item != null && item.Equals(this.ActiveItem))
            {
                if (this.IsActive)
                {
                    ScreenExtensions.TryActivate(item);
                    this.OnActivationProcessed(item, true);
                }
                return;
            }

            this.CloseStrategy.Execute(new[] { this.ActiveItem }, (canClose, items) =>
            {
                if (canClose)
                    this.ChangeActiveItem(item, true);
                else this.OnActivationProcessed(item, false);
            });
        }

        /// <summary>
        /// Deactivates the specified item.
        /// </summary>
        /// <param name="item">The item to close.</param>
        /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
        public override void DeactivateItem(T item, bool close)
        {
            if (item == null || !item.Equals(this.ActiveItem))
            {
                return;
            }

            this.CloseStrategy.Execute(new[] { this.ActiveItem }, (canClose, items) =>
            {
                if (canClose)
                    this.ChangeActiveItem(default(T), close);
            });
        }

        /// <summary>
        /// Called to check whether or not this instance can close.
        /// </summary>
        /// <param name="callback">The implementor calls this action with the result of the close check.</param>
        public override void CanClose(Action<bool> callback)
        {
            this.CloseStrategy.Execute(new[] { this.ActiveItem }, (canClose, items) => callback(canClose));
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            ScreenExtensions.TryActivate(this.ActiveItem);
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            ScreenExtensions.TryDeactivate(this.ActiveItem, close);
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns>The collection of children.</returns>
        public override IEnumerable<T> GetChildren()
        {
            return new[] { this.ActiveItem };
        }
    }
}
