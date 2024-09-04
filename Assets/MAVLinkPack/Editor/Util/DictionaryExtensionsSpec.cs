using System;
using System.Collections.Generic;
using MAVLinkPack.Scripts.Util;
using NUnit.Framework;

namespace MAVLinkPack.Editor.Util
{
    public class DictionaryExtensionsSpec
    {
        [Test]
        public void MergeWith_SumReducer_CombinesValuesCorrectly()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };
            var dict2 = new Dictionary<string, int> { { "b", 3 }, { "c", 4 }, { "d", 5 } };

            var result = dict1.Merge(dict2, (x, y) => x + y);

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(1, result["a"]);
            Assert.AreEqual(5, result["b"]);
            Assert.AreEqual(7, result["c"]);
            Assert.AreEqual(5, result["d"]);
        }

        [Test]
        public void MergeWith_MaxReducer_SelectsMaxValueCorrectly()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 5 }, { "c", 3 } };
            var dict2 = new Dictionary<string, int> { { "b", 2 }, { "c", 4 }, { "d", 5 } };

            var result = dict1.Merge(dict2, Math.Max);

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(1, result["a"]);
            Assert.AreEqual(5, result["b"]);
            Assert.AreEqual(4, result["c"]);
            Assert.AreEqual(5, result["d"]);
        }

        [Test]
        public void MergeWith_EmptyDictionaries_ReturnsEmptyDictionary()
        {
            var dict1 = new Dictionary<string, int>();
            var dict2 = new Dictionary<string, int>();

            var result = dict1.Merge(dict2, (x, y) => x + y);

            Assert.IsEmpty(result);
        }

        [Test]
        public void MergeWith_OneEmptyDictionary_ReturnsOtherDictionary()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };
            var dict2 = new Dictionary<string, int>();

            var result = dict1.Merge(dict2, (x, y) => x + y);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result["a"]);
            Assert.AreEqual(2, result["b"]);
        }

        [Test]
        public void MergeWith_DifferentValueTypes_WorksCorrectly()
        {
            var dict1 = new Dictionary<int, string> { { 1, "a" }, { 2, "b" } };
            var dict2 = new Dictionary<int, string> { { 2, "c" }, { 3, "d" } };

            var result = dict1.Merge(dict2, (x, y) => x + y);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a", result[1]);
            Assert.AreEqual("bc", result[2]);
            Assert.AreEqual("d", result[3]);
        }

        [Test]
        public void MergeWith_NullReducerFunction_ThrowsArgumentNullException()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 } };
            var dict2 = new Dictionary<string, int> { { "b", 2 }, { "a", 1 } };

            // var dd = dict1.Merge(dict2, null);

            Assert.Throws<ArgumentNullException>(() => dict1.Merge(dict2, null));
        }
    }
}