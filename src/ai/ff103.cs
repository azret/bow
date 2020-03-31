using System.Ai.Collections;
using System.Collections.Generic;
using System.Threading;

namespace System.Ai.Trainers {
    public class ff103 : Logistic, ITrainer {
        const float lr = 0.1371f;

        IList<Bag> _data;

        public ff103(IModel model, IList<Bag> data) {
            Model = model;
            _data = data;
        }

        public void Build() {
            foreach (var b in _data) {
                foreach (var s in b) {
                    var w = Model.Push(s.Id);
                    if (w == null) {
                        throw new OutOfMemoryException();
                    }
                    w.ζ.Re++;
                }
            }
        }

        public IModel Model { get; }

        double loss = 0,
            cc = 0;

        double ITrainer.Loss => loss / cc;

        void ITrainer.Execute() {
            Bag p = Pick(_data);
            LearnFromPosDistr(p, (y) => {
                for (var h = 0; h < NEGATIVES; h++) {
                    Bag n = Pick(_data);
                    if (n != null & n != p) {
                        LearnFromNegDistr(n, y);
                    }
                }
            });
        }

        const int NEGATIVES = 3,
            RADIUS = 7;

        void LearnFromPosDistr(Bag b, Action<Classifier> a) {
            foreach (var w in b) {
                if (w == null)
                    continue;
                var y = Model[w.Id];
                if (y != null) {
                    Bag X = GetContext(b, RADIUS, y.Id);
                    if (X.Count > 0) {
                        const float POSITIVE = 1.0f;
                        var o = Sgd(lr, ref loss, ref cc, out double err,
                            POSITIVE, y.GetVector(), Model.Select(X));
                        if (!double.IsNaN(err) && !double.IsInfinity(err)) {
                            loss += err;
                            cc++;
                            _LOG_(X, y.Id, POSITIVE, o, loss / cc);
                        } else {
                            Console.WriteLine("NaN or Infinity detected...");
                        }
                        a(y);
                    }
                }
            }
        }

        void LearnFromNegDistr(Bag b, Classifier y) {
            var X = GetContext(b, RADIUS, y.Id);
            if (X.Count > 0) {
                const float NEGATIVE = 0.0f;
                var o = Sgd(lr, ref loss, ref cc, out double err,
                    NEGATIVE, y.GetVector(), Model.Select(X));
                if (!double.IsNaN(err) && !double.IsInfinity(err)) {
                    loss += err;
                    cc++;
                    _LOG_(X, y.Id, NEGATIVE, o, loss / cc);
                } else {
                    Console.WriteLine("NaN or Infinity detected...");
                }
            }
        }

        double Sgd(float lr, ref double loss, ref double cc, out double err, float t,
            Complex[] y, IEnumerable<Complex[]> X) {
            var o = Sgd(
                y,
                X,
                lr,
                t);
            err = 0d;
            if (t >= 0.5) {
                err = -System.Math.Log(o);
            } else {
                err = -System.Math.Log(1.0 - o);
            }
            return o;
        }

        Bag GetContext(Bag ctx, int radius, string w) {
            var X = new Bag();
            for (var n = 0; n < radius; n++) {
                string c = Pick(ctx, w);
                if (c != null && !c.Equals(w)) {
                    X.Push(c);
                }
            }
            return X;
        }

        static Bag Pick(IList<Bag> data) => data[global::Random.Next(data.Count)];
        string Pick(Bag bag, string neg) {
            for (var i = 0; i < bag.Count; i++) {
                var c = bag.Get(global::Random.Next(bag.Capacity));
                if (c != null && !c.Equals(neg)) {
                    return c;
                }
            }
            return null;
        }

        const int VERBOSITY = 13;
        int verbOut;

        void _LOG_(IEnumerable<string> X, string id, float t, double o, double loss) {
            if ((verbOut >= 0 && ((verbOut % VERBOSITY) == 0))) {
                Console.Write($"[{Thread.CurrentThread.ManagedThreadId}] ({verbOut}), Loss: {loss} : " +
                    $"p(y = {Convert.ToInt32(t)}, {id} | {string.Join<string>(", ", X)})" +
                    $" = {o}\r\n");
            }
            Interlocked.Increment(ref verbOut);
        }
    }
}
