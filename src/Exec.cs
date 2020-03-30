using System;
using System.Ai;
using System.Ai.Collections;
using System.Ai.Trainers;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

unsafe partial class Exec {
    public static bool Search(
        App app,
        string cliScript,
        Func<bool> IsTerminated) {

        app.CurrentModel.RunFullCosineSearch(cliScript, 17);

        return false;
    }

    public static bool Fit(
        App app,
        string dir,
        Func<bool> IsTerminated) {

        var data = new List<Bag>() {
            new Bag() {
                "apple",
                "orange",
                "fruit",
                "eat",
                "drink",
                "food"
            },
            new Bag() {
                "table",
                "chair",
                "room",
                "door"
            },
            new Bag() {
                "C#",
                "JavaScript",
                "Java",
                "C",
                "C++"
            },
            new Bag() {
                "drink",
                "coffee",
                "tea",
                "water",
                "wine",
                "beer",
                "milk"
            },
            new Bag() {
                "cat",
                "dog",
                "cow",
                "animal",
                "human"
            },
            new Bag() {
                "book",
                "school",
                "pen",
                "table",
                "chair"
            }
        };

        const int CAPACITY = 1048576,
            GENS = (int)1e6,
            DIMS = 7;

        var model = app.CurrentModel = new System.Ai.Model(CAPACITY, DIMS);

        foreach (var c in data) {
            foreach (var id in c) {
                var it = model.Push(id.Id);
                it.ζ.Re++;
            }
        }

        model.Randomize();

        app.StartWin32UI(null,
                       Curves.DrawCurves, () => model, "Bag of Words w/ Negative Sampling",
                       Color.White,
                       Properties.Resources.Oxygen,
                       new Size(623, 400));

        Runner.Run(new ff103(model), GENS,
            data,
            (loss) => { },
            IsTerminated);

        string outputFileName = Path.ChangeExtension(typeof(App).Assembly.Location, ".md");

        Model.SaveToFile(model.Sort(), model.Dims, outputFileName);

        return false;
    }
}