using System.Collections;
using System.Collections.Concurrent;

namespace Server.Utils
{
    public class ConcurrentSet<T> : IEnumerable<T> where T : notnull
    {
        // Choose byte to minimize mem usage
        private readonly ConcurrentDictionary<T, byte> dict = new();

        public bool TryAdd(T item) => dict.TryAdd(item, 0);

        public bool TryRemove(T item) => dict.TryRemove(item, out _);

        public bool ContainsKey(T item) => dict.ContainsKey(item);

        public IEnumerator<T> GetEnumerator() => dict.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
