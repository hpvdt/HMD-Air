using System;
using System.Collections.Generic;
using HMD.Scripts.Streaming.VCap;
using MAVLinkAPI.Routing;
using NUnit.Framework;
using UnityEngine;

namespace HMD.Tests.Streaming
{
    public class VCapSpec
    {
        private static void AssertArgsEqual(VCapFeed.ArgsT expected, VCapFeed.ArgsT actual)
        {
            Assert.AreEqual(expected.Index, actual.Index);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Resolution.HasValue, actual.Resolution.HasValue);

            if (expected.Resolution.HasValue)
            {
                Assert.IsTrue(actual.Resolution.HasValue);
                var expectedRes = expected.Resolution.Value;
                var actualRes = actual.Resolution.Value;

                Assert.AreEqual(expectedRes.width, actualRes.width);
                Assert.AreEqual(expectedRes.height, actualRes.height);

                if (expectedRes.refreshRateRatio.numerator != 0 || actualRes.refreshRateRatio.numerator != 0)
                {
                    Assert.AreEqual(expectedRes.refreshRateRatio.numerator, actualRes.refreshRateRatio.numerator);
                    Assert.AreEqual(expectedRes.refreshRateRatio.denominator, actualRes.refreshRateRatio.denominator);
                }
            }
        }

        private static Resolution MakeResolution(int width, int height, int refreshRate)
        {
            var res = new Resolution
            {
                width = width,
                height = height,
                refreshRateRatio = new RefreshRate
                {
                    numerator = (uint)refreshRate,
                    denominator = 1
                }
            };
            return res;
        }

        // A Test behaves as an ordinary method
        [Test]
        public void SpikeMissingField()
        {
            // Use the Assert class to test conditions
            var str = "name: '/dev/video0'";
            var yaml = new Yaml();

            var obj = yaml.Rev<VCapFeed.ArgsT>(str);
            var str2 = yaml.Fwd(obj);

            var expected = "index: \n"
                           + "name: /dev/video0\n"
                           + "resolution: \n";
            Assert.AreEqual(expected, str2.Replace("\r\n", "\n"));
        }

        [Test]
        public void Deserialize_MinimalYaml_PopulatesNameAndLeavesOptionalsNull()
        {
            var yaml = new Yaml();
            var obj = yaml.Rev<VCapFeed.ArgsT>("name: /dev/video0");

            Assert.IsNull(obj.Index);
            Assert.AreEqual("/dev/video0", obj.Name);
            Assert.IsFalse(obj.Resolution.HasValue);
        }

        [Test]
        public void Deserialize_WithResolutionWidthHeightOnly_PopulatesResolution()
        {
            var yaml = new Yaml();
            var obj = yaml.Rev<VCapFeed.ArgsT>(
                "name: cameraWithRes\n" +
                "resolution:\n" +
                "  width: 640\n" +
                "  height: 480\n"
            );

            Assert.IsNull(obj.Index);
            Assert.AreEqual("cameraWithRes", obj.Name);
            Assert.IsTrue(obj.Resolution.HasValue);
            Assert.AreEqual(640, obj.Resolution.Value.width);
            Assert.AreEqual(480, obj.Resolution.Value.height);
            Assert.AreEqual(0u, obj.Resolution.Value.refreshRateRatio.numerator);
        }

        [Test]
        public void RoundTrip_WithIndexAndName_PreservesValues()
        {
            var yaml = new Yaml();
            var original = new VCapFeed.ArgsT
            {
                Index = 3,
                Name = "camera0",
                Resolution = null
            };

            var serialized = yaml.Fwd(original);
            var roundTrip = yaml.Rev<VCapFeed.ArgsT>(serialized);

            AssertArgsEqual(original, roundTrip);
        }

        [Test]
        public void RoundTrip_ListOfArgs_PreservesCountAndValues()
        {
            var yaml = new Yaml();
            var original = new List<VCapFeed.ArgsT>
            {
                new() { Index = 0, Name = "camA", Resolution = null },
                new() { Index = 1, Name = "camB", Resolution = null }
            };

            var serialized = yaml.Fwd(original);
            var roundTrip = yaml.Rev<List<VCapFeed.ArgsT>>(serialized);

            Assert.AreEqual(original.Count, roundTrip.Count);
            for (var i = 0; i < original.Count; i++)
                AssertArgsEqual(original[i], roundTrip[i]);
        }

        [Test]
        public void RoundTrip_WithExplicitResolution_PreservesResolutionFields()
        {
            var yaml = new Yaml();
            var original = new VCapFeed.ArgsT
            {
                Index = 2,
                Name = "cameraWithRes",
                Resolution = MakeResolution(640, 480, 30)
            };

            var serialized = yaml.Fwd(original);
            StringAssert.Contains("resolution", serialized);
            StringAssert.Contains("width", serialized);
            StringAssert.Contains("height", serialized);

            var roundTrip = yaml.Rev<VCapFeed.ArgsT>(serialized);
            AssertArgsEqual(original, roundTrip);
        }

        [Test]
        public void RoundTrip_ListOfArgs_WithResolution_PreservesResolutionFields()
        {
            var yaml = new Yaml();
            var original = new List<VCapFeed.ArgsT>
            {
                new() { Index = 0, Name = "camA", Resolution = MakeResolution(1920, 1080, 60) },
                new() { Index = 1, Name = "camB", Resolution = null }
            };

            var serialized = yaml.Fwd(original);
            var roundTrip = yaml.Rev<List<VCapFeed.ArgsT>>(serialized);

            Assert.AreEqual(original.Count, roundTrip.Count);
            for (var i = 0; i < original.Count; i++)
                AssertArgsEqual(original[i], roundTrip[i]);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        // [UnityTest]
        // public IEnumerator PickeSuiteWithEnumeratorPasses()
        // {
        //     // Use the Assert class to test conditions.
        //     // Use yield to skip a frame.
        //     yield return null;
        // }
    }
}
