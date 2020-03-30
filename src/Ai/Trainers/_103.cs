using System.Ai.Collections;
using System.Collections.Generic;
using System.Threading;

namespace System.Ai.Trainers {
    public class _103 : Logistic, ITrainer<IList<Bag>> {
        Classifier _model;
        public _103(Classifier model) {
            _model = model;
        }
        public void OnTrain(IList<Bag> data) {
            int NEGATIVES = 3;
            int RADIUS = 7;
            const float lr = 0.0371f;
            Bag pos = data[global::Random.Next(data.Count)];
            foreach (var i in pos) {
                var wo = _model[i.Id];
                if (wo != null) {
                    var bag = new Bag();
                    for (var r = 0; r < RADIUS; r++) {
                        var wi = _model[pos.Get(global::Random.Next(pos.Capacity))];
                        if (wi != null && !wi.Id.Equals(wo.Id)) {
                            bag.Push(wi.Id);
                        }
                    }
                    if (bag.Count > 0) {
                        const float POSITIVE = 1.0f;
                        var y = Sgd(
                            wo.GetVector(),
                            _model.Select(bag),
                            lr,
                            POSITIVE);
                        _LOG_(bag, wo, POSITIVE, y, 0);
                    }
                }
                if (NEGATIVES > 0 && wo != null) {
                    Bag neg = data[global::Random.Next(data.Count)];
                    if (neg != pos) {
                        var bag = new Bag();
                        for (var r = 0; r < RADIUS; r++) {
                            var wi = _model[neg.Get(global::Random.Next(neg.Capacity))];
                            if (wi != null && !wi.Id.Equals(wo.Id)) {
                                bag.Push(wi.Id);
                            }
                        }
                        if (bag.Count > 0) {
                            const float NEGATIVE = 0.0f;
                            var y = Sgd(
                                wo.GetVector(),
                                _model.Select(bag),
                                lr,
                                NEGATIVE);
                            _LOG_(bag, wo, NEGATIVE, y, 0);
                        }
                    }
                }
            }
        }
        const int VERBOSITY = 13;
        int verbOut;
        void _LOG_(IEnumerable<string> input, Dot output, float target, double y, double err) {
            if (err == 0 || (verbOut >= 0 && ((verbOut % VERBOSITY) == 0))) {
                Console.Write($"[{Thread.CurrentThread.ManagedThreadId}] ({verbOut}) : " +
                    $"p(y = {Convert.ToInt32(target)}, {output.Id} | {string.Join<string>(", ", input)})" +
                    $" = {y}\r\n");
            }
            Interlocked.Increment(ref verbOut);
        }
    }
}
