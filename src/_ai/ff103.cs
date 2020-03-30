using System.Ai.Collections;
using System.Collections.Generic;
using System.Threading;

namespace System.Ai.Trainers {
    public class ff103 : Logistic, ITrainer<IList<Bag>> {
        Model _model;
        public ff103(Model model) {
            _model = model;
        }
        public void OnTrain(IList<Bag> data) {
            int NEGATIVES = 3;
            int RADIUS = 7;
            const float lr = 0.0371f;
            double loss = 0,
                cc = 0;
            Bag pos = data[global::Random.Next(data.Count)];
            foreach (var i in pos) {
                var y = _model[i.Id];
                if (y != null) {
                    var X = new Bag();
                    for (var r = 0; r < RADIUS; r++) {
                        var x = _model[pos.Get(global::Random.Next(pos.Capacity))];
                        if (x != null && !x.Id.Equals(y.Id)) {
                            X.Push(x.Id);
                        }
                    }
                    if (X.Count > 0) {
                        const float POSITIVE = 1.0f;
                        var o = Sgd(
                            y.GetVector(),
                            _model.Select(X),
                            lr,
                            POSITIVE);
                        var err = -System.Math.Log(o);
                        if (!double.IsNaN(err) && !double.IsInfinity(err)) {
                            loss += err;
                            cc++;
                            _LOG_(X, y.Id, POSITIVE, o, loss / cc);
                        } else {
                            Console.WriteLine("NaN or Infinity detected...");
                        }
                    }
                }
                if (NEGATIVES > 0 && y != null) {
                    Bag neg = data[global::Random.Next(data.Count)];
                    if (neg != pos) {
                        var X = new Bag();
                        for (var r = 0; r < RADIUS; r++) {
                            var x = _model[neg.Get(global::Random.Next(neg.Capacity))];
                            if (x != null && !x.Id.Equals(y.Id)) {
                                X.Push(x.Id);
                            }
                        }
                        if (X.Count > 0) {
                            const float NEGATIVE = 0.0f;
                            var o = Sgd(
                                y.GetVector(),
                                _model.Select(X),
                                lr,
                                NEGATIVE);
                            var err = -System.Math.Log(1.0 - o);
                            if (!double.IsNaN(err) && !double.IsInfinity(err)) {
                                loss += err;
                                cc++;
                                _LOG_(X, y.Id, NEGATIVE, o, loss / cc);
                            } else {
                                Console.WriteLine("NaN or Infinity detected...");
                            }
                        }
                    }
                }
            }
            if (cc > 0) {
                _model.SetLoss(loss / cc);
            }
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
