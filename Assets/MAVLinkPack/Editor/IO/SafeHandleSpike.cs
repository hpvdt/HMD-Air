using System;
using MAVLinkPack.Scripts.Util;
using Microsoft.Win32.SafeHandles;
using NUnit.Framework;

namespace MAVLinkPack.Editor.IO
{
    public class SafeHandleSpike
    {
        // [Test]
        // will fail
        public void Fake()
        {
            var i1 = IO.Fake.Counter;
            using (var obj = new Fake(true))
            {
                Assert.AreEqual(i1, IO.Fake.Counter);
                // do things
            }

            Assert.AreEqual(i1 + 1, IO.Fake.Counter);
        }

        [Test]
        public void Real()
        {
            var i1 = IO.Real.Counter;
            using (var obj = new Real(true))
            {
                Assert.AreEqual(i1, IO.Real.Counter);
                // do things
            }

            Assert.AreEqual(i1 + 1, IO.Real.Counter);
        }
    }

    public class Fake : SafeHandleMinusOneIsInvalid
    {
        public static volatile int Counter;

        public Fake(nint handle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(handle);
        }

        public Fake(bool ownsHandle) : base(ownsHandle)
        {
        }

        protected override bool ReleaseHandle()
        {
            Counter += 1;
            return true;
        }
    }

    public class Real : ActuallySafeHandle
    {
        public static volatile int Counter;

        public Real(bool ownsHandle) : base(ownsHandle)
        {
        }

        protected override bool ActualReleaseHandle()
        {
            Counter += 1;
            return true;
        }
    }
}