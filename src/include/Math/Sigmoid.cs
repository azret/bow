using System;
/// <summary>
/// σ(ƒ(a) = 1 / (1 + e⁻ᵃ))
/// </summary>
public class Sigmoid : IFunc {
    public static readonly IFunc Ω = New();
    public static IFunc New() {
        return new Sigmoid();
    }
    public override string ToString() {
        return "ƒ(a) = 1 / (1 + e⁻ᵃ)";
    }
    public static double f(double a) {
        return 1 / (1 + Math.Exp(-a));
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
