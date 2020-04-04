using System;
using System.Ai;
using System.Ai.Trainers;
using System.Drawing;
using System.IO;
using System.Text.Orthography;

unsafe partial class Exec {
    const int CAPACITY = 1048576,
        GENS = (int)1e6,
             DIMS = 73;

    public static bool Search(
        App app,
        string cliScript,
        Func<bool> IsTerminated) {
        app.Session.Model?.RunFullCosineSearch(cliScript, 128);
        return false;
    }

    public static bool Fit(
        App app,
        string dir,
        Func<bool> IsTerminated) {
        string outputFileName = Path.ChangeExtension(dir.TrimEnd('\\'), ".ml");
        Tensor[] sort;
        var cbow = new ContinuousBagOfWords(new System.Ai.Model(CAPACITY, DIMS),
            dir,
            "*.la",
            SearchOption.AllDirectories,
            Latin.Instance);
        if (File.Exists(outputFileName)) {
            foreach (var t in Model.Read(outputFileName)) {
                var y = cbow.Model.Push(t.Id);
                y.SetScore(t.GetScore());
                y.SetVector(
                    t.GetVector());
            }
            cbow.CreateNegativeDistribution((int)1e7);
        } else {
            cbow.Build();
            cbow.CreateNegativeDistribution((int)1e7);
        }

        app.Session = cbow;
        sort = app.Session.Model.Sort();
        App.StartWin32UI(null,
                       Curves.DrawCurves, () => 
                            new Tuple<Tensor[], string>(sort,
                                app.Session.Progress),
                       $"{outputFileName} - (Continuous Bag of Words w/ Negative Sampling)",
                            Color.White,
                       Properties.Resources.Oxygen,
                       new Size(623, 400));

        Trainer.Fit(app.Session,
            GENS,
            IsTerminated);
        Model.Write(outputFileName,
            sort,
            app.Session.Model.Dims);
        Model.Dump(sort, app.Session.Model.Dims,
            Path.ChangeExtension(outputFileName, ".md"));
        return false;
    }

    public static bool Load(
        App app,
        string dir,
        Func<bool> IsTerminated) {
        string outputFileName = Path.ChangeExtension(dir.TrimEnd('\\'), ".ml");
        Tensor[] sort;
        var cbow = new ContinuousBagOfWords(new System.Ai.Model(CAPACITY, DIMS),
            dir,
            "*.la",
            SearchOption.AllDirectories,
            Latin.Instance);
        if (File.Exists(outputFileName)) {
            foreach (var t in Model.Read(outputFileName)) {
                var y = cbow.Model.Push(t.Id);
                y.SetScore(t.GetScore());
                y.SetVector(
                    t.GetVector());
            }
            app.Session = cbow;
            sort = app.Session.Model.Sort();
            App.StartWin32UI(null,
                           Curves.DrawCurves, () =>
                                new Tuple<Tensor[], string>(sort,
                                    app.Session.Progress),
                           $"{outputFileName} - (Continuous Bag of Words w/ Negative Sampling)",
                                Color.White,
                           Properties.Resources.Oxygen,
                           new Size(623, 400));
        }
        return false;
    }
}