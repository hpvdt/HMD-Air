using System.Linq;
using MAVLinkPack.Scripts.Util;
using NUnit.Framework;
using UnityEngine;

namespace MAVLinkPack.Editor.Util
{
    [TestFixture]
    [TestOf(typeof(UnityQuaternionExtensions))]
    public class UnityQuaternionSpike
    {
        [Test]
        public void AllZero()
        {
            // Arrange
            var u1 = Quaternion.identity;
            var u2 = Quaternion.Euler(0, 0, 0);

            AssertQuaternions.Equal(u2, u1);
        }

        [Test]
        public void RollRoundtrip()
        {
            // Arrange
            var u1 = Quaternion.identity;
            var u2 = Quaternion.Euler(0, 0, 720);

            AssertQuaternions.Equal(u2, u1);
        }


        [Test]
        public void UnityQShouldHaveZeroEulerAngles()
        {
            // Arrange
            var unitQuaternion = Quaternion.identity;
            var expectedEulerAngles = Vector3.zero;

            // Act
            var actualEulerAngles = unitQuaternion.eulerAngles;

            // Assert
            Assert.AreEqual(expectedEulerAngles, actualEulerAngles);
        }
    }
}