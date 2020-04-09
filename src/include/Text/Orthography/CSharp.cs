namespace System.Text.Orthography {
    public class CSharp : IOrthography {
        public CSharp() {
        }
        public static IOrthography Instance = new CSharp();
        public string[] Decompose(string s) {
            throw new NotImplementedException();
        }
        public string GetKey(string s) {
            StringBuilder _out = new StringBuilder();
            for (var i = 0; i < s.Length; i++) {
                char c = s[i];
                switch (c) {
                    case '_': _out.Append("_"); break;
                    default:
                        if (char.IsLetterOrDigit(c)) {
                            _out.Append(c);
                        }
                        break;
                }
            }
            if (_out.Length <= 1) {
                return null;
            }
            return _out.ToString();
        }
    }
}