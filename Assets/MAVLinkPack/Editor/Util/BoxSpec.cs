using System;
using MAVLinkPack.Scripts.Util;
using NUnit.Framework;

namespace MAVLinkPack.Editor.Util
{
    [TestFixture]
    [TestOf(typeof(Box))]
    public class BoxSpec
    {
        [Test]
        public void Constructor_WithValidValue_CreatesBox()
        {
            var box = new Box<int>(42);
            Assert.That(box.Value, Is.EqualTo(42));
        }

        [Test]
        public void Constructor_WithNullValue_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Box<string>(null));
        }

        [Test]
        public void ImplicitConversion_FromValue_CreatesBox()
        {
            Box<string> box = "test";
            Assert.That(box.Value, Is.EqualTo("test"));
        }

        [Test]
        public void Equals_SameValue_ReturnsTrue()
        {
            var box1 = new Box<int>(42);
            var box2 = new Box<int>(42);
            Assert.That(box1.Equals(box2), Is.True);
        }

        [Test]
        public void Equals_DifferentValue_ReturnsFalse()
        {
            var box1 = new Box<int>(42);
            var box2 = new Box<int>(24);
            Assert.That(box1.Equals(box2), Is.False);
        }

        [Test]
        public void Equals_NullObject_ReturnsFalse()
        {
            var box = new Box<int>(42);
            Assert.That(box.Equals(null), Is.False);
        }


        [Test]
        public void Equals_SameTuple_ReturnsTrue()
        {
            var t1 = (3, new Box<int>(42));
            var t2 = (3, new Box<int>(42));
            Assert.That(t1.Equals(t2), Is.True);
        }

        [Test]
        public void GetHashCode_SameValue_ReturnsSameHashCode()
        {
            var box1 = new Box<string>("test");
            var box2 = new Box<string>("test");
            Assert.That(box1.GetHashCode(), Is.EqualTo(box2.GetHashCode()));
        }

        [Test]
        public void EqualityOperator_SameValue_ReturnsTrue()
        {
            Box<int> box1 = 42;
            Box<int> box2 = 42;
            Assert.That(box1 == box2, Is.True);
        }

        [Test]
        public void InequalityOperator_DifferentValue_ReturnsTrue()
        {
            Box<int> box1 = 42;
            Box<int> box2 = 24;
            Assert.That(box1 != box2, Is.True);
        }
    }
}