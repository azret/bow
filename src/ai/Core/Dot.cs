namespace System.Ai {
    public partial class Dot : IEquatable<Dot> {
        public static Dot Create(string id, int hashCode) {
            return new Dot(id, hashCode);
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
            out T dot, out int depth) where T : Dot {
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
        protected Dot() {
            HashCode = base.GetHashCode();
        }
        public Dot(string id, int hashCode) {
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
        public readonly int HashCode;
        public override int GetHashCode() => HashCode;
        public override string ToString() { return Id; }
        public override bool Equals(object other) {
            if (other == null) { return this == null; }
            if (ReferenceEquals(other, this)) { return true; }
            if (other is string s) { return string.Equals(Id, s); }
            if (other is Dot g) { return Equals(g); }
            return false;
        }
        public bool Equals(Dot other) {
            if (other == null) { return this == null; }
            if (ReferenceEquals(other, this)) { return true; }
            return string.Equals(Id, other.Id);
        }
    }
}