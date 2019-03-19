using DynamicData;
using DynamicData.ReactiveUI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Caliburn.Micro.ReactiveUI
{
    public partial class ReactiveConductor<T>
    {
        /// <summary>
        /// An implementation of <see cref="IConductor"/> that holds on many items.
        /// </summary>
        public partial class Collection
        {
            /// <summary>
            /// An implementation of <see cref="IConductor"/> that holds on many items but only activates one at a time.
            /// </summary>
            public class OneActive : ReactiveConductorBaseWithActiveItem<T>
            {
                private readonly ObservableCollection<T> _items = new ObservableCollection<T>();

                /// <summary>
                /// Initializes a new instance of the <see cref="Conductor&lt;T&gt;.Collection.OneActive"/> class.
                /// </summary>
                public OneActive()
                {
                    this.Items = new ReadOnlyObservableCollection<T>(this._items);

                    this._items.CollectionChanged += (s, e) =>
                    {
                        switch (e.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                                e.NewItems.OfType<IChild>().Apply(x => x.Parent = this);
                                break;
                            case NotifyCollectionChangedAction.Remove:
                                e.OldItems.OfType<IChild>().Apply(x => x.Parent = null);
                                break;
                            case NotifyCollectionChangedAction.Replace:
                                e.NewItems.OfType<IChild>().Apply(x => x.Parent = this);
                                e.OldItems.OfType<IChild>().Apply(x => x.Parent = null);
                                break;
                            case NotifyCollectionChangedAction.Reset:
                                this._items.OfType<IChild>().Apply(x => x.Parent = this);
                                break;
                        }
                    };
                }

                /// <summary>
                /// Gets the items that are currently being conducted.
                /// </summary>
                public ReadOnlyObservableCollection<T> Items { get; }

                /// <summary>
                /// Gets the children.
                /// </summary>
                /// <returns>The collection of children.</returns>
                public override IEnumerable<T> GetChildren()
                {
                    return this._items;
                }

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

                    this.ChangeActiveItem(item, false);
                }

                /// <summary>
                /// Deactivates the specified item.
                /// </summary>
                /// <param name="item">The item to close.</param>
                /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
                public override void DeactivateItem(T item, bool close)
                {
                    if (item == null)
                    {
                        return;
                    }

                    if (!close)
                    {
                        ScreenExtensions.TryDeactivate(item, false);
                    }
                    else
                    {
                        this.CloseStrategy.Execute(new[] { item }, (canClose, closable) =>
                        {
                            if (canClose)
                            {
                                this.CloseItemCore(item);
                            }
                        });
                    }
                }

                void CloseItemCore(T item)
                {
                    if (item.Equals(this.ActiveItem))
                    {
                        var index = this._items.IndexOf(item);
                        var next = this.DetermineNextItemToActivate(this._items, index);

                        this.ChangeActiveItem(next, true);
                    }
                    else
                    {
                        ScreenExtensions.TryDeactivate(item, true);
                    }

                    this._items.Remove(item);
                }

                /// <summary>
                /// Determines the next item to activate based on the last active index.
                /// </summary>
                /// <param name="list">The list of possible active items.</param>
                /// <param name="lastIndex">The index of the last active item.</param>
                /// <returns>The next item to activate.</returns>
                /// <remarks>Called after an active item is closed.</remarks>
                protected virtual T DetermineNextItemToActivate(IList<T> list, int lastIndex)
                {
                    var toRemoveAt = lastIndex - 1;

                    if (toRemoveAt == -1 && list.Count > 1)
                    {
                        return list[1];
                    }

                    if (toRemoveAt > -1 && toRemoveAt < list.Count - 1)
                    {
                        return list[toRemoveAt];
                    }

                    return default(T);
                }

                /// <summary>
                /// Called to check whether or not this instance can close.
                /// </summary>
                /// <param name="callback">The implementor calls this action with the result of the close check.</param>
                public override void CanClose(Action<bool> callback)
                {
                    this.CloseStrategy.Execute(this._items.ToList(), (canClose, closable) =>
                    {
                        if (!canClose && closable.Any())
                        {
                            if (closable.Contains(this.ActiveItem))
                            {
                                var list = this._items.ToList();
                                var next = this.ActiveItem;
                                do
                                {
                                    var previous = next;
                                    next = this.DetermineNextItemToActivate(list, list.IndexOf(previous));
                                    list.Remove(previous);
                                } while (closable.Contains(next));

                                var previousActive = this.ActiveItem;
                                this.ChangeActiveItem(next, true);
                                this._items.Remove(previousActive);

                                var stillToClose = closable.ToList();
                                stillToClose.Remove(previousActive);
                                closable = stillToClose;
                            }

                            closable.OfType<IDeactivate>().Apply(x => x.Deactivate(true));
                            this._items.Remove(closable);
                        }

                        callback(canClose);
                    });
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
                    if (close)
                    {
                        this._items.OfType<IDeactivate>().Apply(x => x.Deactivate(true));
                        this._items.Clear();
                    }
                    else
                    {
                        ScreenExtensions.TryDeactivate(this.ActiveItem, false);
                    }
                }

                /// <summary>
                /// Ensures that an item is ready to be activated.
                /// </summary>
                /// <param name="newItem">The item that is about to be activated.</param>
                /// <returns>The item to be activated.</returns>
                protected override T EnsureItem(T newItem)
                {
                    if (newItem == null)
                    {
                        newItem = this.DetermineNextItemToActivate(this._items, this.ActiveItem != null ? this._items.IndexOf(this.ActiveItem) : 0);
                    }
                    else
                    {
                        var index = this._items.IndexOf(newItem);

                        if (index == -1)
                            this._items.Add(newItem);
                        else
                            newItem = this._items[index];
                    }

                    return base.EnsureItem(newItem);
                }
            }
        }
    }
}
