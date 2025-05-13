#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace MAVLinkAPI.Scripts.Util.Lifetime
{
    public abstract class Cleanable : IDisposable
    {
        public static class GlobalRegistry
        {
            public static readonly AtomicInt GlobalCounter = new();

            // TODO: not efficient, should use ConcurrentMultiMap
            public static readonly HashSet<Cleanable> Registered = new();
        }

        public static readonly object GlobalAccessLock = new();

        public int ID = new Random().Next();

        private readonly Lifetime _lifetime;

        public Cleanable(Lifetime? lifetime = null)
        {
            lifetime ??= Lifetime.Static;

            _lifetime = lifetime;
            _lifetime.Register(this);

            lock (GlobalAccessLock)
            {
                GlobalRegistry.Registered.Add(this);
                GlobalRegistry.GlobalCounter.Increment();
            }

            //  TODO: how to add duplication runtime check?
            // var peers = this.SelfAndPeers();
            // if (!peers.Contains(this))
            //     Debug.LogException(new IOException("INTERNAL ERROR!"));
        }

        public void Dispose()
        {
            DoClean();
            _lifetime.Deregister(this);
        }


        protected abstract void DoClean();
    }


    public static class SafeCleanExtensions
    {
        // public static object AccessLock<T>(this T self) where T : Cleanable
        // {
        //     return Cleanable.GlobalAccessLock; // this is very blocking, will optimise later
        // }

        public static IEnumerable<T> SelfAndPeers<T>(this T self)
            where T : Cleanable // should be read only
        {
            lock (Cleanable.GlobalAccessLock)
            {
                var selfType = self.GetType();

                var filtered = Cleanable.GlobalRegistry.Registered
                    .Where(x =>
                    {
                        var isSelfType = x.GetType() == selfType;
                        return isSelfType;
                    })
                    .Cast<T>();

                return filtered;
            }
        }


        public static IEnumerable<T> Peers<T>(this T self) where T : Cleanable
        {
            var peers = self.SelfAndPeers<T>();
            return peers.Where(v => !ReferenceEquals(v, self));
        }
    }
}