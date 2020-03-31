using System.Collections.Generic;

namespace System.Ai {
    public interface IModel : IEnumerable<Classifier> {
        int Dims { get; }
        Classifier this[string id] { get; }
        Classifier Push(string id);
        void Clear();
        Complex[] Select(string id);
        IEnumerable<Complex[]> Select(IEnumerable<string> id);
    }
}