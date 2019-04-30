using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.Collections
{
    public class ObservableOrderedSet<T> : ISet<T>
    {
        #region constants & fields

        private readonly IDictionary<T, int> _indexesByKey;
        private readonly IList<T> _keysByIndex;

        #endregion

        #region ctors

        public ObservableOrderedSet(ISet<T> initialItems)
        {
            var capacity = initialItems != null ? initialItems.Count : 0;

            this._indexesByKey = new Dictionary<T, int>(capacity);
            foreach (var item in initialItems)
            {
                this.SilentAdd(item);
            }

            this._keysByIndex = new List<T>(initialItems);
        }

        public ObservableOrderedSet() : this(null)
        {
        }

        #endregion

        #region methods

        private bool SilentAdd(T item)
        {
            if (this._indexesByKey.ContainsKey(item))
                return false;

            this._keysByIndex.Add(item);
            this._indexesByKey.Add(item, this._keysByIndex.Count);

            return true;
        }

        #endregion

        #region ISet<T>

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => false;

        public bool Add(T item)
        {
            var wasAdded = this.SilentAdd(item);

            //if (wasAdded)

            return wasAdded;
        }

        public void Clear()
        {
            this._keysByIndex.Clear();
            this._indexesByKey.Clear();
        }

        public bool Contains(T item)
        {
            return this._indexesByKey.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._keysByIndex.GetEnumerator();
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
            if (!this._indexesByKey.TryGetValue(item, out var index))
                return false;

            var wasRemoved = this._indexesByKey.Remove(item);
            if (wasRemoved)
                this._keysByIndex.RemoveAt(index);

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
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}