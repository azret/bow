using System;
using System.Ai;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Gdi32;

unsafe partial class Curves {
    public static void DrawCurves(Graphics g, RectangleF r, float t, ITrainer W) {
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        DrawPaper(g, r);
        void DrawClassifier(Classifier w, Brush b) {
            if (w != null && w.GetVector() != null) {
                DrawVector(g, r, w.GetVector(), b);
            }
        }
        int i = 0;
        foreach (var w in W.Model) {
            if (i >= Colors.Length) {
                break;
            }
            DrawClassifier(w, Colors[i % Colors.Length]);
            i++;
        }
        DrawLoss(g, r, W.Loss);
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
            PointF[] pts = dots.ToArray();
            if (pts.Length > 1) {
                //g.DrawCurve(
                //    pen,
                //    pts);
            }
            foreach (PointF p in pts) {
                float m = r.Height / 2f;
                g.FillEllipse(brush, p.X - 3,
                    p.Y - 3, 7, 7);
            }
            pen.Dispose();
        }
        g.PixelOffsetMode = PixelOffsetMode;
    }

    static void DrawLoss(Graphics g, RectangleF r, double loss) {
        string szLoss = $"Loss: {loss}";
        if (szLoss != null) {
            var sz = g.MeasureString(szLoss, Gdi32.Font);
            g.DrawString(
                szLoss, Gdi32.Font, Brushes.DarkGray, r.Left + 8,
                 8);
        }
    }

    static void DrawPhase(Graphics g, RectangleF r, float phase) {
        string szPhase = $"{phase:n2}s";
        if (szPhase != null) {
            var sz = g.MeasureString(szPhase, Gdi32.Font);
            g.DrawString(
                szPhase, Gdi32.Font, Brushes.DarkGray, r.Right - 8 - sz.Width,
                 8);
        }
    }

    static Brush[] Colors = new Brush[]
    {
        Brushes.OrangeRed,
        Brushes.MidnightBlue,
        Brushes.MediumVioletRed,
        // Brushes.MediumSlateBlue,
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
        // Brushes.MediumPurple,
        // Brushes.Linen,
        // Brushes.PaleGreen,
        // Brushes.PaleTurquoise,
        Brushes.PaleVioletRed,
        // Brushes.SlateBlue,
        // Brushes.SlateGray,
        // Brushes.Snow,
        // Brushes.SpringGreen,
        // Brushes.SteelBlue,
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
        // Brushes.PowderBlue,
        // Brushes.Sienna,
        Brushes.Purple,
        // Brushes.RosyBrown,
        // Brushes.RoyalBlue,
        // Brushes.SaddleBrown,
        // Brushes.Salmon,
        // Brushes.SandyBrown,
        // Brushes.SeaGreen,
        // Brushes.Red,
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
        // Brushes.Black,
        // Brushes.DarkSalmon,
        // Brushes.DarkSeaGreen,
        // Brushes.DarkSlateBlue,
        // Brushes.Honeydew,
        // Brushes.HotPink,
        // Brushes.IndianRed,
        // Brushes.Indigo,
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