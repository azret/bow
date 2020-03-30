using System.Ai.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Ai {
    public class Model : IEnumerable<Classifier>, IModel {
        protected readonly Hash<Classifier> Hash;
        public int Dims { get; }
        private Model(int dims) {
            Dims = dims;
        }
        public Model(int capacity, int dims) : this(dims) {
            Hash = new Hash<Classifier>((id, hashCode) => new Classifier(id, hashCode, Dims), capacity);
        }
        public void Clear() => Hash.Clear();
        public IEnumerator<Classifier> GetEnumerator() => Hash.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Hash.GetEnumerator();
        public Classifier this[string id] => Hash[id];
        public Classifier Push(string id) => Hash.Push(id);
        public void Randomize() {
            foreach (Classifier wo in Hash) {
                wo.Randomize();
            }
        }
        double _loss;
        public void SetLoss(double loss) {
            _loss = loss;
        }
        public double GetLoss() {
            return _loss;
        }
        public IEnumerable<Complex[]> Select(IEnumerable<string> bag) {
            Complex[][] list = new Complex[bag.Count()][]; int n = 0;
            foreach (var i in bag) {
                Classifier w = Hash[i];
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
