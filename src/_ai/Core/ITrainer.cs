namespace System.Ai {
    public interface ITrainer<T> {
        void OnTrain(T data);
    }
}