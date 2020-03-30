using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial struct Complex {
        public float Re,
            Im;
        public Complex(float re, float im) {
            Re = re;
            Im = im;
        }
        public override string ToString() {
            if (Im > 0) {
                return string.Format("{0}+i{1}", Re, Im);
            } else if (Im < 0) {
                return string.Format("{0}-i{1}", Re, -Im);
            } else {
                return string.Format("{0}", Re);
            }
        }
        public float Magnitude {
            get {
                return (float)Math.Sqrt((Re * Re) + (Im * Im));
            }
        }
        public float Phase {
            get {
                return (float)Math.Atan2(Im, Re);
            }
        }
        public void Scale(float factor) {
            if (factor < -1 ||  factor > Math.E) {
                throw new ArgumentOutOfRangeException();
            }
            double vol = Magnitude,
                pH = Phase;
            double re = (Math.Cos(pH) * vol * factor),
                        im = (Math.Sin(pH) * vol * factor);
            if (Re != re) {
                Re = (float)re;
            }
            if (Im != im) {
                Im = (float)im;
            }
        }
    }
    public partial struct Complex {
        public static IEnumerable<Complex[]> ShortTimeFourierTransform(
            float[] data, int samples, Func<int, int, double> envelope) {
            var m = (int)Math.Log(samples, 2);
            if (samples <= 0 || Math.Pow(2, m) != samples) {
                throw new ArgumentException();
            }
            for (int e = 0; e < data.Length; e += samples) {
                Complex[] fft = new Complex[samples];
                if (e + samples <= data.Length) {
                    for (int s = 0; s < samples; s++) {
                        float A = envelope != null
                            ? (float)envelope(s, samples)
                            : 1.0f;
                        fft[s].Re = A *
                            data[s + e];
                    }
                }
                FastFourierTransform(
                    fft,
                    +1);
                yield return fft;
            }
        }
    }
    public partial struct Complex {
        public static Complex[] FFT(float[] X) {
            int k;
            Complex[] fft = new Complex[X.Length];
            for (k = 0; k < X.Length; k++) {
                fft[k].Re = X[k];
            }
            FastFourierTransform(fft, +1);
            return fft;
        }
        public static float[] InverseFFT(Complex[] fft) {
            int k;
            Complex[] f2 = new Complex[fft.Length];
            for (k = 0; k < fft.Length; k++) {
                f2[k] = fft[k];
            }
            float[] X = new float[f2.Length];
            FastFourierTransform(f2, -1);
            for (k = 0; k < f2.Length; k++) {
                X[k] = f2[k].Re;
            }
            return X;
        }
        public static Complex[] DFT(float[] x) {
            double pH = 2.0 * Math.PI / x.Length;
            int k, n;
            Complex[] dft = new Complex[x.Length];
            for (k = 0; k < x.Length; k++) {
                dft[k] = new Complex(0.0f, 0.0f);
                for (n = 0; n < x.Length; n++) {
                    dft[k].Re += x[n] * (float)Math.Cos(pH * k * n);
                    dft[k].Im -= x[n] * (float)Math.Sin(pH * k * n);
                }
                dft[k].Re /= x.Length;
                dft[k].Im /= x.Length;
            }
            return dft;
        }
        public static double[] InverseDFT(Complex[] dft) {
            double[] X = new double[dft.Length];
            double im,
                pH = 2.0 * Math.PI / dft.Length;
            for (int n = 0; n < dft.Length; n++) {
                im = X[n] = 0.0;
                for (int k = 0; k < dft.Length; k++) {
                    X[n] += dft[k].Re * Math.Cos(pH * k * n)
                          - dft[k].Im * Math.Sin(pH * k * n);
                    im += dft[k].Re * Math.Sin(pH * k * n)
                          + dft[k].Im * Math.Cos(pH * k * n);
                }
            }
            return X;
        }
        public static void FastFourierTransform(Complex[] fft, short dir) {
            int n = fft.Length,
                    m = (int)Math.Log(n, 2);
            if (Math.Pow(2, m) != n) {
                throw new InvalidOperationException();
            }
            int half = n >> 1,
                    j = 0;
            for (int i = 0; i < n - 1; i++) {
                if (i < j) {
                    float tx = fft[i].Re,
                        ty = fft[i].Im;
                    fft[i].Re = fft[j].Re;
                    fft[i].Im = fft[j].Im;
                    fft[j].Re = tx;
                    fft[j].Im = ty;
                }
                int k = half;
                while (k <= j) {
                    j -= k;
                    k >>= 1;
                }
                j += k;
            }
            float c1 = -1.0f,
                c2 = 0.0f;
            int l2 = 1;
            for (int l = 0; l < m; l++) {
                int l1 = l2;
                l2 <<= 1;
                float u1 = 1.0f,
                    u2 = 0.0f;
                for (j = 0; j < l1; j++) {
                    for (int i = j; i < n; i += l2) {
                        int i1 = i + l1;
                        float t1 = u1 * fft[i1].Re - u2 * fft[i1].Im,
                            t2 = u1 * fft[i1].Im + u2 * fft[i1].Re;
                        fft[i1].Re = fft[i].Re - t1;
                        fft[i1].Im = fft[i].Im - t2;
                        fft[i].Re += t1;
                        fft[i].Im += t2;
                    }
                    float z = u1 * c1 - u2 * c2;
                    u2 = u1 * c2 + u2 * c1;
                    u1 = z;
                }
                c2 = (float)Math.Sqrt((1.0 - c1) / 2.0);
                if (dir == 1) {
                    c2 = -c2;
                }
                c1 = (float)Math.Sqrt((1.0 + c1) / 2.0);
            }
            if (dir == 1) {
                for (int i = 0; i < n; i++) {
                    fft[i].Re /= n;
                    fft[i].Im /= n;
                }
            }
        }
    }
}