using System.IO;
using System.Text.Orthography;

public class Args {
    public readonly int Capacity = 1048576,
        Gens = (int)1e6,
            Dims = 73;

    public readonly int
            Negatives = 3,
        Window = 3;

    /// <summary>
    /// Learning Rate
    /// </summary>
    public readonly float lr = 0.13371f;

    /// <summary>
    /// Parser
    /// </summary>
    public readonly IOrthography Orthography = null;

    /// <summary>
    /// Search Path
    /// </summary>
    public readonly string SearchPath = null;

    /// <summary>
    /// Search Pattern
    /// </summary>
    public readonly string SearchPattern = "*.txt";

    /// <summary>
    /// Search Options
    /// </summary>
    public readonly SearchOption SearchOption = SearchOption.AllDirectories;

    /// <summary>
    /// Model File Name
    /// </summary>
    public string OutputFileName => Path.ChangeExtension(SearchPath.TrimEnd('\\'), ".z");

    /// <summary>
    /// Creates an instance of the <see cref="Args"/>
    /// </summary>
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

    /// <summary>
    /// Creates an instance of the <see cref="Args"/>
    /// </summary>
    public Args(IOrthography orthography, string searchPath, string searchPattern) {
        Orthography = orthography;
        SearchPath = searchPath;
        SearchPattern = searchPattern;
    }

    /// <summary>
    /// Creates an instance of the <see cref="Args"/>
    /// </summary>
    public Args(Args args, string searchPath)
        : this(args.Capacity, args.Gens, args.Dims, args.Orthography, searchPath, args.SearchPattern, args.SearchOption) {
    }
}
