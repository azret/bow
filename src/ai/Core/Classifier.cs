namespace System.Ai {
    public class Classifier : Dot {
        readonly Complex[] β;
        public Classifier(string id, int hashCode, int dims, bool randomize)
            : base(id, hashCode) {
            if (dims < 0 || dims > 1024) {
                throw new ArgumentOutOfRangeException(nameof(dims));
            }
            β = new Complex[dims];
            if (randomize)
                Randomize();
        }
        public int Length { get => β.Length; }
        public Complex[] GetVector() => β;
        public void Randomize() {
            for (int i = 0; i < β.Length; i++) {
                β[i].Re = ((global::Random.Next() & 0xFFFF) / (float)(0xFFFF)) - 0.5f;
                β[i].Im = ((global::Random.Next() & 0xFFFF) / (float)(0xFFFF)) - 0.5f;
            }
        }
    }
}