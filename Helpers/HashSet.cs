using System.Collections.Generic;
using System.Diagnostics;

namespace CompatCheckAndMigrate.Helpers
{
    public class HashSet<T>
    {
        public HashSet(IEqualityComparer<T> comparer)
        {
            Debug.Assert(comparer != null, "comparer must not be null");
            _dictionary = new Dictionary<T, int>(comparer);
        }

        public HashSet(IList<T> list, IEqualityComparer<T> comparer)
        {
            Debug.Assert(comparer != null, "comparer must not be null");
            Debug.Assert(list != null, "list must not be null");

            _dictionary = new Dictionary<T, int>(comparer);

            foreach (T item in list)
            {
                if (!_dictionary.ContainsKey(item))
                {
                    _dictionary.Add(item, 1);
                }
            }
        }

        public bool Contains(T item)
        {
            Debug.Assert(item != null, "item must not be null");
            return _dictionary.ContainsKey(item);
        }

        public IEnumerable<T> Elements
        {
            get
            {
                return _dictionary.Keys;
            }
        }

        public void Add(T item)
        {
            Debug.Assert(!Contains(item), "adding a duplicate");
            _dictionary.Add(item, 1);
        }

        public void Include(T item)
        {
            Debug.Assert(item != null, "item must not be null");
            _dictionary[item] = 1;
        }

        public bool Remove(T item)
        {
            Debug.Assert(item != null, "item must not be null");
            return _dictionary.Remove(item);
        }

        public List<T> ToList()
        {
            var list = new List<T>();
            foreach (var item in _dictionary.Keys)
            {
                list.Add(item);
            }
            return list;
        }

        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }

        private Dictionary<T, int> _dictionary;
    }
}
