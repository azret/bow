using System.Ai.Collections;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Ai {
    public partial class Model : IEnumerable<Tensor>, IModel {
        readonly Hash<Tensor> _hash;
        private Model(int dims) {
            Dims = dims;
        }
        public Model(int capacity, int dims) : this(dims) {
            _hash = new Hash<Tensor>((id, hashCode) => new Tensor(id, hashCode, Dims, true), capacity);
        }
        public int Dims { get; }
        public int Count => _hash.Count;
        public void Clear() => _hash.Clear();
        public Tensor[] Sequence() => Hash<Tensor>.Sequence(_hash);
        public IEnumerator<Tensor> GetEnumerator() => _hash.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _hash.GetEnumerator();
        public Tensor this[string id] => _hash[id];
        public Tensor Push(string id) => _hash.Push(id);
        public Tensor[] Sort(int take = int.MaxValue) {
            Tensor[] sort = Sequence();
            Array.Sort(
                sort,
                (a, b) => {
                    int c = -a.CompareTo(b);
                    if (c == 0) {
                        c = a.Id.CompareTo(b.Id);
                    }
                    return c;
                });
            if (take < sort.Length) {
                Array.Resize(ref sort, take);
            }
            return sort;
        }
        public Complex[] Select(string id) => _hash[id]?.GetVector();
        public IEnumerable<Complex[]> Select(IEnumerable<string> id) {
            Complex[][] list = new Complex[System.Linq.Enumerable.Count(id)][]; int n = 0;
            foreach (var sz in id) {
                Tensor w = _hash[sz];
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
        public static void Dump(IEnumerable<Tensor> Model, int dims, string outputFilePath) {
            Console.Write($"\r\nSaving: {outputFilePath}...\r\n");
            using (var stream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None)) {
                int i = 0;
                string s;
                if (dims > 0) {
                    s = $"ML " +
                        $"| {dims}\r\n\r\n";
                } else {
                    s = $"ML\r\n\r\n";
                }
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                stream.Write(bytes,
                    0, bytes.Length);
                foreach (Tensor it in Model) {
                    if (it == null) {
                        continue;
                    }
                    var sz = new StringBuilder();
                    Complex[] w = it.GetVector();
                    if (w != null) {
                        for (var j = 0; j < Math.Min(w.Length, 7); j++) {
                            if (sz.Length > 0) {
                                sz.Append(" ");
                            }
                            sz.Append(w[j]);
                        }
                    }
                    float score = it.GetScore();
                    if (sz.Length > 0) {
                        s = $"**{it.Id}** | {score} | {sz.ToString()}\r\n";
                    } else {
                        s = $"**{it.Id}** | {score}\r\n";
                    }
                    bytes = Encoding.UTF8.GetBytes(s);
                    stream.Write(bytes,
                        0, bytes.Length);
                    i++;
                }
            }
            Console.Write("\r\nReady!\r\n");
        }
    }

    public partial class Model {
        static void WriteBytes(FileStream file, byte[] value) => file.Write(value, 0, value.Length);
        static void WriteLong(FileStream file, long value) => WriteBytes(file, BitConverter.GetBytes(value));
        static void WriteInt(FileStream file, int value) => WriteBytes(file, BitConverter.GetBytes(value));
        static void WriteShort(FileStream file, short value) => WriteBytes(file, BitConverter.GetBytes(value));
        static void WriteChars(FileStream file, string value) => WriteBytes(file, Encoding.ASCII.GetBytes(value));
        static void WriteFloat(FileStream file, float value) => WriteBytes(file, BitConverter.GetBytes(value));
        public static void Write(string outputFilePath, IEnumerable<Tensor> data, int dims) {
            Console.Write($"\r\nSaving: {outputFilePath}...\r\n");
            using (FileStream file = System.IO.File.Create(outputFilePath)) {
                WriteChars(file, "RIFF");
                long size = 0;
                WriteLong(file, size);
                WriteChars(file, "ML");
                if (dims > 1024 || dims < 0) {
                    throw new InvalidDataException();
                }
                int count = 0;
                WriteInt(file, count);
                WriteInt(file, dims);
                foreach (var t in data) {
                    if (t.Id != null && t.Id.Length > 256) {
                        throw new InvalidDataException();
                    }
                    file.WriteByte((byte)'§');
                    if (t.Id != null && t.Id.Length > 0) {
                        var buff
                            = Encoding.UTF8.GetBytes(t.Id);
                        WriteInt(file, buff.Length);
                        WriteBytes(file, buff);
                    } else {
                        WriteInt(file, 0);
                    }
                    WriteFloat(file, t.GetScore());
                    var vec = t.GetVector();
                    if (vec == null || vec.Length == 0) {
                        if (0 != dims) {
                            throw new InvalidDataException();
                        }
                        WriteInt(file, 0);
                    } else {
                        if (vec.Length != dims || vec.Length > 1024) {
                            throw new InvalidDataException();
                        }
                        WriteInt(file, vec.Length);
                        for (var j = 0; j < vec.Length; j++) {
                            WriteFloat(file, vec[j].Re);
                            WriteFloat(file, vec[j].Im);
                        }
                    }
                    count++;
                }
                size = file.Position;
                file.Seek(14,
                    SeekOrigin.Begin);
                WriteInt(file, count);
                file.Seek(4,
                    SeekOrigin.Begin);
                WriteLong(file, size);
                file.Seek(0,
                    SeekOrigin.End);
            }
            Console.Write("\r\nReady!\r\n");
        }
        static float ReadFloat(FileStream file) {
            var bytes = new byte[Marshal.SizeOf<float>()];
            file.Read(bytes, 0, Marshal.SizeOf<float>());
            return BitConverter.ToSingle(bytes, 0);
        }
        static int ReadInt(FileStream file) {
            var bytes = new byte[Marshal.SizeOf<int>()];
            file.Read(bytes, 0, Marshal.SizeOf<int>());
            return BitConverter.ToInt32(bytes, 0);
        }
        static short ReadShort(FileStream file) {
            var bytes = new byte[Marshal.SizeOf<short>()];
            file.Read(bytes, 0, Marshal.SizeOf<short>());
            return BitConverter.ToInt16(bytes, 0);
        }
        static long ReadLong(FileStream file) {
            var bytes = new byte[Marshal.SizeOf<long>()];
            file.Read(bytes, 0, Marshal.SizeOf<long>());
            return BitConverter.ToInt64(bytes, 0);
        }
        static string ReadChars(FileStream file, int len) {
            var bytes = new byte[len];
            file.Read(bytes, 0, len);
            return Encoding.ASCII.GetString(bytes);
        }
        public static IEnumerable<Tensor> Read(string fileName) {
            using (var file = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read)) {
                if (ReadChars(file, 4) != "RIFF") {
                    throw new InvalidDataException();
                }
                long nFileLength = ReadLong(file);
                if (ReadChars(file, 2) != "ML") {
                    throw new InvalidDataException();
                }
                int nCount = ReadInt(file),
                        dims = ReadInt(file);
                while (file.Position < file.Length) {
                    byte cSection = (byte)file.ReadByte();
                    switch (cSection) {
                        case (byte)'§':
                            var len = ReadInt(file);
                            if (len > 256 || len < 0) {
                                throw new InvalidDataException();
                            }
                            string id = null;
                            if (len > 0) {
                                var bytes = new byte[len];
                                file.Read(bytes,
                                    0, len);
                                id = Encoding.UTF8.GetString(bytes);
                            }
                            var score = ReadFloat(file);
                            var n = ReadInt(file);
                            if (n != dims) {
                                throw new InvalidDataException();
                            }
                            Complex[] vec = new Complex[n];
                            for (var j = 0; j < vec.Length; j++) {
                                vec[j].Re = ReadFloat(file);
                                vec[j].Im = ReadFloat(file);
                            }
                            var t = new Tensor(id,
                                Dot.ComputeHashCode(id), dims, false);
                            t.SetScore(score);
                            t.SetVector(vec);
                            yield return t;
                            break;
                        default:
                            throw new InvalidDataException();
                    }
                }
            }
        }
    }
}
