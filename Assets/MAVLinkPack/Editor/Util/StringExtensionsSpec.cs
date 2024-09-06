using System.Collections.Generic;
using MAVLinkPack.Scripts.Util;
using NUnit.Framework;

namespace MAVLinkPack.Editor.Util
{
// NUnit Test Class

    [TestFixture]
    public class StringExtensionsSpec
    {
        [Test]
        public void IndentBlock_WithDefaultParameters_IndentsCorrectly()
        {
            var input = "Line 1\nLine 2\nLine 3";
            var expected = new List<string> { "    Line 1", "    Line 2", "    Line 3" };

            var result = input.Block().Indent().Lines;

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void IndentBlock_WithCustomIndentationLevel_IndentsCorrectly()
        {
            var input = "Line 1\nLine 2\nLine 3";
            var expected = new List<string> { "        Line 1", "        Line 2", "        Line 3" };

            var result = input.Block().Indent(2).Lines;

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void IndentBlock_WithCustomSpacesPerIndent_IndentsCorrectly()
        {
            var input = "Line 1\nLine 2\nLine 3";
            var expected = new List<string> { "  Line 1", "  Line 2", "  Line 3" };

            var result = input.Block().Indent(spacesPerIndent: 2).Lines;

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void IndentBlock_WithoutIndentingFirstLine_IndentsCorrectly()
        {
            var input = "Line 1\nLine 2\nLine 3";
            var expected = new List<string> { "Line 1", "    Line 2", "    Line 3" };

            var result = input.Block().Indent(indentFirstLine: false).Lines;

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void IndentBlock_WithEmptyString_ReturnsEmptyList()
        {
            var input = "";
            var result = input.Block().Indent().Lines;

            Assert.IsEmpty(result);
        }

        [Test]
        public void IndentBlock_WithNullString_ReturnsEmptyList()
        {
            var result = ((string)null).Block().Indent().Lines;

            Assert.IsEmpty(result);
        }

        [Test]
        public void IndentBlock_WithSingleLine_ReturnsListWithSingleIndentedLine()
        {
            var input = "Single line";
            var expected = new List<string> { "    Single line" };

            var result = input.Block().Indent().Lines;

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void IndentBlock_WithMixedLineEndings_HandlesCorrectly()
        {
            var input = "Line 1\r\nLine 2\rLine 3\nLine 4";
            var expected = new List<string> { "    Line 1", "    Line 2", "    Line 3", "    Line 4" };

            var result = input.Block().Indent().Lines;

            CollectionAssert.AreEqual(expected, result);
        }
    }
}