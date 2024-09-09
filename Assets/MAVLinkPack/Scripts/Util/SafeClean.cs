using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

namespace MAVLinkPack.Scripts.Util
{
    public static class SafeCleanManager
    {
        public static readonly AtomicInt GlobalCounter = new();

        public static readonly List<SafeClean> Pool = new();

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
                SafeCleanManager.Pool.Add(this);
                SafeCleanManager.GlobalCounter.Increment();
            }

            var peers = this.SelfAndPeers();
            if (!peers.Contains(this))
                Debug.LogException(new IOException("INTERNAL ERROR!"));
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
                {
                    Debug.Log($"Cleaning successful, removing {this} from peers");
                    lock (this.ReadWrite())
                    {
                        SafeCleanManager.Pool.Remove(this);
                        SafeCleanManager.GlobalCounter.Decrement();
                    }
                }
                else
                {
                    Debug.LogWarning("Cleaning failed");
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
            return SafeCleanManager.ReadWrite; // should avoid a global lock and classify by types
        }

        public static IEnumerable<T> SelfAndPeers<T>(this T self)
            where T : SafeClean // should be read only
        {
            lock (self.ReadWrite())
            {
                var selfType = self.GetType();

                var filtered = SafeCleanManager.Pool
                    .Where(x =>
                    {
                        var isSelfType = x.GetType() == selfType;
                        return isSelfType;
                    })
                    .Cast<T>();

                return filtered;
            }
        }


        public static IEnumerable<T> Peers<T>(this T self) where T : SafeClean
        {
            var peers = self.SelfAndPeers<T>();
            return peers.Where(v => !ReferenceEquals(v, self));
        }
    }
}