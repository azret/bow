namespace System.Ai {
    public interface ITrainer {
        IModel Model { get; }
        void Dojo();
        double Loss { get; }
    }
}