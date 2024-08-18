using NUnit.Framework;

namespace HMD.Tests
{
    using Scripts.Pickle;
    using Scripts.Streaming.VCap;

    public class PickeSuite
    {
        // A Test behaves as an ordinary method
        [Test]
        public void PickeSuiteSimplePasses()
        {
            // Use the Assert class to test conditions

            var str = "name: '/dev/video0'\n";
            var yaml = new Yaml();

            var obj = yaml.Rev<VCapFeed.DeviceSelector>(str);
            var str2 = yaml.Fwd(obj);

            Assert.AreEqual(str, str2);
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
