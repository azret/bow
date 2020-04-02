using System.Ai.Collections;
using System.Collections.Generic;
using System.Threading;

namespace System.Ai.Trainers {
    public class ff103 : Logistic, ITrainer {
        const int VERBOSITY = 13;

        const float lr = 0.3371f;

        const int POSITIVES = 3,
                NEGATIVES = 3,
            RADIUS = 7;

        IList<string[]> _data;

        public ff103(IModel model) {
            Model = model;
        }

        public void Build() {
            var data = new List<string[]>() {
                new string[] {
                    "white",
                    "red",
                    "green",
                    "black",
                    "orange",
                    "blue"
                },
                new string[] {
                    "apple",
                    "orange",
                    "fruit",
                    "eat",
                    "food"
                },
                new string[] {
                    "apple",
                    "google",
                    "microsoft",
                    "amazon",
                },
                new string[] {
                    "table",
                    "chair",
                    "room",
                    "door"
                },
                new string[] {
                    "C#",
                    "JavaScript",
                    "Java",
                    "C",
                    "C++"
                },
                new string[] {
                    "drink",
                    "coffee",
                    "tea",
                    "water",
                    "wine",
                    "beer",
                    "milk"
                },
                new string[] {
                    "cat",
                    "dog",
                    "cow",
                    "monkey",
                    "animal",
                    "human"
                },
                new string[] {
                    "language",
                    "english",
                    "french",
                    "spanish",
                    "german"
                },
                new string[] {
                    "city",
                    "new york",
                    "los angeles",
                    "paris",
                    "tokyo"
                }
            };
            _data = data;
            foreach (var b in _data) {
                foreach (var s in b) {
                    var w = Model.Push(s);
                    if (w == null) {
                        throw new OutOfMemoryException();
                    }
                    w++;
                }
            }
        }

        public IModel Model { get; }

        double loss = 0,
            cc = 0;

        string ITrainer.Progress => $"Loss: {(loss / cc)}";

        void ITrainer.Fit() {
            for (var k = 0; k < POSITIVES; k++) {
                var Sample = GetSample();
                foreach (var s in Sample) {
                    if (s == null)
                        continue;
                    var w = Model[s];
                    if (w != null) {
                        LearnFromPositiveSample(GetCoOccurrences(Sample, RADIUS), w);
                        for (var h = 0; h < NEGATIVES; h++) {
                            var Prime = GetSample();
                            if (Prime != null && Prime != Sample) {
                                LearnFromNegativeSample(GetCoOccurrences(Prime, RADIUS), w);
                            }
                        }
                    }
                }
            }
        }

        void LearnFromPositiveSample(Bag X, Tensor y) {
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
            }
        }

        void LearnFromNegativeSample(Bag X, Tensor y) {
            if (X.Count > 0) {
                const float NEGATIVE = 0.0f;
                var σ = Sgd(lr, ref loss, ref cc, out double err,
                    NEGATIVE, y.GetVector(), Model.Select(X));
                if (!double.IsNaN(err) && !double.IsInfinity(err)) {
                    loss += err;
                    cc++;
                    _LOG_(X, y.Id, NEGATIVE, σ, loss / cc);
                } else {
                    Console.WriteLine("NaN or Infinity detected...");
                }
            }
        }

        double Sgd(float lr, ref double loss, ref double cc, out double err, float t,
            Complex[] y, IEnumerable<Complex[]> X) {
            var σ = Sgd(
                y,
                X,
                lr,
                t);
            err = 0d;
            if (t >= 0.5) {
                err = σ > 0
                    ? - System.Math.Log(σ)
                    : 1;
            } else {
                err = σ >= 0 && σ < 1 
                    ? - System.Math.Log(1.0 - σ)
                    : 1;
            }
            return σ;
        }

        Bag GetCoOccurrences(string[] sample, int radius) {
            var X = new Bag();
            for (var n = 0; n < radius; n++) {
                string c = GetRandom(sample);
                if (c != null) {
                    X.Push(c);
                }
            }
            return X;
        }

        string[] GetSample() => _data[global::Random.Next(_data.Count)];

        string GetRandom(string[] sample) {
            for (var i = 0; i < sample.Length; i++) {
                var c = sample[global::Random.Next(sample.Length)];
                if (c != null) {
                    return c;
                }
            }
            return null;
        }

        int _verbOut;

        void _LOG_(IEnumerable<string> X, string id, float t, double o, double loss) {
            if ((_verbOut >= 0 && ((_verbOut % VERBOSITY) == 0))) {
                Console.Write($"[{Thread.CurrentThread.ManagedThreadId}] ({_verbOut}), Loss: {loss} : " +
                    $"p(y = {Convert.ToInt32(t)}, {id} | {string.Join<string>(", ", X)})" +
                    $" = {o}\r\n");
            }
            Interlocked.Increment(ref _verbOut);
        }
    }
}
