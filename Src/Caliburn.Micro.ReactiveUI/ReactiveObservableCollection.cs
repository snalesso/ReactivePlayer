﻿using System.Reactive.Concurrency;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caliburn.Micro.ReactiveUI
{
    public class ReactiveObservableCollection<T> : ReactiveList<T>, IObservableCollection<T>
    {
        public bool IsNotifying { get; set; }

        public ReactiveObservableCollection()
            : base()
        {
        }

        public ReactiveObservableCollection(IEnumerable<T> initialContents)
            : base(initialContents)
        {
        }

        public ReactiveObservableCollection(IEnumerable<T> initialContents = null, double resetChangeThreshold = 0.3, IScheduler scheduler = null)
            : base(initialContents, resetChangeThreshold, scheduler)
        {
        }

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        public void NotifyOfPropertyChange(string propertyName)
        {
            this.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        //  Raises a change notification indicating that all bindings should be refreshed.
        /// </summary>
        public void Refresh()
        {
            NotifyOfPropertyChange(string.Empty);
        }

        /// <summary>
        /// Removes items.
        /// </summary>
        /// <param name="items">The items to be removed.</param>
        public void RemoveRange(IEnumerable<T> items)
        {
            RemoveAll(items);
        }
    }
}
