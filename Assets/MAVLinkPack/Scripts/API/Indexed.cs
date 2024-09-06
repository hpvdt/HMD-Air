#nullable enable
using System;
using System.Collections.Generic;
using MAVLinkPack.Editor.Util;

namespace MAVLinkPack.Scripts.API
{
    public struct Indexed<T>
    {
        // TODO: do I need to index by systemID and componentID?
        public TypeLookup Lookup;

        public Dictionary<uint, T?> Index;

        // default constructor

        public class Accessor : Dependent<Indexed<T>>
        {
            public uint ID;

            public T? Value
            {
                get => Outer.Index[ID];
                set => Outer.Index[ID] = value;
            }

            public T? ValueOrDefault => Outer.Index.GetValueOrDefault(ID);

            public T ValueOr(T fallback)
            {
                return Outer.Index.GetValueOrDefault(ID, fallback);
            }

            public T ValueOrInsert(Func<T> fallback)
            {
                var index = Outer.Index;
                if (index.TryGetValue(ID, out var existing)) return existing;

                index[ID] = fallback();
                return index[ID];
            }

            public T? ValueOrInsertDefault()
            {
                return ValueOrInsert(() => default);
            }

            public void Remove()
            {
                Outer.Index.Remove(ID);
            }

            public MAVLink.message_info Info => Outer.Lookup.ByID[ID];
        }

        public readonly Accessor Get(uint id)
        {
            return new Accessor { Outer = this, ID = id };
        }

        public readonly Accessor Get<TMav>() where TMav : struct
        {
            var id = TypeLookup.Global.ByType[typeof(TMav)].msgid;
            return Get(id);
        }

        // do we need by systemID and componentID?

        public static Indexed<T> Global(Dictionary<uint, T>? index = null)
        {
            var ii = index ?? new Dictionary<uint, T>();
            return new Indexed<T> { Lookup = TypeLookup.Global, Index = ii };
        }
    }
}