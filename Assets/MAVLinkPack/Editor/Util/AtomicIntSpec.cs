using System.Threading.Tasks;
using MAVLinkPack.Scripts.Util;
using NUnit.Framework;

namespace MAVLinkPack.Editor.Util
{
    [TestFixture]
    public class AtomicIntSpec
    {
        [TestFixture]
        public class AtomicIntCounterTests
        {
            [Test]
            public void InitialValue_ShouldBeZero()
            {
                var counter = new AtomicInt();
                Assert.AreEqual(0, counter.Value);
            }

            [Test]
            public void InitialValue_ShouldBeSet()
            {
                var counter = new AtomicInt();
                counter.Value = 5;
                Assert.AreEqual(5, counter.Value);
            }

            [Test]
            public void Increment_ShouldIncrementValue()
            {
                var counter = new AtomicInt();
                Assert.AreEqual(1, counter.Next());
                Assert.AreEqual(1, counter.Value);
            }

            [Test]
            public void Decrement_ShouldDecrementValue()
            {
                var counter = new AtomicInt();
                counter.Value = 1;
                Assert.AreEqual(0, counter.Prev());
                Assert.AreEqual(0, counter.Value);
            }

            [Test]
            public void Add_ShouldAddValue()
            {
                var counter = new AtomicInt();
                Assert.AreEqual(5, counter.Add(5));
                Assert.AreEqual(5, counter.Value);
            }

            // [Test]
            public async Task Spike()
            {
                await Task.Delay(100); //Unity bug, upgrade until this test pass
            }

            [Test]
            public void ConcurrentAccess_ShouldBeThreadSafe()
            {
                var counter = new AtomicInt();
                var tasks = new Task[1000];

                for (var i = 0; i < 1000; i++)
                    tasks[i] = Task.Run(() => { counter.Next(); });

                var all = Task.WhenAll(tasks);
                all.Wait();


                Assert.AreEqual(1000, counter.Value);
            }
        }
    }
}