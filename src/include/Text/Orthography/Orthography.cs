namespace System.Text.Orthography {
    public interface IOrthography {
        string GetKey(string s);
        string[] Decompose(string s);
    }
}