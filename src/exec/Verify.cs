using System;
using System.Ai;
using System.IO;
using System.Linq;

unsafe partial class Verify {
    public static bool Run(
        App app,
        string dir,
        Func<bool> IsTerminated) {
        const int CAPACITY = 1048576,
                 DIMS = 3;
        string outputFileName = Path.GetTempFileName();
        var expected = new System.Ai.Model(CAPACITY, DIMS);
        var a = expected.Push("a");
        a.SetScore(+0.5f);
        a.SetVector(
            new Complex[] { new Complex(0, 1), new Complex(2, 3), new Complex(4, 5) });
        var b = expected.Push("z");
        b.SetScore(-0.5f);
        b.SetVector(
            new Complex[] { new Complex(-6, -7), new Complex(-8, -9), new Complex(-10, -11) });
        var sort = expected.Sort();
        Model.Dump(sort, expected.Dims, Path.ChangeExtension(outputFileName, ".md"));
        Model.Write(Path.ChangeExtension(outputFileName, ".model"),
            sort,
            expected.Dims);
        var loaded = Model.Read(Path.ChangeExtension(outputFileName, ".model")).ToArray();
        return false;
    }
}