using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.Collections
{
    public class ObservableSet<T> : ISet<T>, INotifyCollectionChanged
    {
        private readonly ISet<T> _set;
        private readonly ObservableCollection<T> _observableCollection;

        public ObservableSet()
        {
        }

        public ObservableSet(ISet<T> items)
        {
            this._set = items ?? new HashSet<T>();
            this._observableCollection = new ObservableCollection<T>(this._set);
        }

        public int Count => this._set.Count;

        public bool IsReadOnly => this._set.IsReadOnly;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => this._observableCollection.CollectionChanged += value;
            remove => this._observableCollection.CollectionChanged -= value;
        }

        public bool Add(T item)
        {
            var wasAdded = this._set.Add(item);
            if (wasAdded)
                this._observableCollection.Add(item);

            return wasAdded;
        }

        public void Clear()
        {
            this._set.Clear();
            this._observableCollection.Clear();
        }

        public bool Contains(T item)
        {
            return this._set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._set.CopyTo(array, arrayIndex);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._set.GetEnumerator();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            var wasRemoved = this._set.Remove(item);

            if (wasRemoved)
                this._observableCollection.Remove(item);

            return wasRemoved;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.Add(T item)
        {
            (this._set as ICollection<T>).Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this._set as IEnumerable).GetEnumerator();
        }
    }
}