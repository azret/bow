namespace System.Text {
    using System.Collections.Generic;
    public struct PlainTextTag {
        public const int NONE = 0;
        public const int CRLF = 1;
        public const int WHITE = 2;
        public const int TEXT = 3;
        public int Type;
        public int X;
        public int Y;
        public int StartIndex;
        public int Length;
        public string TextFragment;
        public PlainTextTag(int type, int startIndex, int length, string textFragment)
            : this() {
            Type = type;
            StartIndex = startIndex;
            Length = length;
            TextFragment = textFragment;
        }
    }
    public static partial class PlainText {
        public static IEnumerable<PlainTextTag> ForEach(string textFragment) {
            return ForEach(textFragment,
                0,
                textFragment.Length,
                0);
        }
        public static IEnumerable<PlainTextTag> ForEach(string textFragment,
            int offset, int cc, int pad) {
            const int MAX = 137;
            bool IsSpecialChar(char c) {
                switch (c) {
                    case '_':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-':
                    case '+':
                        return true;
                }
                return false;
            }
            bool IsStartOfWord(char c) {
                return char.IsLetter(c) || IsSpecialChar(c);
            }
            bool IsStartOfWordOrBreakChar(char c) {
                return IsStartOfWord(c) || c == '\r'
                        || c == '\n';
            }
            bool IsPartOfWord(char c) {
                return IsStartOfWord(c) || IsSpecialChar(c);
            }
            var CRLF = new PlainTextTag(PlainTextTag.CRLF, 0, 0, null);
            int l = pad;
            while (l > 0) {
                yield return CRLF;
                l--;
            }
            var run = new PlainTextTag();
            for (int i = offset, start = offset; i < cc;) {
                char c = textFragment[i]; int len;
                switch (c) {
                    case '\r':
                    case '\n':
                        start = i;
                        i++;
                        run.X = 0;
                        if (c == '\n') {
                            run.Y++;
                        }
                        len = i - start;
                        if (len > 0) {
                            var t = new PlainTextTag(PlainTextTag.CRLF, start, len, textFragment) {
                                X = run.X,
                                Y = run.Y
                            };
                            yield return t;
                        }
                        break;
                    default:
                        if (IsStartOfWord(textFragment[i])) {
                            start = i;
                            i++;
                            int n = 0;
                            while (i < cc && IsPartOfWord(textFragment[i])) {
                                if (n > MAX) break;
                                n++;
                                i++;
                            }
                            len = i - start;
                            if (len > 0) {
                                var t = new PlainTextTag(PlainTextTag.TEXT, start, len, textFragment) {
                                    X = run.X,
                                    Y = run.Y
                                };
                                yield return t;
                            }
                        } else {
                            start = i;
                            i++;
                            int type = PlainTextTag.WHITE;
                            while (i < cc
                                    && (!IsStartOfWordOrBreakChar(textFragment[i]))) {
                                i++;
                            }
                            len = i - start;
                            if (len > 0) {
                                var t = new PlainTextTag(type, start, len, textFragment) {
                                    X = run.X,
                                    Y = run.Y
                                };
                                yield return t;
                            }
                        }
                        break;
                }
            }
            l = pad;
            while (l > 0) {
                yield return CRLF;
                l--;
            }
        }
    }
}