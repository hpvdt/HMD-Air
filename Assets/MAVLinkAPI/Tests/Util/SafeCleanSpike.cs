using System;
using MAVLinkAPI.Scripts.Util;
using MAVLinkAPI.Scripts.Util.Lifetime;
using Microsoft.Win32.SafeHandles;
using NUnit.Framework;

namespace MAVLinkAPI.Tests.Util
{
    public class SafeCleanSpike
    {
        [Test]
        public void Fake()
        {
            var i1 = HandleExample.Counter;

            // using (var obj = new Fake(10, true))
            // using (var obj = new Fake(0, true))
            // using (var obj = new Fake(-1, true)) // won't work
            using (var obj = new HandleExample(true))
            {
                Assert.AreEqual(i1, HandleExample.Counter);
                // do things
            }

            Assert.AreEqual(i1 + 1, HandleExample.Counter);
        }

        [Test]
        public void Real()
        {
            var i1 = TestCleanable.Counter;
            using (var obj = new TestCleanable())
            {
                Assert.AreEqual(i1, TestCleanable.Counter);
                // do things
            }

            Assert.AreEqual(i1 + 1, TestCleanable.Counter);
        }
    }

    public class HandleExample : SafeHandleMinusOneIsInvalid
    {
        public static volatile int Counter;

        public HandleExample(nint handle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(handle);
        }

        public HandleExample(bool ownsHandle) : base(ownsHandle)
        {
            var i = new IntPtr(0);

            SetHandle(i);
        }

        protected override bool ReleaseHandle()
        {
            Counter += 1;
            return true;
        }
    }

    public class TestCleanable : Cleanable
    {
        public static volatile int Counter;


        protected override void DoClean()
        {
            Counter += 1;
        }
    }
}