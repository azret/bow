namespace System.Ai {
    public partial class _Dot : IEquatable<_Dot> {
        public static _Dot Factory(string id, int hashCode) {
            return new _Dot(id, hashCode);
        }
        public static int ComputeHashCode(string id) {
            uint h = 2166136261;
            unchecked {
                h = h ^ (uint)id.GetHashCode();
                h = h * 16777619;
                return (int)h & 0x7FFFFFFF;
            }
        }
        public static int LinearProbe<T>(T[] hash, string id, int hashCode,
            out T dot, out int depth) where T : _Dot {
            dot = null; depth = 0;
            if (hash == null) {
                return -1;
            }
            var cc = hash.Length;
            int i = hashCode % cc,
                         start = i;
            depth = 0;
            dot = hash[i];
            while (dot != null && (!(dot.GetHashCode() == hashCode && dot.Equals(id)))) {
                i = (i + 1) % cc;
                depth++;
                if (i == start) {
                    return -1;
                }
                dot = hash[i];
            }
            return i;
        }
        public _Dot() {
            HashCode = base.GetHashCode();
        }
        public _Dot(string id, int hashCode) {
            if (id == null || id.Length == 0) {
                throw new ArgumentNullException(nameof(id));
            }
            if (hashCode < 0) {
                throw new ArgumentOutOfRangeException(nameof(hashCode));
            }
            Id = id;
            HashCode = hashCode;
        }
        public readonly string Id;
        // public Complex Weight;
        // public float Re {
        //     get {
        //         return Weight.Re;
        //     }
        //     set {
        //         Weight.Re = value;
        //     }
        // }
        // public float Im {
        //     get {
        //         return Weight.Im;
        //     }
        //     set {
        //         Weight.Im = value;
        //     }
        // }
        // public Complex[] Vector;
        public readonly int HashCode;
        public override int GetHashCode() => HashCode;
        public override string ToString() { return Id; }
        public override bool Equals(object other) {
            if (other == null) { return this == null; }
            if (ReferenceEquals(other, this)) { return true; }
            if (other is string s) { return string.Equals(Id, s); }
            if (other is _Dot g) { return Equals(g); }
            return false;
        }
        public bool Equals(_Dot other) {
            if (other == null) { return this == null; }
            if (ReferenceEquals(other, this)) { return true; }
            return string.Equals(Id, other.Id);
        }
        // public static int CompareTo(Dot a, Dot b) {
        //     if (a == null) {
        //         return b == null
        //             ? 0
        //             : -1;
        //     } else if (b == null) {
        //         return a == null
        //             ? 0
        //             : 1;
        //     } else {
        //         return a.Re.CompareTo(b.Re);
        //     }
        // }
        // public int CompareTo(Dot other) {
        //     return CompareTo(this, other);
        // }
    }
    public class Dot : _Dot {
        readonly Complex[] Vector;
        public Dot(string id, int hashCode, int dims)
            : base(id, hashCode) {
            if (dims < 0 || dims > 1024) {
                throw new ArgumentOutOfRangeException(nameof(dims));
            }
            Vector = new Complex[dims];
        }
        public int Length { get => Vector.Length; }
        public Complex[] GetVector() => Vector;
        public void Randomize() {
            for (int i = 0; i < Vector.Length; i++) {
                Vector[i].Re = ((global::Random.Next() & 0xFFFF) / (65536f) - 0.5f);
                Vector[i].Im = ((global::Random.Next() & 0xFFFF) / (65536f) - 0.5f);
            }
        }
        public float Compute(float[] X) {
            var dot = 0f;
            for (int j = 0; j < X.Length; j++) {
                dot += ((Vector[j].Re + Vector[j].Im) / 2.0f) * X[j];
            }
            return (float)SigQ.f(dot);
        }
    }
}