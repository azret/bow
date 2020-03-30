using System.Collections.Generic;

namespace System.Ai {
    public interface IModel {
        Classifier this[string id] { get; }
        int Dims { get; }
        void Clear();
        Classifier Push(string id);
        void Randomize();
        IEnumerable<Complex[]> Select(IEnumerable<string> bag);
    }
}