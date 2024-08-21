using MAVLinkAPI.Scripts.Util;
using NUnit.Framework;
using UnityEngine;

namespace MAVLinkAPI.Editor.Util
{
    [TestFixture]
    [TestOf(typeof(UnityQuaternionExtensions))]
    public class UnityQuaternionExtensionsSpecs
    {
        [Test]
        public void InvertPitch_IdentityQuaternion_ReturnsIdentityQuaternion()
        {
            var result = Quaternion.identity.InvertPitch();
            AssertQuaternions.Equal(Quaternion.identity, result);
        }

        [Test]
        public void InvertPitch_90DegreePitch_Returns90DegreeOppositePitch()
        {
            var pitch90 = Quaternion.Euler(90, 0, 0);
            var expected = Quaternion.Euler(-90, 0, 0);
            var result = pitch90.InvertPitch();
            AssertQuaternions.Equal(expected, result);
        }


        [Test]
        public void InvertPitch_ComplexRotation_OnlyInvertsPitch()
        {
            var complex = Quaternion.Euler(30, 45, 0);
            var expected = Quaternion.Euler(-30, 45, 0);
            var result = complex.InvertPitch();
            AssertQuaternions.Equal(expected, result);
        }

        [Test]
        public void InvertPitch_ComplexRotation_OnlyInvertsPitch2()
        {
            var complex = Quaternion.Euler(30, 60, 0);
            var expected = Quaternion.Euler(-30, 60, 0);
            var result = complex.InvertPitch();
            AssertQuaternions.Equal(expected, result);
        }
    }

    public static class AssertQuaternions
    {
        private const float Epsilon = 1e-6f;

        public static void Equal(Quaternion expected, Quaternion actual)
        {
            var msg = $"component mismatch, expected: {expected}, actual: {actual}";

            Assert.That(Mathf.Abs(expected.x - actual.x), Is.LessThan(Epsilon), $"X {msg}");
            Assert.That(Mathf.Abs(expected.y - actual.y), Is.LessThan(Epsilon), $"Y {msg}");
            Assert.That(Mathf.Abs(expected.z - actual.z), Is.LessThan(Epsilon), $"Z {msg}");
            Assert.That(Mathf.Abs(expected.w - actual.w), Is.LessThan(Epsilon), $"W {msg}");
        }
    }
}