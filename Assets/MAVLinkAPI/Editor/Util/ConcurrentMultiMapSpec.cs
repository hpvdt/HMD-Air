using System.Collections.Generic;
using System.Threading.Tasks;
using MAVLinkAPI.Scripts.Util;
using NUnit.Framework;

namespace MAVLinkAPI.Editor.Util
{
    [TestFixture]
    [TestOf(typeof(ConcurrentMultiMap<,>))]
    public class ConcurrentMultiMapSpec
    {
        private ConcurrentMultiMap<string, int> _multiMap;

        [SetUp]
        public void Setup()
        {
            _multiMap = new ConcurrentMultiMap<string, int>();
        }

        [Test]
        public void Add_ShouldAddKeyValuePair()
        {
            _multiMap.Add("key1", 1);
            _multiMap.Add("key1", 2);
            _multiMap.Add("key2", 3);

            Assert.IsTrue(_multiMap.TryGetValues("key1", out var values1));
            CollectionAssert.AreEquivalent(new[] { 1, 2 }, values1);

            Assert.IsTrue(_multiMap.TryGetValues("key2", out var values2));
            CollectionAssert.AreEquivalent(new[] { 3 }, values2);
        }

        [Test]
        public void Remove_ShouldRemoveKeyValuePair()
        {
            _multiMap.Add("key1", 1);
            _multiMap.Add("key1", 2);

            Assert.IsTrue(_multiMap.Remove("key1", 1));
            Assert.IsFalse(_multiMap.Remove("key1", 3));

            Assert.IsTrue(_multiMap.TryGetValues("key1", out var values));
            CollectionAssert.AreEquivalent(new[] { 2 }, values);
        }

        [Test]
        public void TryGetValues_ShouldReturnCorrectValues()
        {
            _multiMap.Add("key1", 1);
            _multiMap.Add("key1", 2);

            Assert.IsTrue(_multiMap.TryGetValues("key1", out var values));
            CollectionAssert.AreEquivalent(new[] { 1, 2 }, values);

            Assert.IsFalse(_multiMap.TryGetValues("key2", out _));
        }

        [Test]
        public void ConcurrentOperations_ShouldBeThreadSafe()
        {
            var tasks = new List<Task>();
            for (var i = 0; i < 1000; i++)
            {
                var i1 = i;
                tasks.Add(Task.Run(() => _multiMap.Add("key", i1)));
                tasks.Add(Task.Run(() => { _multiMap.TryGetValues("key", out _); }));
                if (i % 2 == 0) tasks.Add(Task.Run(() => _multiMap.Remove("key", i1)));
            }

            Task.WaitAll(tasks.ToArray());

            Assert.DoesNotThrow(() => _multiMap.TryGetValues("key", out var finalValues));
        }
    }
}