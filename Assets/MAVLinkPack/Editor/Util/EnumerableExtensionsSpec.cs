using MAVLinkPack.Scripts.Util;

namespace MAVLinkPack.Editor.Util
{
    using System.Linq;
    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    [TestOf(typeof(EnumerableExtensions))]
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void ZipWithNext_EmptySequence_ReturnsEmptySequence()
        {
            var result = Enumerable.Empty<int>().ZipWithNext(default).ToList();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ZipWithNext_SingleElement_ReturnsOneElementWithNullNext()
        {
            var result = new[] { 1 }.ZipWithNext(-1).ToList();
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0], Is.EqualTo((1, -1)));
        }

        [Test]
        public void ZipWithNext_MultipleElements_ReturnsCorrectPairs()
        {
            var result = new[] { 1, 2, 3 }.ZipWithNext(-1).ToList();
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0], Is.EqualTo((1, 2)));
            Assert.That(result[1], Is.EqualTo((2, 3)));
            Assert.That(result[2], Is.EqualTo((3, -1)));
        }

        [Test]
        public void ZipWithNext_ConsumesSequenceOnlyOnce()
        {
            var sequence = new SequenceCounter<int>(new[] { 1, 2, 3 });
            var result = sequence.ZipWithNext(-1).ToList();

            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(sequence.EnumerationCount, Is.EqualTo(1));
        }
    }

// Helper class to count sequence enumerations
    public class SequenceCounter<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _source;
        public int EnumerationCount { get; private set; }

        public SequenceCounter(IEnumerable<T> source)
        {
            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
        {
            EnumerationCount++;
            return _source.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}