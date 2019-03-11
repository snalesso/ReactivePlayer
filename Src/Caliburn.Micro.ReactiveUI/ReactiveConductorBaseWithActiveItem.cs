namespace Caliburn.Micro.ReactiveUI
{
    /// <summary>
    /// A base class for various implementations of <see cref="IConductor"/> that maintain an active item.
    /// </summary>
    /// <typeparam name="T">The type that is being conducted.</typeparam>
    public abstract class ReactiveConductorBaseWithActiveItem<T> : ReactiveConductorBase<T>, IConductActiveItem where T : class
    {
        T activeItem;

        /// <summary>
        /// The currently active item.
        /// </summary>
        public T ActiveItem
        {
            get { return this.activeItem; }
            set { this.ActivateItem(value); }
        }

        /// <summary>
        /// The currently active item.
        /// </summary>
        /// <value></value>
        object IHaveActiveItem.ActiveItem
        {
            get { return this.ActiveItem; }
            set { this.ActiveItem = (T)value; }
        }

        /// <summary>
        /// Changes the active item.
        /// </summary>
        /// <param name="newItem">The new item to activate.</param>
        /// <param name="closePrevious">Indicates whether or not to close the previous active item.</param>
        protected virtual void ChangeActiveItem(T newItem, bool closePrevious)
        {
            ScreenExtensions.TryDeactivate(this.activeItem, closePrevious);

            newItem = this.EnsureItem(newItem);

            if (this.IsActive)
                ScreenExtensions.TryActivate(newItem);

            this.activeItem = newItem;
            this.NotifyOfPropertyChange("ActiveItem");
            this.OnActivationProcessed(this.activeItem, true);
        }
    }
}
