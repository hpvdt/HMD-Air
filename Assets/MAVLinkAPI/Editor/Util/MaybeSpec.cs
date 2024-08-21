using System;
using MAVLinkAPI.Scripts.Util;
using NUnit.Framework;

namespace MAVLinkAPI.Editor.Util
{
    [TestFixture]
    public class MaybeTests
    {
        [Test]
        public void Some_CreatesValueWithContent()
        {
            var maybe = Maybe<int>.Some(42);
            Assert.That(maybe.HasValue, Is.True);
            Assert.That(maybe.Value, Is.EqualTo(42));
        }

        [Test]
        public void None_CreatesEmptyValue()
        {
            var maybe = Maybe<int>.None();
            Assert.That(maybe.HasValue, Is.False);
        }

        [Test]
        public void Value_ThrowsWhenEmpty()
        {
            var maybe = Maybe<int>.None();
            Assert.Throws<InvalidOperationException>(() => _ = maybe.Value);
        }

        [Test]
        public void ValueOrDefault_ReturnsValueWhenSome()
        {
            var maybe = Maybe<int>.Some(42);
            Assert.That(maybe.ValueOrDefault(0), Is.EqualTo(42));
        }

        [Test]
        public void ValueOrDefault_ReturnsDefaultWhenNone()
        {
            var maybe = Maybe<int>.None();
            Assert.That(maybe.ValueOrDefault(0), Is.EqualTo(0));
        }

        [Test]
        public void Match_ExecutesSomeActionWhenSome()
        {
            var maybe = Maybe<int>.Some(42);
            maybe.Match(
                value => Assert.That(value, Is.EqualTo(42)),
                () => Assert.Fail("Should not execute none action")
            );
        }

        [Test]
        public void Match_ExecutesNoneActionWhenNone()
        {
            var maybe = Maybe<int>.None();
            maybe.Match(
                _ => Assert.Fail("Should not execute some action"),
                () => Assert.Pass()
            );
        }

        [Test]
        public void Match_ReturnsCorrectResultWhenSome()
        {
            var maybe = Maybe<int>.Some(42);
            var result = maybe.Match(
                value => value * 2,
                () => 0
            );
            Assert.That(result, Is.EqualTo(84));
        }

        [Test]
        public void Match_ReturnsCorrectResultWhenNone()
        {
            var maybe = Maybe<int>.None();
            var result = maybe.Match(
                value => value * 2,
                () => 0
            );
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Map_TransformsValueWhenSome()
        {
            var maybe = Maybe<int>.Some(42);
            var result = maybe.Map(x => x.ToString());
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo("42"));
        }

        [Test]
        public void Map_ReturnsNoneWhenNone()
        {
            var maybe = Maybe<int>.None();
            var result = maybe.Map(x => x.ToString());
            Assert.That(result.HasValue, Is.False);
        }

        [Test]
        public void Bind_TransformsValueWhenSome()
        {
            var maybe = Maybe<int>.Some(42);
            var result = maybe.Bind(x => Maybe<string>.Some(x.ToString()));
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo("42"));
        }

        [Test]
        public void Bind_ReturnsNoneWhenNone()
        {
            var maybe = Maybe<int>.None();
            var result = maybe.Bind(x => Maybe<string>.Some(x.ToString()));
            Assert.That(result.HasValue, Is.False);
        }

        [Test]
        public void ImplicitConversion_CreatesValueWithContent()
        {
            Maybe<int> maybe = 42;
            Assert.That(maybe.HasValue, Is.True);
            Assert.That(maybe.Value, Is.EqualTo(42));
        }

        [Test]
        public void ToString_ReturnsCorrectRepresentationWhenSome()
        {
            var maybe = Maybe<int>.Some(42);
            Assert.That(maybe.ToString(), Is.EqualTo("Some(42)"));
        }

        [Test]
        public void ToString_ReturnsCorrectRepresentationWhenNone()
        {
            var maybe = Maybe<int>.None();
            Assert.That(maybe.ToString(), Is.EqualTo("None"));
        }
    }
}