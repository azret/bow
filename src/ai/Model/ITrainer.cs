namespace System.Ai {
    public interface ITrainer {
        // IModel Model { get; }
        void Fit(Func<bool> HasCtrlBreak);
        string Progress { get; }
    }
}