using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Shops.Entities.List
{
    public abstract class AbstractProductList<TRowType>
    {
        private readonly Dictionary<string, TRowType> _list = new ();

        public void Add(AbstractProductList<TRowType> list)
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));
            foreach (var row in list._list)
            {
                Add(row.Value);
            }
        }

        public void Put(TRowType row)
        {
            _list[GetRowId(row)] = row;
        }

        public void TryRemove(string rowId)
        {
            _list.Remove(rowId);
        }

        public bool TryGetValue(string id, out TRowType row)
        {
            return _list.TryGetValue(id, out row);
        }

        public ImmutableList<TRowType> GetRows()
        {
            return _list.Values.ToImmutableList();
        }

        protected abstract string GetRowId(TRowType row);
        protected abstract TRowType MergeRows(TRowType row1, TRowType row2);

        protected void Add(TRowType row)
        {
            string id = GetRowId(row);
            if (_list.TryGetValue(id, out TRowType existingRow))
            {
                _list[id] = MergeRows(existingRow, row);
            }
            else
            {
                _list[id] = row;
            }
        }
    }
}