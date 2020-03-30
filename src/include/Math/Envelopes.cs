using System;

public static partial class Envelopes {
    public static double Welch(int k, int length) {
        var u = (k - length * 0.5) / (length * 0.5);
        return 1.0 - (u * u);
    }
    public static double Hann(int k, int length) {
        double sin = Math.Sin(
            (Math.PI * k) / length);
        return sin * sin;
    }
    public static double Parabola(int k, int length) {
        double u = 1.0f - (double)k
            / (length);
        return (float)Math.Pow(1.0f - (u * 2 - 1) * (u * 2 - 1), 3);
    }
    public static double Harris(int k, int length) {
        double a0 = 0.3635819;
        double a1 = 0.4891775;
        double a2 = 0.1365995;
        double a3 = 0.0106411;
        return a0 - a1 * Math.Cos((2 * Math.PI * k) / length)
                  + a2 * Math.Cos((4 * Math.PI * k) / length)
                  - a3 * Math.Cos((6 * Math.PI * k) / length);
    }
}