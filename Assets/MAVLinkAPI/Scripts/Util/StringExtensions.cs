using System;
using System.Collections.Generic;
using System.Linq;

namespace MAVLinkAPI.Scripts.Util
{
    public class TextBlock
    {
        public readonly List<string> Lines;

        public TextBlock(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Lines = new List<string>();
                return;
            }

            Lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
        }

        public TextBlock Indent(int indentationLevel = 1, int spacesPerIndent = 4, bool indentFirstLine = true)
        {
            var indentation = new string(' ', indentationLevel * spacesPerIndent);

            var lines = Lines
                .Select((line, index) => index == 0 && !indentFirstLine ? line : indentation + line)
                .ToList();

            return new TextBlock(string.Join(Environment.NewLine, lines));
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Lines);
        }
    }

    public static class StringExtensions
    {
        public static TextBlock Block(this string text)
        {
            return new TextBlock(text);
        }

        public static string BlockSelect(this string text, Func<TextBlock, TextBlock> fn)
        {
            return fn(Block(text)).ToString();
        }
    }
}