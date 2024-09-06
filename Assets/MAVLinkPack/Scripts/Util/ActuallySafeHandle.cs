using System;
using Microsoft.Win32.SafeHandles;

namespace MAVLinkPack.Scripts.Util
{
    public abstract class ActuallySafeHandle : SafeHandleMinusOneIsInvalid
    {
        public ActuallySafeHandle(bool ownsHandle) : base(ownsHandle)
        {
        }


        public ActuallySafeHandle() : base(true)
        {
        }

        private static readonly IntPtr InvalidHandleValue = new(-1);
        public override bool IsInvalid => handle == InvalidHandleValue;

        protected abstract bool ActualReleaseHandle();


        protected sealed override bool ReleaseHandle()
        {
            if (!IsInvalid) return ActualReleaseHandle();

            return false;
        }

        protected sealed override void Dispose(bool disposing)
        {
            ActualReleaseHandle();
        }
    }
}