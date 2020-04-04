using System;
using System.Ai;
using System.Ai.Trainers;
using System.Drawing;
using System.IO;

unsafe partial class Exec {
    public static bool Search(
        App app,
        string cliScript,
        Func<bool> IsTerminated) {
        app.Session.Model?.RunFullCosineSearch(cliScript, 128);
        return false;
    }

    public static bool Count(
        App app,
        Args args,
        Func<bool> IsTerminated) {
        string outputFileName = Path.ChangeExtension(args.SearchPath.TrimEnd('\\'), ".ml");

        Tensor[] sort;

        var cbow = new ContinuousBagOfWords(new System.Ai.Model(args.Capacity, args.Dims),
            args.SearchPath,
            args.SearchPattern,
            args.SearchOption,
            args.Orthography);

        cbow.Build();

        sort = cbow.Model.Sort();

        App.StartWin32UI(null,
                       Curves.DrawCounts, () =>
                            sort,
                       $"{args.SearchPath} - (Distribution)",
                            Color.White,
                       Properties.Resources.Oxygen,
                       new Size(623, 400));

        return false;
    }

    public static bool Fit(
        App app,
        Args args,
        Func<bool> IsTerminated) {
        string outputFileName = Path.ChangeExtension(args.SearchPath.TrimEnd('\\'), ".ml");
        Tensor[] sort;
        var cbow = new ContinuousBagOfWords(new System.Ai.Model(args.Capacity, args.Dims),
            args.SearchPath,
            args.SearchPattern,
            args.SearchOption,
            args.Orthography);
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
            args.Gens,
            IsTerminated);
        Model.Write(outputFileName,
            sort,
            app.Session.Model.Dims);
        Model.Dump(sort, app.Session.Model.Dims,
            Path.ChangeExtension(outputFileName, ".md"));
        return false;
    }
}