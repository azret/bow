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
        void DrawClassifier(Tensor w, Pen b) {
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
        DrawLoss(g, r, W.Progress);
        DrawPhase(g, r, t);
    }

    private static void DrawPaper(Graphics g,
        RectangleF r,
        byte Xscale = 12,
        byte Yscale = 12) {
        var PixelOffsetMode = g.PixelOffsetMode;
        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
        for (int x = 0; x < r.Width; x += Xscale) {
            if (x % (Xscale * 4) == 0) {
                g.DrawLine(Pens.Silver,
                    new PointF(x, 0),
                    new PointF(x, r.Height));
            }
            else if (x % Xscale == 0) {
                g.DrawLine(Pens.LightGray,
                    new PointF(x, 0),
                    new PointF(x, r.Height));
            }
        }
        for (int y = 0; y < r.Height; y += Yscale) {
            if (y % (Yscale * 4) == 0) {
                g.DrawLine(Pens.Silver,
                    new PointF(0, y),
                    new PointF(r.Width, y));
            }
            else if (y % Yscale == 0) {
                g.DrawLine(Pens.LightGray,
                    new PointF(0, y),
                    new PointF(r.Width, y));
            }
        }
        g.PixelOffsetMode = PixelOffsetMode;
    }

    private static void DrawVector(Graphics g,
        RectangleF r,
        Complex[] F,
        Pen brush) {
        float linf(float val, float from, float to) {
            return (val * (to / from));
        }
        var PixelOffsetMode = g.PixelOffsetMode;
        if (F != null) {
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            var dots = new List<PointF>();
            var pen = new Pen(brush.Color, 2f);
            for (int i = 0; i < F.Length; i++) {
                var ampl = SigF.f((F[i].Im + F[i].Re) * 0.5) - 0.5;
                // var ampl = ((F[i].Re + F[i].Im) / 2f);
                //var ampl = Tanh.f((F[i].Re + F[i].Im) / 2f);
                if (ampl < -1) {
                    ampl = -1;
                }
                if (ampl > +1) {
                    ampl = +1;
                }
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
                g.DrawCurve(
                    pen,
                    pts);
            }
            foreach (PointF p in pts) {
                float m = r.Height / 2f;
                g.FillEllipse(brush.Brush, p.X - 3,
                    p.Y - 3, 7, 7);
            }
            pen.Dispose();
        }
        g.PixelOffsetMode = PixelOffsetMode;
    }

    static void DrawLoss(Graphics g, RectangleF r, string loss) {
        string szLoss = $"{loss}";
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

    static Pen[] Colors = new Pen[]
    {
        Pens.OrangeRed,
        Pens.MidnightBlue,
        Pens.MediumVioletRed,
        // Pens.MediumSlateBlue,
        Pens.LightSeaGreen,
        Pens.LightSkyBlue,
        // Pens.LightSlateGray,
        // Pens.LightSteelBlue,
        // Pens.LimeGreen,
        Pens.Magenta,
        Pens.Maroon,
        // Pens.MediumAquamarine,
        // Pens.MediumBlue,
        // Pens.MediumOrchid,
        // Pens.MediumPurple,
        // Pens.Linen,
        // Pens.PaleGreen,
        // Pens.PaleTurquoise,
        Pens.PaleVioletRed,
        // Pens.SlateBlue,
        // Pens.SlateGray,
        // Pens.Snow,
        // Pens.SpringGreen,
        // Pens.SteelBlue,
        // Pens.Tan,
        // Pens.SkyBlue,
        // Pens.Teal,
        // Pens.Tomato,
        // Pens.Turquoise,
        Pens.Violet,
        // Pens.Wheat,
        // Pens.White,
        // Pens.WhiteSmoke,
        // Pens.Thistle,
        Pens.LightSalmon,
        // Pens.Silver,
        // Pens.SeaShell,
        // Pens.PapayaWhip,
        // Pens.PeachPuff,
        // Pens.Peru,
        // Pens.Pink,
        // Pens.Plum,
        // Pens.PowderBlue,
        // Pens.Sienna,
        Pens.Purple,
        // Pens.RosyBrown,
        // Pens.RoyalBlue,
        // Pens.SaddleBrown,
        // Pens.Salmon,
        // Pens.SandyBrown,
        // Pens.SeaGreen,
        // Pens.Red,
        // Pens.Yellow,
        // Pens.LightPink,
        // Pens.LightGreen,
        // Pens.DarkOrchid,
        Pens.DarkOrange,
        // Pens.DarkOliveGreen,
        Pens.DarkMagenta,
        // Pens.DarkKhaki,
        // Pens.DarkGreen,
        // Pens.DarkGray,
        // Pens.DarkGoldenrod,
        Pens.DarkCyan,
        // Pens.DarkBlue,
        // Pens.Cyan,
        // Pens.Crimson,
        // Pens.Cornsilk,
        // Pens.CornflowerBlue,
        // Pens.Coral,
        // Pens.Chocolate,
        // Pens.Chartreuse,
        // Pens.AliceBlue,
        // Pens.AntiqueWhite,
        // Pens.Aqua,
        // Pens.Aquamarine,
        // Pens.Azure,
        // Pens.Beige,
        Pens.DarkRed,
        // Pens.Bisque,
        // Pens.BlanchedAlmond,
        // Pens.Blue,
        // Pens.BlueViolet,
        // Pens.Brown,
        // Pens.BurlyWood,
        // Pens.CadetBlue,
        // Pens.Black,
        // Pens.DarkSalmon,
        // Pens.DarkSeaGreen,
        // Pens.DarkSlateBlue,
        // Pens.Honeydew,
        // Pens.HotPink,
        // Pens.IndianRed,
        // Pens.Indigo,
        // Pens.Ivory,
        // Pens.Khaki,
        // Pens.GreenYellow,
        // Pens.Lavender,
        // Pens.LawnGreen,
        // Pens.LemonChiffon,
        Pens.LightBlue,
        // Pens.LightCoral,
        // Pens.LightCyan,
        // Pens.LightGoldenrodYellow,
        // Pens.LavenderBlush,
        // Pens.LightGray,
        // Pens.Green,
        // Pens.Goldenrod,
        // Pens.DarkSlateGray,
        // Pens.DarkTurquoise,
        Pens.DarkViolet,
        // Pens.DeepPink,
        // Pens.DeepSkyBlue,
        // Pens.DimGray,
        // Pens.Gray,
        // Pens.DodgerBlue,
        // Pens.FloralWhite,
        // Pens.ForestGreen,
        // Pens.Fuchsia,
        // Pens.Gainsboro,
        // Pens.GhostWhite,
        // Pens.Gold,
        // Pens.Firebrick,
        // Pens.YellowGreen
    };
}