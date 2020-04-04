using System.IO;
using System.Text.Orthography;

public class Args {
    public readonly int Capacity = 1048576,
        Gens = (int)1e6,
            Dims = 73;

    public readonly IOrthography Orthography;
    public readonly string SearchPath;
    public readonly string SearchPattern;
    public readonly SearchOption SearchOption = SearchOption.AllDirectories;

    public Args(IOrthography orthography, string searchPath, string searchPattern) {
        Orthography = orthography;
        SearchPath = searchPath;
        SearchPattern = searchPattern;
    }

    private Args(int capacity, int gens, int dims, IOrthography orthography, string searchPath,
        string searchPattern, SearchOption searchOption) {
        Capacity = capacity;
        Gens = gens;
        Dims = dims;
        Orthography = orthography;
        SearchPath = searchPath;
        SearchPattern = searchPattern;
        SearchOption = searchOption;
    }

    public Args Create(string searchPath) {
        var args = new Args(Capacity, Gens, Dims,
            Orthography, searchPath, SearchPattern, SearchOption);
        return args;
    }
}
