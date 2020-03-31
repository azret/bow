namespace System.Ai {
    public interface ITrainer {
        IModel Model { get; }
        void Execute();
        double Loss { get; }
    }
}