using System.Text.RegularExpressions;

namespace MAVLinkAPI.Scripts.Util
{
    public static class GlobPatternConverter
    {
        public static Regex GlobToRegex(string glob)
        {
            var regexPattern = "^";

            foreach (var c in glob)
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

            regexPattern += "$";
            return new Regex(regexPattern, RegexOptions.IgnoreCase);
        }
    }
}