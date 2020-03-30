using System;
using System.Diagnostics;
/// <summary>
/// ƒ(a) = 1 / (1 + e⁻ᵃ)
/// </summary>
public class SigF : IFunc {
    public const int MAX = 4;
    public const int _TABLE_SIZE = 512;
    public static float[] _TABLE = new float[_TABLE_SIZE + 1];
    static void initSigmoid() {
        for (int i = 0; i < _TABLE_SIZE + 1; i++) {
            var x = (i * 2 * (float)MAX) / (float)_TABLE_SIZE - (double)MAX;
            _TABLE[i] = 1.0f / (1.0f + (float)Math.Exp(-x));
        }
    }
    static SigF() {
        initSigmoid();
    }
    public static readonly IFunc Ω = New();
    public static IFunc New() {
        return new SigF();
    }
    public override string ToString() {
        return "ƒ(a) = 1 / (1 + e⁻ᵃ)";
    }
    public static double f(double a) {
        if (a < -MAX) {
            return 0.0;
        } else if (a > MAX) {
            return 1.0;
        } else {
            int i = (int)((a + MAX) * _TABLE_SIZE / MAX / 2);
            return _TABLE[i];
        }
    }
    public static double df(double f) {
        return f * (1 - f);
    }
    double IFunc.f(double a) {
        return f(a);
    }
    double IFunc.df(double f) {
        return df(f);
    }
}

public class LogF : IFunc {
    public const int MAX = 4;
    public const int _TABLE_SIZE = 512;
    public static float[] _TABLE = new float[_TABLE_SIZE + 1];
    static void initSigmoid() {
        for (int i = 0; i < _TABLE_SIZE + 1; i++) {
            var x = (i * 2f * (float)MAX) / (float)_TABLE_SIZE - (float)MAX;
            int n = (int)(((x / MAX) + 1) / 2 * _TABLE_SIZE);
            int k = (int)((x + MAX) * _TABLE_SIZE / MAX / 2);
            Debug.Assert(n == i);
            Debug.Assert(k == i);
            Debug.Assert(k == n);
            var y = Math.Log(x);
            if (double.IsNaN(y) || double.IsInfinity(y)) {
                y = 0;
            }
            _TABLE[i] = (float)y;
        }
    }
    static LogF() {
        initSigmoid();
    }
    public static readonly IFunc Ω = New();
    public static IFunc New() {
        return new SigF();
    }
    public override string ToString() {
        return "ƒ(a) = 1 / (1 + e⁻ᵃ)";
    }
    public static double f(double a) {
        if (a < -MAX) {
            return 0.0;
        } else if (a > MAX) {
            return 1.0;
        } else {
            int i = (int)((a + MAX) * _TABLE_SIZE / MAX / 2);
            return _TABLE[i];
        }
    }
    public static double df(double f) {
        return f * (1 - f);
    }
    double IFunc.f(double a) {
        return f(a);
    }
    double IFunc.df(double f) {
        return df(f);
    }
}
