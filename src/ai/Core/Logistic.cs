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

        public static float[] Sum(IEnumerable<Complex[]> X) {
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

        public static void Update(IEnumerable<Complex[]> X, float[] ΔRe, float[] ΔIm) {
            foreach (Complex[] x in X) {
                if (x == null) continue;
                if (ΔRe != null) {
                    Debug.Assert(x.Length == ΔRe.Length);
                }
                if (ΔIm != null) {
                    Debug.Assert(x.Length == ΔIm.Length);
                }
                for (var j = 0; j < x.Length; j++) {
                    if (ΔRe != null) {
                        x[j].Re += ΔRe[j];
                    }
                    if (ΔIm != null) {
                        x[j].Im += ΔIm[j];
                    }
                }
            }
        }

        public static double BinaryLogistic(Complex[] y, float[] X, float t, float lr, float[] Δ) {
            Debug.Assert(X != null);
            Debug.Assert(y != null);
            Debug.Assert(X.Length == y.Length && y.Length == Δ.Length);
            Debug.Assert(t >= 0 && t <= 1);
            Debug.Assert(lr >= 0 && lr <= 1);
            int len = X.Length;
            float σ = 0.0f;
            for (int j = 0; j < X.Length; j++) {
                σ += y[j].Im * X[j];
            }
            σ = (float)SigF.f(σ);
            if (Δ != null) {
                float δ = lr * (t - σ);
                if (float.IsNaN(δ) || float.IsInfinity(δ)) {
                    Console.WriteLine("NaN or Infinity detected...");
                    return δ;
                }
                for (int j = 0; j < len; j++) {
                    Δ[j] += y[j].Im * δ;
                    y[j].Im += X[j] * δ;
                }
            }
            return σ;
        }

        public static double Sgd(Complex[] y, IEnumerable<Complex[]> X, float lr, float t) {
            Debug.Assert(X != null);
            Debug.Assert(y != null);
            Debug.Assert(t >= 0 && t <= 1);
            float[] ΔRe = new float[y.Length];
            var σ = BinaryLogistic(
                y,
                Sum(X),
                t,
                lr,
                ΔRe);
            Update(X, ΔRe, null);
            return σ;
        }
    }
}