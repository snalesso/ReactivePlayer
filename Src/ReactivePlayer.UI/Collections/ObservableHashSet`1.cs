using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.Collections
{
    public class ObservableHashSet<T> : ObservableCollection<T>, ISet<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region const & fields

        private readonly HashSet<T> _hashSet;
        //private readonly ObservableCollection<T> _observableCollection;

        #endregion

        #region ctors

        public ObservableHashSet()
        {
        }

        public ObservableHashSet(ISet<T> items)
        {
            this._hashSet = new HashSet<T>(items);
        }

        #endregion

        //#region INotifyPropertyChanged

        //public event PropertyChangedEventHandler PropertyChanged
        //{
        //    add => this._observableCollection.PropertyChanged += value;
        //    remove => this._observableCollection.CollectionChanged -= value;
        //}

        //#endregion

        //#region INotifyCollectionChanged

        //public event NotifyCollectionChangedEventHandler CollectionChanged
        //{
        //    add => this._observableCollection.CollectionChanged += value;
        //    remove => this._observableCollection.CollectionChanged -= value;
        //}

        //#endregion

        #region ISet<T>

        public new int Count => this._hashSet.Count;

        public bool IsReadOnly => false;

        public new bool Add(T item)
        {
            var wasAdded = this._hashSet.Add(item);

            if (wasAdded)
                base.Add(item);

            return wasAdded;
        }

        public new void Clear()
        {
            this._hashSet.Clear();
            base.Clear();
        }

        public new bool Contains(T item)
        {
            return this._hashSet.Contains(item);
        }

        public new void CopyTo(T[] array, int arrayIndex)
        {
            this._hashSet.CopyTo(array, arrayIndex);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return this._hashSet.GetEnumerator();
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

        public new bool Remove(T item)
        {
            var wasRemoved = this._hashSet.Remove(item);

            if (wasRemoved)
                base.Remove(item);

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

        //void ICollection<T>.Add(T item)
        //{
        //    (this._set as ICollection<T>).Add(item);
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return (this._set as IEnumerable).GetEnumerator();
        //}

        #endregion
    }
}