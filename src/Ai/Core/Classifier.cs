namespace System.Ai {
    public class Classifier : Dot {
        readonly Complex[] W;
        public Classifier(string id, int hashCode, int dims)
            : base(id, hashCode) {
            if (dims < 0 || dims > 1024) {
                throw new ArgumentOutOfRangeException(nameof(dims));
            }
            W = new Complex[dims];
        }
        public int Length { get => W.Length; }
        public Complex[] GetVector() => W;
        public void Randomize() {
            for (int i = 0; i < W.Length; i++) {
                W[i].Re = ((global::Random.Next() & 0xFFFF) / (65536f) - 0.5f);
                W[i].Im = ((global::Random.Next() & 0xFFFF) / (65536f) - 0.5f);
            }
        }
        public float Compute(float[] X) {
            var dot = 0f;
            for (int j = 0; j < X.Length; j++) {
                dot += ((W[j].Re + W[j].Im) / 2.0f) * X[j];
            }
            return (float)SigQ.f(dot);
        }
    }
}