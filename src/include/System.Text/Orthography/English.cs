namespace System.Text.Orthography {
    public class English : IOrthography {
        public English() {
        }
        public static IOrthography Instance = new English();
        public string[] Decompose(string s) {
            throw new NotImplementedException();
        }
        public string GetKey(string s) {
            StringBuilder _out = new StringBuilder();
            for (var i = 0; i < s.Length; i++) {
                char c = s[i];
                switch (c) {
                    case '-': _out.Append("-"); break;
                    default:
                        if (char.IsLetter(c)) {
                            _out.Append(char.ToLowerInvariant(c));
                        }
                        break;
                }
            }
            return _out.ToString();
        }
    }
}