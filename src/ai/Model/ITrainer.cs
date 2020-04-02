namespace System.Ai {
    public interface ITrainer {
        IModel Model { get; }
        void Fit();
        string Progress { get; }
    }
}