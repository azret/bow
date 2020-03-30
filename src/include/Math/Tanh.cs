using System;
/// <summary>
/// Tanh (ƒ(a) = (e²ᵃ - 1) / (e²ᵃ + 1))
/// </summary>
public class Tanh : IFunc {
    public static readonly IFunc Ω = New();
    public static IFunc New() {
        return new Tanh();
    }
    public override string ToString() {
        return "ƒ(a) = (e²ᵃ - 1) / (e²ᵃ + 1)";
    }
    public static double f(double a) {
        var exp = Math.Exp(2 * a);
        return (exp - 1f) / (exp + 1f);
    }
    public static double df(double a) {
        return (1 - a * a);
    }
    double IFunc.f(double a) {
        return f(a);
    }
    double IFunc.df(double a) {
        return df(a);
    }
}
