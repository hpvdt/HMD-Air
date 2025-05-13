using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

namespace MAVLinkAPI.Scripts.Util.Lifetime
{
    public class Lifetime : SafeHandleMinusOneIsInvalid
    {
        private static readonly IntPtr ValidHandle = new(0);
        // private static readonly IntPtr InvalidHandle = new(-1);

        // Using HashSet with a lock for thread safety
        private readonly ConcurrentDictionary<int, Cleanable> _managed = new();
        private readonly object _managedLock = new();


        public Lifetime(
            IntPtr? handle = null,
            bool ownsHandle = true
        ) : base(ownsHandle)
        {
            handle ??= ValidHandle;

            SetHandle(handle.Value);
        }

        protected sealed override bool ReleaseHandle()
        {
            var noError = true;

            lock (_managedLock)
            {
                foreach (var cleanable in _managed.Values)
                    try
                    {
                        cleanable.Dispose();
                        Deregister(cleanable);
                        Debug.Log($"successfully cleaned {cleanable}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("cleaning failed: " + e);
                        noError = false;
                    }
            }

            return noError;
        }

        // Methods to add and remove Cleanable objects with thread safety
        public void Register(Cleanable cleanable)
        {
            lock (_managedLock)
            {
                _managed.GetOrAdd(cleanable.ID, cleanable);
            }
        }

        public void Deregister(Cleanable cleanable)
        {
            lock (_managedLock)
            {
                _managed.Remove(cleanable.ID, out _);
            }
        }

        // Operator overloads for += and -= syntax
        public static Lifetime operator +(Lifetime lifetime, Cleanable cleanable)
        {
            lifetime.Register(cleanable);
            return lifetime;
        }

        public static Lifetime operator -(Lifetime lifetime, Cleanable cleanable)
        {
            lifetime.Deregister(cleanable);
            return lifetime;
        }

        // Static lifetime that can be used application-wide
        public static readonly Lifetime Static = new StaticLifetime();

        // Internal implementation for the static lifetime
        private class StaticLifetime : Lifetime
        {
            public StaticLifetime() : base()
            {
            }
        }
    }


    // public class Fallback : Lifetime
    // {
    // }
}