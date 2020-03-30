namespace System.Text.Orthography {
    public interface IOrthography {
        string GetKey(string textFragment);
    }

    public class CaseSensitive : IOrthography {
        public string GetKey(string textFragment) {
            StringBuilder _out = new StringBuilder();
            for (var i = 0; i < textFragment.Length; i++) {
                char c = textFragment[i];
                switch (c) {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        break;
                    default:
                        _out.Append(c);
                        break;
                }
            }
            return _out.ToString();
        }
    }
}