namespace System.Text.Orthography {
    public class Latin : IOrthography {
        public Latin() {
        }
        public static IOrthography Instance = new Latin();
        public string GetKey(string s) {
            StringBuilder _out = new StringBuilder();
            for (var i = 0; i < s.Length; i++) {
                char c = s[i];
                switch (c) {
                    case 'v':
                        _out.Append("u");
                        break;
                    case 'V':
                        _out.Append("U");
                        break;
                    case 'Æ':
                        if (i + 1 < s.Length && char.IsUpper(s[i + 1])) {
                            _out.Append("AE");
                        } else {
                            _out.Append("Ae");
                        }
                        break;
                    case 'æ':
                        _out.Append("ae");
                        break;
                    case 'Œ':
                        if (i + 1 < s.Length && char.IsUpper(s[i + 1])) {
                            _out.Append("OE");
                        } else {
                            _out.Append("Oe");
                        }
                        break;
                    case 'œ':
                        _out.Append("oe");
                        break;
                    case 'Ā':
                        _out.Append("A");
                        break;
                    case 'ā':
                        _out.Append("a");
                        break;
                    case 'Ē':
                        _out.Append("E");
                        break;
                    case 'ē':
                        _out.Append("e");
                        break;
                    case 'Ī':
                        _out.Append("I");
                        break;
                    case 'ī':
                        _out.Append("i");
                        break;
                    case 'Ō':
                        _out.Append("O");
                        break;
                    case 'ō':
                        _out.Append("o");
                        break;
                    case 'Ū':
                        _out.Append("U");
                        break;
                    case 'ū':
                        _out.Append("u");
                        break;
                    case 'ȳ': _out.Append("y"); break;
                    case 'ɏ': _out.Append("y"); break;
                    case 'ă': _out.Append("a"); break;
                    case 'ĕ': _out.Append("e"); break;
                    case 'ŭ': _out.Append("u"); break;
                    case 'ĭ': _out.Append("i"); break;
                    case 'ŏ': _out.Append("o"); break;
                    case 'ä': _out.Append("a"); break;
                    case 'ë': _out.Append("e"); break;
                    case 'Ȳ': _out.Append("Y"); break;
                    case 'Ɏ': _out.Append("Y"); break;
                    case 'Ă': _out.Append("A"); break;
                    case 'Ĕ': _out.Append("E"); break;
                    case 'Ŭ': _out.Append("U"); break;
                    case 'Ĭ': _out.Append("I"); break;
                    case 'Ŏ': _out.Append("O"); break;
                    case 'Ä': _out.Append("A"); break;
                    case 'Ë': _out.Append("E"); break;
                    default:
                        if (char.IsLetter(c)) {
                            _out.Append(c);
                        }
                        break;
                }
            }
            string k = _out.ToString();
            switch (k) {
                case "utcumque":
                case "adusque":
                case "inique":
                case "qualiscumque":
                case "quotcumque":
                case "susque":
                case "quandocumque":
                case "antique":
                case "ubiquaque":
                case "plerique":
                case "simulatque":
                case "quantumcumque":
                case "adaeque":
                case "quoque":
                case "utercumque":
                case "denique":
                case "utroque":
                case "plerumque":
                case "utrimque":
                case "abusque":
                case "aeque":
                case "ubicumque":
                case "quousque":
                case "neque":
                case "que":
                case "atque":
                case "quantuscumque":
                case "quotusquisque":
                case "ubiquomque":
                case "oblique":
                case "quinque":
                case "quique":
                case "iamque":
                case "jamque":
                case "usque":
                case "quocumque":
                case "itaque":
                case "utrobique":
                case "quotuscunque":
                case "plerusque":
                case "ubique":
                case "namque":
                case "quisque":
                case "peraeque":
                case "undique":
                case "quacumque":
                case "absque":
                case "utique":
                case "quicumque":
                case "quantuluscumque":
                case "quotienscumque":
                case "quandoque":
                case "uterque":
                case "quomodocumque":
                case "utrubique":
                case "cumque":
                case "inque":
                    break;
                default:
                    if (k != "que" && k.EndsWith("que")) {
                        k = k.Substring(0, k.Length - "que".Length);
                    }
                    break;
            }
            return k.ToLowerInvariant();
        }
    }
}