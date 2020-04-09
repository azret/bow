namespace System.Ai {
    public interface ITrainer {
        void Fit(Func<bool> HasCtrlBreak);
    }
}