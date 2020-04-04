namespace System.Ai.Trainers {
    public class Iris : Logistic, ITrainer {
        static Tuple<float[], string>[] Samples = {
            new Tuple<float[], string>(new float[] { 5.1f,3.5f,1.4f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.9f,3.0f,1.4f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.7f,3.2f,1.3f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.6f,3.1f,1.5f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.0f,3.6f,1.4f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.4f,3.9f,1.7f,0.4f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.6f,3.4f,1.4f,0.3f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.0f,3.4f,1.5f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.4f,2.9f,1.4f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.9f,3.1f,1.5f,0.1f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.4f,3.7f,1.5f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.8f,3.4f,1.6f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.8f,3.0f,1.4f,0.1f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.3f,3.0f,1.1f,0.1f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.8f,4.0f,1.2f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.7f,4.4f,1.5f,0.4f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.4f,3.9f,1.3f,0.4f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.1f,3.5f,1.4f,0.3f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.7f,3.8f,1.7f,0.3f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.1f,3.8f,1.5f,0.3f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.4f,3.4f,1.7f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.1f,3.7f,1.5f,0.4f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.6f,3.6f,1.0f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.1f,3.3f,1.7f,0.5f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.8f,3.4f,1.9f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.0f,3.0f,1.6f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.0f,3.4f,1.6f,0.4f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.2f,3.5f,1.5f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.2f,3.4f,1.4f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.7f,3.2f,1.6f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.8f,3.1f,1.6f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.4f,3.4f,1.5f,0.4f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.2f,4.1f,1.5f,0.1f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.5f,4.2f,1.4f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.9f,3.1f,1.5f,0.1f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.0f,3.2f,1.2f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.5f,3.5f,1.3f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.9f,3.1f,1.5f,0.1f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.4f,3.0f,1.3f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.1f,3.4f,1.5f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.0f,3.5f,1.3f,0.3f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.5f,2.3f,1.3f,0.3f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.4f,3.2f,1.3f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.0f,3.5f,1.6f,0.6f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.1f,3.8f,1.9f,0.4f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.8f,3.0f,1.4f,0.3f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.1f,3.8f,1.6f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 4.6f,3.2f,1.4f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.3f,3.7f,1.5f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 5.0f,3.3f,1.4f,0.2f }, "setosa"),
            new Tuple<float[], string>(new float[] { 7.0f,3.2f,4.7f,1.4f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.4f,3.2f,4.5f,1.5f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.9f,3.1f,4.9f,1.5f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.5f,2.3f,4.0f,1.3f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.5f,2.8f,4.6f,1.5f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.7f,2.8f,4.5f,1.3f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.3f,3.3f,4.7f,1.6f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 4.9f,2.4f,3.3f,1.0f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.6f,2.9f,4.6f,1.3f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.2f,2.7f,3.9f,1.4f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.0f,2.0f,3.5f,1.0f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.9f,3.0f,4.2f,1.5f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.0f,2.2f,4.0f,1.0f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.1f,2.9f,4.7f,1.4f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.6f,2.9f,3.6f,1.3f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.7f,3.1f,4.4f,1.4f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.6f,3.0f,4.5f,1.5f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.8f,2.7f,4.1f,1.0f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.2f,2.2f,4.5f,1.5f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.6f,2.5f,3.9f,1.1f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.9f,3.2f,4.8f,1.8f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.1f,2.8f,4.0f,1.3f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.3f,2.5f,4.9f,1.5f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.1f,2.8f,4.7f,1.2f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.4f,2.9f,4.3f,1.3f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.6f,3.0f,4.4f,1.4f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.8f,2.8f,4.8f,1.4f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.7f,3.0f,5.0f,1.7f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.0f,2.9f,4.5f,1.5f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.7f,2.6f,3.5f,1.0f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.5f,2.4f,3.8f,1.1f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.5f,2.4f,3.7f,1.0f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.8f,2.7f,3.9f,1.2f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.0f,2.7f,5.1f,1.6f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.4f,3.0f,4.5f,1.5f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.0f,3.4f,4.5f,1.6f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.7f,3.1f,4.7f,1.5f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.3f,2.3f,4.4f,1.3f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.6f,3.0f,4.1f,1.3f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.5f,2.5f,4.0f,1.3f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.5f,2.6f,4.4f,1.2f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.1f,3.0f,4.6f,1.4f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.8f,2.6f,4.0f,1.2f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.0f,2.3f,3.3f,1.0f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.6f,2.7f,4.2f,1.3f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.7f,3.0f,4.2f,1.2f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.7f,2.9f,4.2f,1.3f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.2f,2.9f,4.3f,1.3f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.1f,2.5f,3.0f,1.1f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 5.7f,2.8f,4.1f,1.3f }, "versicolor"),
            new Tuple<float[], string>(new float[] { 6.3f,3.3f,6.0f,2.5f }, "virginica"),
            new Tuple<float[], string>(new float[] { 5.8f,2.7f,5.1f,1.9f }, "virginica"),
            new Tuple<float[], string>(new float[] { 7.1f,3.0f,5.9f,2.1f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.3f,2.9f,5.6f,1.8f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.5f,3.0f,5.8f,2.2f }, "virginica"),
            new Tuple<float[], string>(new float[] { 7.6f,3.0f,6.6f,2.1f }, "virginica"),
            new Tuple<float[], string>(new float[] { 4.9f,2.5f,4.5f,1.7f }, "virginica"),
            new Tuple<float[], string>(new float[] { 7.3f,2.9f,6.3f,1.8f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.7f,2.5f,5.8f,1.8f }, "virginica"),
            new Tuple<float[], string>(new float[] { 7.2f,3.6f,6.1f,2.5f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.5f,3.2f,5.1f,2.0f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.4f,2.7f,5.3f,1.9f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.8f,3.0f,5.5f,2.1f }, "virginica"),
            new Tuple<float[], string>(new float[] { 5.7f,2.5f,5.0f,2.0f }, "virginica"),
            new Tuple<float[], string>(new float[] { 5.8f,2.8f,5.1f,2.4f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.4f,3.2f,5.3f,2.3f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.5f,3.0f,5.5f,1.8f }, "virginica"),
            new Tuple<float[], string>(new float[] { 7.7f,3.8f,6.7f,2.2f }, "virginica"),
            new Tuple<float[], string>(new float[] { 7.7f,2.6f,6.9f,2.3f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.0f,2.2f,5.0f,1.5f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.9f,3.2f,5.7f,2.3f }, "virginica"),
            new Tuple<float[], string>(new float[] { 5.6f,2.8f,4.9f,2.0f }, "virginica"),
            new Tuple<float[], string>(new float[] { 7.7f,2.8f,6.7f,2.0f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.3f,2.7f,4.9f,1.8f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.7f,3.3f,5.7f,2.1f }, "virginica"),
            new Tuple<float[], string>(new float[] { 7.2f,3.2f,6.0f,1.8f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.2f,2.8f,4.8f,1.8f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.1f,3.0f,4.9f,1.8f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.4f,2.8f,5.6f,2.1f }, "virginica"),
            new Tuple<float[], string>(new float[] { 7.2f,3.0f,5.8f,1.6f }, "virginica"),
            new Tuple<float[], string>(new float[] { 7.4f,2.8f,6.1f,1.9f }, "virginica"),
            new Tuple<float[], string>(new float[] { 7.9f,3.8f,6.4f,2.0f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.4f,2.8f,5.6f,2.2f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.3f,2.8f,5.1f,1.5f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.1f,2.6f,5.6f,1.4f }, "virginica"),
            new Tuple<float[], string>(new float[] { 7.7f,3.0f,6.1f,2.3f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.3f,3.4f,5.6f,2.4f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.4f,3.1f,5.5f,1.8f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.0f,3.0f,4.8f,1.8f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.9f,3.1f,5.4f,2.1f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.7f,3.1f,5.6f,2.4f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.9f,3.1f,5.1f,2.3f }, "virginica"),
            new Tuple<float[], string>(new float[] { 5.8f,2.7f,5.1f,1.9f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.8f,3.2f,5.9f,2.3f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.7f,3.3f,5.7f,2.5f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.7f,3.0f,5.2f,2.3f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.3f,2.5f,5.0f,1.9f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.5f,3.0f,5.2f,2.0f }, "virginica"),
            new Tuple<float[], string>(new float[] { 6.2f,3.4f,5.4f,2.3f }, "virginica"),
            new Tuple<float[], string>(new float[] { 5.9f,3.0f,5.1f,1.8f }, "virginica")
        };

        const int VERBOSITY = 13;

        const float lr = 0.3f;

        const int POSITIVES = 1,
                NEGATIVES = 3;

        public Iris() {
            Model = new System.Ai.Model(11, 4);
        }

        public void Build() {
            foreach (var s in Samples) {
                var w = Model.Push(s.Item2);
                if (w == null) {
                    throw new OutOfMemoryException();
                }
                w++;
            }
        }

        public IModel Model { get; }

        double _accuracy = 0;

        public string Progress => $"Accuracy: {(_accuracy)}%";

        void ITrainer.Fit(Func<bool> HasCtrlBreak) {
            for (var k = 0; k < POSITIVES; k++) {
                var Yes = GetRandomSample();
                var y = Model[Yes.Item2];
                if (y != null) {
                    const float POSITIVE = 1.0f;
                    Logistic.BinaryLogistic(y.GetVector(), Yes.Item1, POSITIVE, lr, null);
                    for (var h = 0; h < NEGATIVES; h++) {
                        var No = GetRandomSample();
                        while (No != null && string.Equals(No.Item2, Yes.Item2)) {
                            No = GetRandomSample();
                        }
                        if (No != null) {
                            const float NEGATIVE = 0.0f;
                            Logistic.BinaryLogistic(y.GetVector(), No.Item1, NEGATIVE, lr, null);
                        }
                    }
                }
            }
            double pct = 0,
                cc = 0d;
            foreach (var y in Model) {
                foreach (var s in Samples) {
                    var σ = Logistic.BinaryLogistic(y, s.Item1);
                    if (y.Id.Equals(s.Item2)) {
                        if (σ >= 0.5) {
                            pct++;
                        }
                    } else {
                        if (σ < 0.5) {
                            pct++;
                        }
                    }
                    cc++;
                }
            }
            _accuracy = Math.Round((pct / cc) * 100);
        }

        Tuple<float[], string> GetRandomSample() => Samples[global::Random.Next(Samples.Length)];
    }
}
