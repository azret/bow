using System;
using System.Ai;
using System.Ai.Trainers;
using System.Drawing;
using System.IO;
using System.Text;

unsafe partial class Exec {
    public static bool Search(
        App app,
        string cliScript,
        Func<bool> IsTerminated) {
        app.CurrentModel?.RunFullCosineSearch(cliScript, 128);
        return false;
    }

    public static bool Count(
        App app,
        Args args,
        Func<bool> IsTerminated) {

        var Model = new System.Ai.Model(args.Capacity, 0);

        var Files = Tools.GetFiles(
            args.SearchPath,
            args.SearchPattern,
            args.SearchOption);

        foreach (var file in Files) {
            Console.WriteLine($"Reading {Tools.GetShortPath(file)}...");
            string buff = File.ReadAllText(file);
            foreach (var t in PlainText.ForEach(buff)) {
                if (t.Type == PlainTextTag.TEXT) {
                    var k = args.Orthography.GetKey(buff.Substring(
                        t.StartIndex, t.Length));
                    if (k != null && k.Length > 0) {
                        //var y = Model.Push(k);
                        //y++;
                        var ids = args.Orthography.Decompose(k);
                        if (ids != null) {
                            foreach (var id in ids) {
                                var sub = Model.Push(id);
                                sub++;
                            }
                        }
                    }
                }
            }
        }

        Tensor[] sort;

        sort = Model.Sort();

        App.StartWin32UI(null,
                       Curves.DrawCounts, () =>
                            sort,
                       $"{args.SearchPath} - Distribution",
                            Color.White,
                       Properties.Resources.Oxygen,
                       new Size(623, 400));


        Model.Dump(sort, Model.Dims, Path.ChangeExtension(args.OutputFileName, ".md"));

        return false;
    }

    public static bool Fit(
        App app,
        Args args,
        Func<bool> IsTerminated) {

        var model = new ContinuousBagOfWords(new System.Ai.Model(args.Capacity, args.Dims),
            args.SearchPath,
            args.SearchPattern,
            args.SearchOption,
            args.Orthography,
            args.lr,
            args.Negatives,
            args.Window);

        if (File.Exists(args.OutputFileName)) {
            model.Load(args.OutputFileName);
        } else {
            model.Build();
        }

        app.CurrentModel = model.Model;

        var sort = model.Model.Sort();

        App.StartWin32UI(null,
                       Curves.DrawCurves, () => 
                            new Tuple<Tensor[], string>(sort,
                                model.Progress),
                       $"{args.OutputFileName} - Continuous Bag of Words w/ Negative Sampling",
                            Color.White,
                       Properties.Resources.Oxygen,
                       new Size(623, 400));

        Trainer.Fit(model,
            args.Gens,
            IsTerminated);

        Model.Write(args.OutputFileName,
            sort,
            model.Model.Dims);

        Model.Dump(sort, model.Model.Dims,
            Path.ChangeExtension(args.OutputFileName, ".md"));

        return false;
    }
}