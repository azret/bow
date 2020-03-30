using System.Ai.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Ai {
    public class Classifier : IEnumerable<Dot> {
        protected readonly Hash<Dot> Hash;
        public int Dims { get; }
        private Classifier(int dims) {
            Dims = dims;
        }
        public Classifier(int capacity, int dims) : this(dims) {
            Hash = new Hash<Dot>((id, hashCode) => new Dot(id, hashCode, Dims), capacity);
        }
        public void Clear() => Hash.Clear();
        public IEnumerator<Dot> GetEnumerator() => Hash.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Hash.GetEnumerator();
        public Dot this[string id] => Hash[id];
        public Dot Push(string id) => Hash.Push(id);
        public void Randomize() {
            foreach (Dot wo in Hash) {
                wo.Randomize();
            }
        }
        public IEnumerable<Complex[]> Select(IEnumerable<string> bag) {
            Complex[][] list = new Complex[bag.Count()][]; int n = 0;
            foreach (var i in bag) {
                Dot w = Hash[i];
                if (w != null) {
                    if (n + 1 > list.Length) {
                        break;
                    }
                    list[n++] = w.GetVector();
                }
            }
            if (n != list.Length) {
                Array.Resize(ref list, n);
            }
            return list;
        }
    }
}
