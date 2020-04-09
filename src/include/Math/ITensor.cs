namespace System.Ai {
    public unsafe interface ITensor {
        void SetVector(Complex[] vector);
        Complex[] GetVector();
    }
}