using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.Ai {
    public static class Search {
        public static Tuple<string, float>[] Predict(this IEnumerable<Tensor> model, float[] X, int take) {
            Tuple<string, float>[] best = new Tuple<string, float>[take];
            foreach (Tensor wo in model) {
                int b = 0;
                for (int j = 0; j < best.Length; j++) {
                    if (best[j] == null) {
                        b = j;
                        break;
                    }
                    if (best[j].Item2 < best[b].Item2) {
                        b = j;
                    }
                }
                var score = Logistic.Dot(wo.GetVector(), X);
                if (best[b] == null || best[b].Item2 < score) {
                    best[b] = new Tuple<string, float>(wo.Id, score);
                }
            }
            return best;
        }
        public static void RunFullCosineSearch(this IModel model, string Q, int max) {
            if (model == null || string.IsNullOrWhiteSpace(Q)) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Model not loaded.\r\n");
                Console.ResetColor();
                Console.WriteLine("See '--load' command for more info...\r\n");
            }
            float[] X = new float[model.Dims];
            float norm = 0;
            var sign = +1;
            foreach (var tok in Q.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)) {
                if (tok == "+") {
                    sign = +1;
                } else if (tok == "-") {
                    sign = -1;
                } else {
                    var wi = model[tok];
                    if (wi != null) {
                        var vi = wi.GetVector();
                        Debug.Assert(vi.Length == X.Length);
                        for (var j = 0; j < X.Length; j++) {
                            X[j] += sign * ((vi[j].Re + vi[j].Im) / 2.0f);
                        }
                        norm++;
                    } else {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"'{tok}' not found.");
                        Console.ResetColor();
                    }
                }
            }
            if (norm > 0) {
                for (var j = 0; j < X.Length; j++) {
                    X[j] /= (float)norm;
                }
            }
            var output = Predict(model, X, max);
            Array.Sort(output,
                (a, b) => {
                    if (a == null && b == null) {
                        return 0;
                    } else if (a == null && b != null) {
                        return 1;
                    } else if (a != null && b == null) {
                        return -1;
                    } else {
                        return a.Item2.CompareTo(b.Item2);
                    }
                });
            Console.WriteLine();
            Console.WriteLine(" [" + string.Join(", ", X.Select(re => Math.Round(re, 4)).Take(7)) + "...]");
            Console.WriteLine();
            int len = 0;
            for (int i = output.Length - 1; i >= 0; i--) {
                var n = output[i];
                if (n != null) {
                    string str = n.Item1;
                    if (len + str.Length > 37  /* break like if does not fit */) {
                        Console.WriteLine(
                            output.Length <= 31
                                ? $" {str} : {n.Item2}"
                                : $" {str}");
                        len = 0;
                    } else {
                        Console.Write(
                            output.Length <= 31
                                ? $" {str} : {n.Item2}"
                                : $" {str}");
                        len += str.Length;
                    }
                }
            }
            Console.WriteLine();
        }
    }
}
