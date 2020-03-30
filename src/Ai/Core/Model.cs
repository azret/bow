using System.Ai.Collections;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Ai {
    public partial class Model : IEnumerable<Classifier>, IModel {
        protected readonly Hash<Classifier> Hash;
        public int Dims { get; }
        private Model(int dims) {
            Dims = dims;
        }
        public Model(int capacity, int dims) : this(dims) {
            Hash = new Hash<Classifier>((id, hashCode) => new Classifier(id, hashCode, Dims), capacity);
        }
        public void Clear() => Hash.Clear();
        public IEnumerator<Classifier> GetEnumerator() => Hash.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Hash.GetEnumerator();
        public Classifier this[string id] => Hash[id];
        public Classifier Push(string id) => Hash.Push(id);
        public void Randomize() {
            foreach (Classifier wo in Hash) {
                Logistic.Randomize(wo.GetVector());
            }
        }
        double _loss;
        public void SetLoss(double loss) {
            _loss = loss;
        }
        public double GetLoss() {
            return _loss;
        }
        public IEnumerable<Classifier> Sort() {
            return Hash<Classifier>.Sort(Hash);
        }
        public IEnumerable<Complex[]> Select(IEnumerable<string> bag) {
            Complex[][] list = new Complex[bag.Count()][]; int n = 0;
            foreach (var i in bag) {
                Classifier w = Hash[i];
                if (w != null) {
                    if (n + 1 > list.Length) {
                        break;
                    }
                    list[n++] = w.GetVector();
                }
            }
            if (n != list.Length) {
                Array.Resize(ref list, n);
            }
            return list;
        }
    }

    public partial class Model {
        public static void SaveToFile(IEnumerable<Classifier> Model, int dims, string outputFilePath) {
            Console.Write($"\r\nSaving: {outputFilePath}...\r\n");
            using (var stream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None)) {
                int i = 0;
                string s;
                s = $"CBOW " +
                    $"| {dims}\r\n";
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                stream.Write(bytes,
                    0, bytes.Length);
                foreach (Classifier it in Model) {
                    if (it == null) {
                        continue;
                    }
                    var sz = new StringBuilder();
                    Complex[] w = it.GetVector();
                    if (w != null) {
                        for (var j = 0; j < w.Length; j++) {
                            if (sz.Length > 0) {
                                sz.Append(" ");
                            }
                            sz.Append(w[j]);
                        }
                    }
                    var score = it.ζ.Re;
                    if (sz.Length > 0) {
                        s = $"{it.Id} | {score} | {sz.ToString()}\r\n";
                    } else {
                        s = $"{it.Id} | {score}\r\n";
                    }
                    bytes = Encoding.UTF8.GetBytes(s);
                    stream.Write(bytes,
                        0, bytes.Length);
                    i++;
                }
            }
            Console.Write("\r\nReady!\r\n");
        }
        // public static Matrix<Word> LoadFromFile(string inputFilePath, int size, out string fmt, out int dims) {
        //     Matrix<Word> Model = new Matrix<Word>((id, hashCode) => new Word(id, hashCode), size);
        //     fmt = null;
        //     Console.Write($"\r\nReading: {inputFilePath}...\r\n\r\n");
        //     string[] lines = File.ReadAllLines(inputFilePath);
        //     dims = 0;
        //     for (int i = 0; i < lines.Length; i++) {
        //         string l = lines[i];
        //         if (string.IsNullOrWhiteSpace(l)) {
        //             continue;
        //         }
        //         if (i == 0) {
        //             ParseHeader(l, out fmt, out dims);
        //         } else {
        //             ParseWord(Model, fmt, l, dims);
        //         }
        //     }
        //     Console.Write($"Ready!\r\n\r\n");
        //     return Model;
        // }
        // static void ParseHeader(string sz, out string fmt, out int dims) {
        //     dims = -1;
        //     int i = 0, wordStart = i;
        //     while (i < sz.Length && (sz[i] != ' ' && sz[i] != '|' && sz[i] != '⁞')) {
        //         i++;
        //     }
        //     fmt = sz.Substring(wordStart, i - wordStart);
        //     while (i < sz.Length && (sz[i] == ' '
        //             || sz[i] == '|' || '⁞' == sz[i])) {
        //         i++;
        //     }
        //     if (fmt != "CLI" && fmt != "MEL" && fmt != "CBOW" && fmt != "MIDI") {
        //         throw new InvalidDataException();
        //     }
        //     int section = 0;
        //     for (; ; ) {
        //         wordStart = i;
        //         while (i < sz.Length && (sz[i] == '-' || sz[i] == '+' || sz[i] == 'E'
        //                 || sz[i] == '.' || char.IsDigit(sz[i]))) {
        //             i++;
        //         }
        //         if (i > wordStart) {
        //             string num = sz.Substring(wordStart, i - wordStart);
        //             switch (section) {
        //                 case 0:
        //                     dims = int.Parse(num);
        //                     break;
        //                 default:
        //                     throw new InvalidDataException();
        //             }
        //             while (i < sz.Length && (sz[i] == ' ' || sz[i] == '|' || '⁞' == sz[i])) {
        //                 if (sz[i] == '|' || sz[i] == '⁞') {
        //                     section++;
        //                 }
        //                 i++;
        //             }
        //         } else {
        //             break;
        //         }
        //     }
        // }
        // static void ParseWord(Matrix<Word> Model, string fmt, string sz, int dims) {
        //     int i = 0, wordStart = i;
        //     while (i < sz.Length && (sz[i] != '\t' && sz[i] != ' '
        //                     && sz[i] != '•' && sz[i] != '|' && sz[i] != '⁞')) {
        //         i++;
        //     }
        //     string w = sz.Substring(wordStart, i - wordStart);
        //     while (i < sz.Length && (sz[i] == '\t' || sz[i] == ' '
        //                     || sz[i] == '•' || sz[i] == '|' || sz[i] == '⁞')) {
        //         i++;
        //     }
        //     Word vec;
        //     if (fmt == "CBOW" || fmt == "MEL" || fmt == "CLI") {
        //         vec = Model.Push(w);
        //         if (vec.Vector == null) {
        //             vec.Alloc(dims);
        //         }
        //     } else {
        //         throw new InvalidDataException();
        //     }
        //     int section = 0,
        //             n = 0;
        //     if (fmt == "MIDI") {
        //         n = dims;
        //     }
        //     for (; ; ) {
        //         wordStart = i;
        //         while (i < sz.Length && (sz[i] == '±' || sz[i] == '-' || sz[i] == '+' || sz[i] == 'E'
        //                 || sz[i] == 'A' || sz[i] == 'B' || sz[i] == 'C'
        //                 || sz[i] == 'D' || sz[i] == 'F' || sz[i] == 'G'
        //                 || sz[i] == '#'
        //                 || sz[i] == '.' || char.IsDigit(sz[i]))) {
        //             i++;
        //         }
        //         var ImSign = +1;
        //         int len = (i - wordStart) - 1;
        //         while (wordStart + len > 0 && wordStart + len < sz.Length
        //                 && (sz[wordStart + len] == '-' || sz[wordStart + len] == '+' || sz[wordStart + len] == '±')) {
        //             if (sz[wordStart + len] == '-') {
        //                 ImSign = -1;
        //             }
        //             len--;
        //         }
        //         string Re = sz.Substring(wordStart, len + 1);
        //         string Im = null;
        //         if (i < sz.Length && (sz[i] == 'i')) {
        //             i++;
        //             wordStart = i;
        //             while (i < sz.Length && (sz[i] == '-' || sz[i] == '+' || sz[i] == 'E'
        //                     || sz[i] == 'A' || sz[i] == 'B' || sz[i] == 'C'
        //                     || sz[i] == 'D' || sz[i] == 'F' || sz[i] == 'G'
        //                     || sz[i] == '#'
        //                     || sz[i] == '.' || char.IsDigit(sz[i]))) {
        //                 i++;
        //             }
        //             Im = sz.Substring(wordStart, i - wordStart);
        //         }
        //         if (!string.IsNullOrWhiteSpace(Re)) {
        //             switch (section) {
        //                 case 0:
        //                     vec.Re = float.Parse(Re);
        //                     if (!string.IsNullOrWhiteSpace(Im)) {
        //                         vec.Im = ImSign * float.Parse(Im);
        //                     }
        //                     break;
        //                 case 1:
        //                     vec.Vector[n].Re = float.Parse(Re);
        //                     if (!string.IsNullOrWhiteSpace(Im)) {
        //                         vec.Vector[n].Im = ImSign * float.Parse(Im);
        //                     }
        //                     n++;
        //                     break;
        //                 default:
        //                     throw new InvalidDataException();
        //             }
        //             while (i < sz.Length && (sz[i] == '\t' || sz[i] == ' ' || sz[i] == '•' || sz[i] == '|' || sz[i] == '⁞')) {
        //                 if (sz[i] == '•' || sz[i] == '|' || sz[i] == '⁞') {
        //                     section++;
        //                 }
        //                 i++;
        //             }
        //         } else /* End of Line */ {
        //             if (n != dims) {
        //                 throw new InvalidDataException();
        //             }
        //             break;
        //         }
        //     }
        // }
    }
}
