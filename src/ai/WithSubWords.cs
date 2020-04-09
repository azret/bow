using System.Ai.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Orthography;
using System.Threading;

namespace System.Ai.Trainers {
    public class WithSubWords : Logistic, ITrainer {
        const int VERBOSITY = 13;

        const float lr = 0.13371f;

        const int
                NEGATIVES = 3,
            WINDOW = 3;

        Tensor[] NegDistr;

        public readonly IModel Model;
        public readonly IModel SubWords;

        string[] Files { get; }

        IOrthography Orthography;

        public WithSubWords(int capacity, int dims, string dir, string searchPattern,
            SearchOption searchOption, IOrthography orthography) {
            Model = new System.Ai.Model(capacity, dims);
            Orthography = orthography;
            Files = Tools.GetFiles(dir,
                searchPattern,
                searchOption
            ).ToArray();
        }

        public void Build() {
            Model.Clear();
            foreach (var file in Files) {
                Console.WriteLine($"Reading {Tools.GetShortPath(file)}...");
                string buff = File.ReadAllText(file);
                foreach (var tok in PlainText.ForEach(buff)) {
                    if (tok.Type == PlainTextTag.TEXT) {
                        var id = Orthography.GetKey(buff.Substring(
                            tok.StartIndex, tok.Length));
                        if (id != null && id.Length > 0) {
                            var y = Model.Push(id);
                            y++;
                        }
                    }
                }
            }
            Console.WriteLine($"Done.");
        }

        public static double PowScale(double score) {
            const double POW = 0.7351;
            const int Xmax = 739;
            if (score <= 1) {
                return 1;
            } else if (score >= Xmax) {
                return System.Math.Pow(Xmax, POW);
            } else {
                return System.Math.Pow(score, POW);
            }
        }

        static string MemSize(double byteCount) {
            string size = "0 Bytes";
            if (byteCount >= 1073741824.0)
                size = String.Format("{0:##.##}", byteCount / 1073741824.0) + " GB";
            else if (byteCount >= 1048576.0)
                size = String.Format("{0:##.##}", byteCount / 1048576.0) + " MB";
            else if (byteCount >= 1024.0)
                size = String.Format("{0:##.##}", byteCount / 1024.0) + " KB";
            else if (byteCount > 0 && byteCount < 1024.0)
                size = byteCount.ToString() + " Bytes";
            return size;
        }

        public void CreateNegativeDistribution(int size) {
            Console.Write($"\r\nCreating negative samples...\r\n");
            double norm = 0,
                    cc = 0;
            foreach (Tensor g in Model) {
                norm += PowScale(g.GetScore());
                cc++;
            }
            if (norm > 0) {
                norm = 1 / norm;
            }
            Tensor[] negDistr = new Tensor[0]; int count = 0;
            foreach (Tensor g in Model) {
                double samples = PowScale(g.GetScore()) * size * norm;
                for (int j = 0; j < samples; j++) {
                    if (count >= negDistr.Length) {
                        Array.Resize(ref negDistr, (int)((negDistr.Length + 7) * 1.75));
                    }
                    negDistr[count++] = g;
                }
            }
            Array.Resize(ref negDistr, count);
            global::Random.Shuffle(
                negDistr,
                negDistr.Length);
            NegDistr = negDistr;
            Console.Write($"\r\n  Size: {count}\r\n  Grams: {Model.Count}\r\n  Mem: {MemSize(count * Marshal.SizeOf<IntPtr>())}\r\n\r\n");
        }

        double loss = 0,
            cc = 0;

        string ITrainer.Progress => $"Loss: {(loss / cc)}";

        void ITrainer.Fit(Func<bool> HasCtrlBreak) {
            string[] lines = File.ReadAllLines(
                Files[global::Random.Next(Files.Length)]);

            for (var k = 0; k < lines.Length; k++) {
                if (HasCtrlBreak?.Invoke() == true) {
                    break;
                }

                var buff = lines[global::Random.Next(lines.Length)];

                PlainTextTag[] scan = PlainText.ForEach(
                    buff
                ).ToArray();

                for (var focus = 0; focus < scan.Length; focus++) {
                    if (scan[focus].Type == PlainTextTag.TEXT) {
                        var w = Orthography.GetKey(buff.Substring(
                           scan[focus].StartIndex, scan[focus].Length));
                        var y = Model[w];
                        if (y == null) {
                            continue;
                        }
                        var X = new Bag();
                        for (var start = focus - WINDOW - 1; start < focus + WINDOW + 1; start++) {
                            if (start != focus && start >= 0 && start < scan.Length &&
                                scan[start].Type == PlainTextTag.TEXT) {
                                string c = Orthography.GetKey(buff.Substring(
                                            scan[start].StartIndex, scan[start].Length));
                                var x = Model[c];
                                if (x != null) {
                                    X.Push(x.Id);
                                }
                            }
                        }
                        if (X.Count > 0) {
                            LearnFromPositiveSample(X, y);
                            for (int j = 0; j < NEGATIVES; j++) {
                                var Z = new Bag();
                                for (var start = 0; start < WINDOW; start++) {
                                    Z.Push(NegDistr[global::Random.Next(NegDistr.Length)].Id);
                                }
                                LearnFromNegativeSample(Z, y);
                            }
                        }
                    }
                }
            }
        }

        void LearnFromPositiveSample(Bag X, Tensor y) {
            if (X.Count > 0) {
                const float POSITIVE = 1.0f;
                var σ = Sgd(lr, ref loss, ref cc, out double err,
                    POSITIVE, y.GetVector(), Model.Select(X));
                if (!double.IsNaN(err) && !double.IsInfinity(err)) {
                    loss += err;
                    cc++;
                    _LOG_(X, y.Id, POSITIVE, σ, loss / cc);
                } else {
                    Console.WriteLine("NaN or Infinity detected...");
                }
            }
        }

        void LearnFromNegativeSample(Bag Z, Tensor y) {
            if (Z.Count > 0) {
                const float NEGATIVE = 0.0f;
                var σ = Sgd(lr, ref loss, ref cc, out double err,
                    NEGATIVE, y.GetVector(), Model.Select(Z));
                if (!double.IsNaN(err) && !double.IsInfinity(err)) {
                    loss += err;
                    cc++;
                    _LOG_(Z, y.Id, NEGATIVE, σ, loss / cc);
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
