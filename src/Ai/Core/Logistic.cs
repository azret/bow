using System.Collections.Generic;
using System.Diagnostics;

namespace System.Ai {
    public class Logistic {
        public float[] Sum(IEnumerable<Complex[]> X) {
            float[] Re = null;
            int cc = 0;
            foreach (Complex[] x in X) {
                if (x == null) continue;
                if (Re == null) {
                    Re = new float[x.Length];
                }
                Debug.Assert(Re.Length == x.Length);
                for (var j = 0; j < Re.Length; j++) {
                    Re[j] += x[j].Re;
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

        public void Update(IEnumerable<Complex[]> X, float[] Δ) {
            foreach (Complex[] x in X) {
                if (x == null) continue;
                Debug.Assert(x.Length == Δ.Length);
                for (var j = 0; j < x.Length; j++) {
                    x[j].Re += Δ[j];
                }
            }
        }

        public double BinaryLogistic(Complex[] y, float[] X, float t, float lr, float[] Δ) {
            Debug.Assert(X != null);
            Debug.Assert(y != null);
            Debug.Assert(X.Length == y.Length && y.Length == Δ.Length);
            Debug.Assert(t >= 0 && t <= 1);
            Debug.Assert(lr >= 0 && lr <= 1);
            int len = X.Length;
            float o = 0.0f;
            for (int j = 0; j < X.Length; j++) {
                o += y[j].Im * X[j];
            }
            o = (float)SigQ.f(o);
            if (Δ != null) {
                float delta = lr * (t - o);
                if (float.IsNaN(delta) || float.IsInfinity(delta)) {
                    Console.WriteLine("NaN or Infinity detected...");
                    return delta;
                }
                for (int j = 0; j < len; j++) {
                    Δ[j] += y[j].Im * delta;
                    y[j].Im += X[j] * delta;
                }
            }
            return o;
        }

        public double Sgd(Complex[] y, IEnumerable<Complex[]> X, float lr, float t) {
            Debug.Assert(X != null);
            Debug.Assert(y != null);
            Debug.Assert(t >= 0 && t <= 1);
            float[] Δ = new float[y.Length];
            var o = BinaryLogistic(
                y,
                Sum(X),
                t,
                lr,
                Δ);
            Update(X, Δ);
            return o;
        }
    }
}