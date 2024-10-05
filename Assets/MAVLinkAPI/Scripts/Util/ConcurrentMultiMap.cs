using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MAVLinkAPI.Scripts.Util
{
    public class ConcurrentMultiMap<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, HashSet<TValue>> _dictionary = new();

        public void Add(TKey key, TValue value)
        {
            _dictionary.AddOrUpdate(key,
                _ => new HashSet<TValue> { value },
                (_, set) =>
                {
                    lock (set)
                    {
                        set.Add(value);
                        return set;
                    }
                });
        }

        public bool Remove(TKey key, TValue value)
        {
            if (_dictionary.TryGetValue(key, out var set))
                lock (set)
                {
                    if (set.Remove(value))
                    {
                        if (set.Count == 0)
                            _dictionary.TryRemove(key, out _);
                        return true;
                    }
                }

            return false;
        }

        public bool TryGetValues(TKey key, out IEnumerable<TValue> values)
        {
            if (_dictionary.TryGetValue(key, out var set))
            {
                values = set;
                return true;
            }

            values = null;
            return false;
        }
    }
}