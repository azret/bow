namespace System.Ai {
    public unsafe class Tensor : Dot, ITensor, IComparable<Tensor> {
        public Tensor(string id, int hashCode, int dims, bool mix)
            : base(id, hashCode) {
            if (dims < 0 || dims > 1024) {
                throw new ArgumentOutOfRangeException(nameof(dims));
            }
            Vector = new Complex[dims];
            if (mix)
                Randomize();
        }
        float Score;
        public static implicit operator string(Tensor t) => t.Id;
        public static implicit operator float(Tensor t) => t.Score;
        public static Tensor operator ++(Tensor t) { t.Score++; return t; }
        public static Tensor operator --(Tensor t) { t.Score--; return t; }
        public float GetScore() => Score;
        public void SetScore(float value) => Score = value;
        Complex[] Vector;
        public static implicit operator Complex[] (Tensor t) => t.Vector;
        public Complex[] GetVector() => Vector;
        public void SetVector(Complex[] value) {
            if (value == null || value.Length != Vector.Length) {
                throw new ArgumentException();
            }
            for (int i = 0; i < Vector.Length; i++) {
                Vector[i] = value[i];
            }
        }
        public void Randomize() {
            for (int i = 0; i < Vector.Length; i++) {
#if !DEBUG
                β[i].Re = ((global::Random.Next() & 0xFFFF) / (float)(0xFFFF)) - 0.5f;
                β[i].Im = ((global::Random.Next() & 0xFFFF) / (float)(0xFFFF)) - 0.5f;
#else
                if (i % 2 == 0) {
                    Vector[i].Im = 0.5f;
                    Vector[i].Re = 0.5f;
                } else {
                    Vector[i].Im = -0.5f;
                    Vector[i].Re = -0.5f;
                }
#endif
            }
        }
        public int CompareTo(Tensor other) {
            return CompareTo(this, other);
        }
        public static int CompareTo(Tensor a, Tensor b) {
            if (a == null) {
                return b == null
                    ? 0
                    : -1;
            } else if (b == null) {
                return a == null
                    ? 0
                    : 1;
            } else {
                return (a.Score).CompareTo(b.Score);
            }
        }
    }
}