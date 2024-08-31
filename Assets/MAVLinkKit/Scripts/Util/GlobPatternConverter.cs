namespace MAVLinkKit.Scripts.Util
{
    using System.Text.RegularExpressions;

    public static class GlobPatternConverter
    {
        public static Regex GlobToRegex(string glob)
        {
            string regexPattern = "^";

            foreach (char c in glob)
            {
                switch (c)
                {
                    case '*':
                        regexPattern += ".*";
                        break;
                    case '?':
                        regexPattern += ".";
                        break;
                    case '.':
                    case '(':
                    case ')':
                    case '+':
                    case '|':
                    case '^':
                    case '$':
                    case '@':
                    case '%':
                        regexPattern += "\\" + c;
                        break;
                    default:
                        regexPattern += c;
                        break;
                }
            }

            regexPattern += "$";
            return new Regex(regexPattern, RegexOptions.IgnoreCase);
        }
    }
}
