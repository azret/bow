using System.Collections.Generic;

namespace System.Ai {
    public interface IModel : IEnumerable<Tensor> {
        int Dims { get; }
        Tensor this[string id] { get; }
        Tensor Push(string id);
        void Clear();
        Tensor[] Sort(int take = int.MaxValue);
        Complex[] Select(string id);
        IEnumerable<Complex[]> Select(IEnumerable<string> id);
    }
}