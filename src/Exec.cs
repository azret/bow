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
                "white",
                "red",
                "green",
                "black",
                "orange",
                "blue"
            },
            new Bag() {
                "apple",
                "orange",
                "fruit",
                "eat",
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
                "language",
                "english",
                "french",
                "spanish",
                "german"
            },
            new Bag() {
                "city",
                "new york",
                "los angeles",
                "paris",
                "tokyo"
            }
        };

        const int CAPACITY = 1048576,
            GENS = (int)1e6,
            DIMS = 37;

        var model = app.CurrentModel = new System.Ai.Model(CAPACITY, DIMS);

        var ff103 = new ff103(model, data);

        ff103.Build();

        app.StartWin32UI(null,
                       Curves.DrawCurves, () => ff103, "Bag of Words w/ Negative Sampling",
                       Color.White,
                       Properties.Resources.Oxygen,
                       new Size(623, 400));

        Trainer.Run(ff103,
            GENS,
            IsTerminated);

        string outputFileName = Path.ChangeExtension(typeof(App).Assembly.Location, ".md");

        Model.SaveToFile(model, model.Dims, outputFileName);

        return false;
    }
}