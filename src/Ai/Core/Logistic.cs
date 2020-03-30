using System.Collections.Generic;
using System.Diagnostics;

namespace System.Ai {
    public class Logistic {
        public float[] Sum(IEnumerable<Complex[]> Wi) {
            float[] Re = null;
            int cc = 0;
            foreach (Complex[] wi in Wi) {
                if (wi == null) continue;
                if (Re == null) {
                    Re = new float[wi.Length];
                }
                Debug.Assert(Re.Length == wi.Length);
                for (var j = 0; j < Re.Length; j++) {
                    Re[j] += wi[j].Re;
                }
                cc++;
            }
            if (cc > 0) {
                for (var j = 0; j < Re.Length; j++) {
                    Re[j] /= cc;
                }
            }
            return Re;
        }

        public void Add(IEnumerable<Complex[]> Wi, float[] Δ) {
            foreach (Complex[] wi in Wi) {
                if (wi == null) continue;
                Debug.Assert(wi.Length == Δ.Length);
                for (var j = 0; j < wi.Length; j++) {
                    wi[j].Re += Δ[j];
                }
            }
        }

        public double BinaryLogistic(Complex[] wo, float[] X, float target, float lr, float[] Δ) {
            Debug.Assert(X != null);
            Debug.Assert(wo != null);
            Debug.Assert(X.Length == wo.Length && wo.Length == Δ.Length);
            Debug.Assert(target >= 0 && target <= 1);
            Debug.Assert(lr >= 0 && lr <= 1);
            int len = X.Length;
            float y = 0.0f;
            for (int j = 0; j < X.Length; j++) {
                y += wo[j].Im * X[j];
            }
            y = (float)SigQ.f(y);
            if (Δ != null) {
                float delta = lr * (target - y);
                if (float.IsNaN(delta) || float.IsInfinity(delta)) {
                    Console.WriteLine("NaN or Infinity detected...");
                    return delta;
                }
                for (int j = 0; j < len; j++) {
                    Δ[j] += wo[j].Im * delta;
                    wo[j].Im += X[j] * delta;
                }
            }
            return y;
        }

        public double Sgd(Complex[] wo, IEnumerable<Complex[]> Wi, float lr, float target) {
            Debug.Assert(Wi != null);
            Debug.Assert(wo != null);
            Debug.Assert(target >= 0 && target <= 1);
            float[] X = Sum(Wi);
            float[] Δ = new float[X.Length];
            var y = BinaryLogistic(
                wo,
                X,
                target,
                lr,
                Δ);
            Add(Wi, Δ);
            return y;
        }
    }
}