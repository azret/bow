using System;
using System.Ai;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Win32.Gdi;

unsafe partial class Curves {
    public static void DrawCurves(Graphics g, RectangleF r, float t, IEnumerable<Dot> W) {
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        DrawPaper(g, r);

        void DrawClassifier(Dot w, Brush b) {
            if (w != null && w.GetVector() != null) {
                DrawVector(g, r, w.GetVector(), b);
            }
        }

        int i = 0;
        foreach (var w in W) {
            if (i >= Colors.Length) {
                break;
            }
            DrawClassifier(w, Colors[i]);
            i++;
        }

        DrawPhase(g, r, t);
    }

    private static void DrawPaper(Graphics g,
        RectangleF r,
        byte Xscale = 16,
        byte Yscale = 16) {
        var PixelOffsetMode = g.PixelOffsetMode;
        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
        var pen = Pens.LightGray;
        for (int x = 0; x < r.Width; x += Xscale) {
            if (x % Xscale == 0) {
                g.DrawLine(pen,
                    new PointF(x, 0),
                    new PointF(x, r.Height));
            }
        }
        for (int y = 0; y < r.Height; y += Yscale) {
            if (y % Yscale == 0) {
                g.DrawLine(pen,
                    new PointF(0, y),
                    new PointF(r.Width, y));
            }
        }
        g.PixelOffsetMode = PixelOffsetMode;
    }

    private static void DrawBars(Graphics g,
        RectangleF r,
        byte Xscale = 16,
        byte Yscale = 16,
        Func<int, float> GetAmplitude = null,
        Func<int, bool> GetPeak = null,
        Brush brush = null) {
        var PixelOffsetMode = g.PixelOffsetMode;
        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        var pen = Pens.LightGray;
        int i = 0;
        for (int x = 0; x < r.Width; x += Xscale) {
            if (x % Xscale == 0) {
                var ampl = GetAmplitude(i);
                if (Math.Abs(ampl) > 0.01) {
                    var h = ((int)((int)(r.Height / Yscale) / 2) * ampl) * Yscale;
                    var med = (int)((int)(r.Height / Yscale) / 2) * Yscale;
                    if (h > 0) {
                        g.FillRectangle(brush,
                            x, med - h, Xscale, h);
                        g.DrawRectangle(pen,
                            x, med - h, Xscale, h);
                        if (GetPeak(i)) {
                            g.FillEllipse(Brushes.Gray, x + (Xscale / 2) - 3,
                                med - h - Yscale / 2 - 3, 7, 7);
                        }
                    } else if (h < 0) {
                        h *= -1;
                        g.FillRectangle(brush,
                            x, med, Xscale, h);
                        g.DrawRectangle(pen,
                            x, med, Xscale, h);
                        if (GetPeak(i)) {
                            g.FillEllipse(Brushes.Gray, x + (Xscale / 2) - 3,
                                med + h + Yscale / 2 - 3, 7, 7);
                        }
                    }
                }
                i++;
            }
        }
        g.PixelOffsetMode = PixelOffsetMode;
    }

    private static void DrawVector(Graphics g,
        RectangleF r,
        Complex[] F,
        Brush brush) {
        float linf(float val, float from, float to) {
            return (val * (to / from));
        }
        var PixelOffsetMode = g.PixelOffsetMode;
        if (F != null) {
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            var dots = new List<PointF>();
            var pen = new Pen(brush, 2f);
            for (int i = 0; i < F.Length; i++) {
                var ampl = SigQ.f((F[i].Re + F[i].Im) * 0.5) - 0.5;
                if (ampl < -1 || ampl > +1) {
                    throw new IndexOutOfRangeException();
                }
                float m = r.Height / 2f;
                float y
                    = linf(-(float)ampl, 1f, m) + m;
                float x
                    = linf(i, F.Length - 1, r.Width);
                dots.Add(new PointF(x, y));
            }
            var pts = dots.ToArray();
            if (pts.Length > 1) {
                g.DrawCurve(
                    pen,
                    pts);
            }
            pen.Dispose();
        }
        g.PixelOffsetMode = PixelOffsetMode;
    }

    static void DrawLabels(Graphics g, RectangleF r, float phase, float hz, Complex[] fft, int startBin, int endBin) {
        string s = $"{fft.Length} at {hz}Hz";
        if (s != null) {
            var sz = g.MeasureString(s, Plot2D.Font);
            g.DrawString(
                s, Plot2D.Font, Brushes.DarkGray, r.Left + 8,
                 8);
        }
        s = $"{(startBin + 1) * (hz / fft.Length):n2}Hz";
        if (s != null) {
            var sz = g.MeasureString(s, Plot2D.Font);
            g.DrawString(
                s, Plot2D.Font, Brushes.DarkGray, r.Left + 8,
                  r.Bottom - 8 - sz.Height);
        }
        s = $"{(endBin + 1) * (hz / fft.Length):n2}Hz";
        if (s != null) {
            var sz = g.MeasureString(s, Plot2D.Font);
            g.DrawString(
                s, Plot2D.Font, Brushes.DarkGray, r.Right - 8 - sz.Width,
                  r.Bottom - 8 - sz.Height);
        }
        s = $"{((endBin + 1) / 2) * (hz / fft.Length):n2}Hz";
        if (s != null) {
            var sz = g.MeasureString(s, Plot2D.Font);
            g.DrawString(
                s, Plot2D.Font, Brushes.DarkGray, r.Left + r.Width / 2 - sz.Width / 2,
                  r.Bottom - 8 - sz.Height);
        }
    }

    static void DrawLabels(Graphics g, RectangleF r, float phase, float hz, float[] X) {
        string szRate = $"{X.Length} at {hz}Hz";
        if (szRate != null) {
            var sz = g.MeasureString(szRate, Plot2D.Font);
            g.DrawString(
                szRate, Plot2D.Font, Brushes.DarkGray, r.Left + 8,
                 8);
        }
    }

    static void DrawPhase(Graphics g, RectangleF r, float phase) {
        string szPhase = $"{phase:n2}s";
        if (szPhase != null) {
            var sz = g.MeasureString(szPhase, Plot2D.Font);
            g.DrawString(
                szPhase, Plot2D.Font, Brushes.DarkGray, r.Right - 8 - sz.Width,
                 8);
        }
    }

    static Brush[] Colors = new Brush[]
    {
        Brushes.OrangeRed,
        Brushes.MidnightBlue,
        Brushes.MediumVioletRed,
        Brushes.MediumSlateBlue,
        Brushes.LightSeaGreen,
        Brushes.LightSkyBlue,
        // Brushes.LightSlateGray,
        // Brushes.LightSteelBlue,
        // Brushes.LimeGreen,
        Brushes.Magenta,
        Brushes.Maroon,
        // Brushes.MediumAquamarine,
        // Brushes.MediumBlue,
        // Brushes.MediumOrchid,
        Brushes.MediumPurple,
        // Brushes.Linen,
        // Brushes.PaleGreen,
        // Brushes.PaleTurquoise,
        // Brushes.PaleVioletRed,
        // Brushes.SlateBlue,
        // Brushes.SlateGray,
        // Brushes.Snow,
        Brushes.SpringGreen,
        Brushes.SteelBlue,
        // Brushes.Tan,
        // Brushes.SkyBlue,
        // Brushes.Teal,
        // Brushes.Tomato,
        // Brushes.Turquoise,
        Brushes.Violet,
        // Brushes.Wheat,
        // Brushes.White,
        // Brushes.WhiteSmoke,
        // Brushes.Thistle,
        Brushes.LightSalmon,
        // Brushes.Silver,
        // Brushes.SeaShell,
        // Brushes.PapayaWhip,
        // Brushes.PeachPuff,
        // Brushes.Peru,
        // Brushes.Pink,
        // Brushes.Plum,
        Brushes.PowderBlue,
        // Brushes.Sienna,
        Brushes.Purple,
        // Brushes.RosyBrown,
        // Brushes.RoyalBlue,
        // Brushes.SaddleBrown,
        // Brushes.Salmon,
        // Brushes.SandyBrown,
        // Brushes.SeaGreen,
        Brushes.Red,
        // Brushes.Yellow,
        // Brushes.LightPink,
        // Brushes.LightGreen,
        // Brushes.DarkOrchid,
        Brushes.DarkOrange,
        // Brushes.DarkOliveGreen,
        Brushes.DarkMagenta,
        // Brushes.DarkKhaki,
        // Brushes.DarkGreen,
        // Brushes.DarkGray,
        // Brushes.DarkGoldenrod,
        Brushes.DarkCyan,
        // Brushes.DarkBlue,
        // Brushes.Cyan,
        // Brushes.Crimson,
        // Brushes.Cornsilk,
        // Brushes.CornflowerBlue,
        // Brushes.Coral,
        // Brushes.Chocolate,
        // Brushes.Chartreuse,
        // Brushes.AliceBlue,
        // Brushes.AntiqueWhite,
        // Brushes.Aqua,
        // Brushes.Aquamarine,
        // Brushes.Azure,
        // Brushes.Beige,
        Brushes.DarkRed,
        // Brushes.Bisque,
        // Brushes.BlanchedAlmond,
        // Brushes.Blue,
        // Brushes.BlueViolet,
        // Brushes.Brown,
        // Brushes.BurlyWood,
        // Brushes.CadetBlue,
        Brushes.Black,
        // Brushes.DarkSalmon,
        // Brushes.DarkSeaGreen,
        // Brushes.DarkSlateBlue,
        // Brushes.Honeydew,
        Brushes.HotPink,
        // Brushes.IndianRed,
        Brushes.Indigo,
        // Brushes.Ivory,
        // Brushes.Khaki,
        // Brushes.GreenYellow,
        // Brushes.Lavender,
        // Brushes.LawnGreen,
        // Brushes.LemonChiffon,
        Brushes.LightBlue,
        // Brushes.LightCoral,
        // Brushes.LightCyan,
        // Brushes.LightGoldenrodYellow,
        // Brushes.LavenderBlush,
        // Brushes.LightGray,
        // Brushes.Green,
        // Brushes.Goldenrod,
        // Brushes.DarkSlateGray,
        // Brushes.DarkTurquoise,
        Brushes.DarkViolet,
        // Brushes.DeepPink,
        // Brushes.DeepSkyBlue,
        // Brushes.DimGray,
        // Brushes.Gray,
        // Brushes.DodgerBlue,
        // Brushes.FloralWhite,
        // Brushes.ForestGreen,
        // Brushes.Fuchsia,
        // Brushes.Gainsboro,
        // Brushes.GhostWhite,
        // Brushes.Gold,
        // Brushes.Firebrick,
        // Brushes.YellowGreen
    };
}