using System;
using HMD.Scripts.Pickle;
using HMD.Scripts.Streaming.VCap;
using NUnit.Framework;

namespace HMD.Editor.Pickle
{
    public class PickleSpike
    {
        // A Test behaves as an ordinary method
        [Test]
        public void SpikeMissingField()
        {
            // Use the Assert class to test conditions

            var str = "name: '/dev/video0'";
            var yaml = new Yaml();

            var obj = yaml.Rev<VCapFeed.DeviceSelector>(str);
            var str2 = yaml.Fwd(obj);

            Assert.AreEqual(str2, $"name: /dev/video0{Environment.NewLine}resolution: {Environment.NewLine}");
            // Use the Assert class to test conditions
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