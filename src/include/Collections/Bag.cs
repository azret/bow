using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;

namespace System.Ai.Collections {
    public class Bag<T> : Dot, IEnumerable<T> where T : Dot {
        protected Func<string, int, T> _factory;
        public Bag(Func<string, int, T> factory)
            : base() {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
        public Bag(string id, int hashCode, Func<string, int, T> factory)
            : base(id, hashCode) {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
        protected int _depth;
        public int Depth { get => _depth; }
        protected int _count;
        public int Count { get => _count; }
        protected T[] _hash;
        public bool Has(string id) {
            if (_hash == null) {
                return false;
            }
            int index = LinearProbe(
                    _hash,
                    id,
                    ComputeHashCode(id),
                    out T item,
                    out int depth);
            if (index >= 0) {
                return _hash[index] != null;
            }
            return false;
        }
        public T this[string id] {
            get {
                if (_hash == null) {
                    return null;
                }
                int index = LinearProbe(
                        _hash,
                        id,
                        ComputeHashCode(id),
                        out T item,
                        out int depth);
                if (index >= 0) {
                    return _hash[index];
                }
                return null;
            }
        }
        public int Capacity { get => _hash?.Length ?? 0; }
        public bool Push(string id) { return Push(id, out T item); }
        public bool Push(string id, out T item) {
            if (_hash == null) {
                _hash = new T[11];
            }
            int index, depth, hashCode = ComputeHashCode(id);
            for (; ; ) {
                index = LinearProbe(
                    _hash,
                    id,
                    hashCode,
                    out item,
                    out depth);
                if (index < 0) {
                    int sz = _hash.Length;
                    Grow();
                    if (sz >= _hash.Length) {
                        throw new OutOfMemoryException();
                    }
                } else {
                    break;
                }
            }
            if (item != null) {
                Debug.Assert(item.Id.Equals(id));
                return false;
            } else {
                var old = _hash[index];
                if (old != null) {
                    throw new InvalidOperationException();
                }
                _hash[index] = item = _factory(id, hashCode);
                if (depth > Depth) {
                    _depth = depth;
                }
                _count++;
                return true;
            }
        }
        void Grow(double factor = Math.E) {
            Debug.Assert(factor > 1);
            T[] hashTable = new T[(int)((_hash.Length + 7) * factor)];
            int count = 0; int depth = 0;
            for (int i = 0; i < _hash.Length; i++) {
                if (_hash[i] == null) continue;
                int index = LinearProbe(
                    hashTable,
                    _hash[i].Id,
                    _hash[i].HashCode,
                    out T item,
                    out int depth_);
                if (index < 0) {
                    throw new OutOfMemoryException();
                }
                if (depth_ > depth) {
                    depth = depth_;
                }
                Debug.Assert(hashTable[index] == null);
                hashTable[index] = _hash[i];
                count++;
            }
            _hash = hashTable;
            _count = count;
            _depth = depth;
        }
        public void Clear() {
            if (_hash != null) {
                for (int i = 0; i < _hash.Length; i++) {
                    _hash[i] = null;
                }
            }
            _count = 0; _depth = 0;
        }
        public bool Remove(string id) {
            if (_hash == null) {
                return false;
            }
            int index, depth, hashCode = ComputeHashCode(id);
            index = LinearProbe(
                _hash,
                id,
                hashCode,
                out T dot,
                out depth);
            if (dot != null) {
                var old = _hash[index];
                if (old != dot) {
                    throw new InvalidOperationException();
                }
                _hash[index] = null;
                _count--;
                return true;
            }
            return false;
        }
        // public T[] Sort() {
        //     int cc = 0;
        //     if (_hash != null) {
        //         T[] sort = new T[Count];
        //         for (int i = 0; i < _hash.Length; i++) {
        //             if (_hash[i] != null) {
        //                 sort[cc++] = _hash[i];
        //             }
        //         }
        //         Array.Resize(ref sort,
        //             cc);
        //         Array.Sort(sort, (a, b) => {
        //             return -(a.Re.CompareTo(b.Re));
        //         });
        //         Debug.Assert(Count == cc);
        //         return sort;
        //     }
        //     Debug.Assert(Count == cc);
        //     return null;
        // }
        // public T ArgMax() {
        //     T max = null;
        //     int cc = 0;
        //     if (_hash != null) {
        //         for (int i = 0; i < _hash.Length; i++) {
        //             if (_hash[i] != null) {
        //                 if (max == null || _hash[i].Re > max.Re) {
        //                     max = _hash[i];
        //                 }
        //                 cc++;
        //             }
        //         }
        //     }
        //     Debug.Assert(Count == cc);
        //     return max;
        // }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        IEnumerator<T> IEnumerable<T>.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<T> GetEnumerator() {
            int cc = 0;
            if (_hash != null) {
                for (int i = 0; i < _hash.Length; i++) {
                    if (_hash[i] != null) {
                        cc++;
                        yield return _hash[i];
                    }
                }
            }
            Debug.Assert(Count == cc);
        }
    }

    public class Bag : Bag<Dot>, IEnumerable<string>, ICollection<string> {
        public Bag()
            : base(Dot.Create) {
        }
        public Bag(string id, int hashCode)
            : base(id, hashCode, Dot.Create) {
        }
        bool ICollection<string>.IsReadOnly => false;
        public void Add(string id) {
            Push(id);
        }
        public string Get(int index) {
            return _hash[index]?.Id;
        }
        bool ICollection<string>.Contains(string id) {
            return Has(id);
        }
        void ICollection<string>.CopyTo(string[] array, int arrayIndex) {
            int n = 0;
            if (_hash != null) {
                for (int i = 0; i < _hash.Length; i++) {
                    if (_hash[i] != null) {
                        array[arrayIndex + n] = _hash[i].Id;
                        n++;
                    }
                }
            }
            Debug.Assert(Count == n);
        }
        IEnumerator<string> IEnumerable<string>.GetEnumerator() {
            int n = 0;
            if (_hash != null) {
                for (int i = 0; i < _hash.Length; i++) {
                    if (_hash[i] != null) {
                        n++;
                        yield return _hash[i].Id;
                    }
                }
            }
            Debug.Assert(Count == n);
        }
    }
}