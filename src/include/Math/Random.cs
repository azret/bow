public class Random {
    static long Seed = System.Environment.TickCount;
    public static int Next(int max = int.MaxValue) {
        Seed = Seed * 25214903917 + 11;
        int i = ((int)(Seed & 0x7FFFFFFF)) % max;
        return i;
    }
    public static void Randomize(double[] A) {
        for (int i = 0; i < A.Length; i++) {
            A[i] = ((global::Random.Next() & 0xFFFF) / (65536f) - 0.5f);
        }
    }
    public static void Shuffle<T>(T[] A, int length) {
        for (int i = 0; i < length; i++) {
            T j = A[i];
            int n = Next(length);
            A[i] = A[n];
            A[n] = j;
        }
    }
}