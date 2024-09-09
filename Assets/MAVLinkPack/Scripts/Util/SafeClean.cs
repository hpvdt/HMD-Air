using System;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;

namespace MAVLinkPack.Scripts.Util
{
    public static class SafeCleanManager<T>
    {
        public static readonly List<T> Pool = new();

        public static readonly object ReadWrite = new();
    }

    public abstract class SafeClean : SafeHandleMinusOneIsInvalid
    {
        private static readonly IntPtr ValidHandle = new(0);

        // private static readonly IntPtr InvalidHandle = new(-1);

        public SafeClean(IntPtr? handle = null, bool ownsHandle = true) : base(ownsHandle)
        {
            handle ??= ValidHandle;

            SetHandle(handle.Value);

            lock (this.ReadWrite())
            {
                this.Peers().Add(this);
            }
        }

        protected sealed override bool ReleaseHandle()
        {
            var success = false;
            try
            {
                success = DoClean();
            }
            finally
            {
                if (success)
                    lock (this.ReadWrite())
                    {
                        this.Peers().Remove(this);
                    }
            }

            return success;
        }

        protected abstract bool DoClean();
    }

    public static class SafeCleanExtensions
    {
        public static object ReadWrite<T>(this T self) where T : SafeClean
        {
            return SafeCleanManager<T>.ReadWrite;
        }

        public static List<T> Peers<T>(this T tt) where T : SafeClean
        {
            lock (tt.ReadWrite())
            {
                return SafeCleanManager<T>.Pool;
            }
        }
    }
}