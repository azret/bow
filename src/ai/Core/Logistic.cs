using System.Collections.Generic;
using System.Diagnostics;

namespace System.Ai {
    public class Logistic {
        public static float Dot(Complex[] β, float[] X) {
            var y = 0f;
            for (int j = 0; j < X.Length; j++) {
                y += ((β[j].Re + β[j].Im) / 2.0f) * X[j];
            }
            return y;
        }

        public static float BinaryLogistic(Complex[] y, float[] X) {
            Debug.Assert(X != null);
            Debug.Assert(y != null);
            Debug.Assert(X.Length == y.Length);
            float σ = 0.0f;
            for (int j = 0; j < X.Length; j++) {
                σ += y[j].Re * X[j];
            }
            σ = (float)SigF.f(σ);
            return σ;
        }

        public static float BinaryLogistic(Complex[] y, float[] X, float t, float lr, float[] Δ) {
            Debug.Assert(X != null);
            Debug.Assert(y != null);
            Debug.Assert(X.Length == y.Length);
            if (Δ != null) {
                Debug.Assert(y.Length == Δ.Length);
            }
            Debug.Assert(t >= 0 && t <= 1);
            Debug.Assert(lr >= 0 && lr <= 1);
            float σ = BinaryLogistic(y, X);
            float δ = lr * (t - σ);
            if (float.IsNaN(δ) || float.IsInfinity(δ)) {
                Console.WriteLine("NaN or Infinity detected...");
                return δ;
            }
            for (int j = 0; j < X.Length; j++) {
                if (Δ != null) {
                    Δ[j] += y[j].Re * δ;
                }
                y[j].Re += X[j] * δ;
            }
            return σ;
        }

        public static double Sgd(Complex[] y, IEnumerable<Complex[]> X, float lr, float t) {
            Debug.Assert(X != null);
            Debug.Assert(y != null);
            Debug.Assert(t >= 0 && t <= 1);
            float[] Δ = new float[y.Length];
            var σ = BinaryLogistic(
                y,
                Sum(),
                t,
                lr,
                Δ);
            foreach (Complex[] x in X) {
                if (x == null) continue;
                if (Δ != null) {
                    Debug.Assert(x.Length == Δ.Length);
                }
                for (var j = 0; j < x.Length; j++) {
                    if (Δ != null) {
                        x[j].Im += Δ[j];
                    }
                }
            }
            return σ;
            float[] Sum() {
                float[] Im = null;
                int cc = 0;
                foreach (Complex[] x in X) {
                    if (x == null) continue;
                    if (Im == null) {
                        Im = new float[x.Length];
                    }
                    Debug.Assert(Im.Length == x.Length);
                    for (var j = 0; j < Im.Length; j++) {
                        Im[j] += x[j].Im;
                    }
                    cc++;
                }
                if (cc > 0) {
                    for (var j = 0; j < Im.Length; j++) {
                        Im[j] /= cc;
                    }
                }
                return Im;
            }
        }
    }
}