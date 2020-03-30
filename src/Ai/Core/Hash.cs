using System.Collections.Generic;
using System.Threading;
using System.Collections;

namespace System.Ai.Collections {
    public partial class Hash<T> : IEnumerable<T>
            where T : _Dot {
        protected Func<string, int, T> _factory;
        public Hash(Func<string, int, T> factory, int capacity) {
            if (capacity > 31048576 || capacity < 0) {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _hash = new T[capacity];
        }
        protected int _count;
        public int Count => _count;
        protected T[] _hash;
        public int Capacity { get => _hash.Length; }
        public T this[string id, int hashCode] {
            get {
                if (id == null || id.Length == 0) return /*!*/ null;
                int index = _Dot.LinearProbe(_hash, id, hashCode,
                    out T row, out int depth);
                if (index < 0) {
                    return null;
                }
                return row;
            }
        }
        public T this[string id] {
            get {
                if (id == null || id.Length == 0) return /*!*/ null;
                int index = _Dot.LinearProbe(_hash, id, _Dot.ComputeHashCode(id),
                    out T row, out int depth);
                if (index < 0) {
                    return /*!*/ null;
                }
                return row;
            }
        }
        protected int _depth;
        public int Depth { get => _depth; }
        public void Clear() {
            _hash = new T[_hash.Length];
            _count = 0; _depth = 0;
        }
        public T Push(string id) { return Push(id, _Dot.ComputeHashCode(id)); }
        public T Push(string id, int hashCode) {
            for (; ;) {
                int index = _Dot.LinearProbe(_hash, id, hashCode,
                    out T row, out int depth);
                if (index < 0) {
                    throw new OutOfMemoryException();
                }
                if (row != null) {
                    Diagnostics.Debug.Assert(row.Id.Equals(id));
                    return row;
                } else {
                    T pre = Interlocked.CompareExchange(
                        ref _hash[index],
                        row = _factory(id, hashCode), null);
                    if (pre != null) {
                        continue;
                    }
                    if (depth > _depth) {
                        _depth = depth;
                    }
                    Interlocked.Increment(ref _count);
                    return row;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator() {
            int count = 0;
            for (int i = 0; i < _hash.Length; i++) {
                var row = _hash[i];
                if (row != null) {
                    count++;
                    yield return row;
                }
            }
            if (count != _count) {
                throw new InvalidOperationException();
            }
        }
    }

    public partial class Hash<T> {
        public static T[] Sequence(Hash<T> M) {
            T[] list = new T[M.Count]; int n = 0;
            for (int i = 0; i < M._hash.Length; i++) {
                T row = M._hash[i];
                if (row != null) {
                    list[n++] = row;
                }
            }
            if (n != M._count) {
                throw new InvalidOperationException();
            }
            if (n < M._count) {
                Array.Resize(ref list, n);
            }
            return list;
        }
        // public static T[] Sort(Matrix<T> M, int skip = 0, int take = int.MaxValue) {
        //     T[] sort = Sequence(M);
        //     Array.Sort(
        //         sort,
        //         (a, b) => -a.Re.CompareTo(b.Re));
        //     if (take < sort.Length) {
        //         Array.Resize(ref sort, take);
        //     }
        //     return sort;
        // }
        // public static T[] Sort(Matrix<T> M, Comparison<T> comparison) {
        //     T[] sort = Sequence(M);
        //     Array.Sort(
        //         sort,
        //         comparison);
        //     return sort;
        // }
    }
}