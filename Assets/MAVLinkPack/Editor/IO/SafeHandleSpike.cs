using System;
using MAVLinkPack.Scripts.Util;
using Microsoft.Win32.SafeHandles;
using NUnit.Framework;

namespace MAVLinkPack.Editor.IO
{
    public class SafeHandleSpike
    {
        [Test]
        public void Fake()
        {
            var i1 = IO.Fake.Counter;

            // using (var obj = new Fake(10, true))
            // using (var obj = new Fake(0, true))
            // using (var obj = new Fake(-1, true)) // won't work
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
            using (var obj = new Real())
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
            var i = new IntPtr(0);

            SetHandle(i);
        }

        protected override bool ReleaseHandle()
        {
            Counter += 1;
            return true;
        }
    }

    public class Real : SafeClean
    {
        public static volatile int Counter;


        protected override bool DoClean()
        {
            Counter += 1;
            return true;
        }
    }
}