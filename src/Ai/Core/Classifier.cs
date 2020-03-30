namespace System.Ai {
    public class Classifier : Dot {
        readonly Complex[] β;
        public Classifier(string id, int hashCode, int dims)
            : base(id, hashCode) {
            if (dims < 0 || dims > 1024) {
                throw new ArgumentOutOfRangeException(nameof(dims));
            }
            β = new Complex[dims];
        }
        public int Length { get => β.Length; }
        public Complex[] GetVector() => β;
    }
}