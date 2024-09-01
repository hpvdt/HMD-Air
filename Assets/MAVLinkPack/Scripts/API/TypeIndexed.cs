#nullable enable
using System.Collections.Generic;
using HMD.Scripts.Util;

namespace MAVLinkPack.Scripts.API
{
    public struct TypeIndexed<T>
    {
        public TypeLookup Lookup;

        public Dictionary<uint, T> Index;

        // default constructor

        public class Accessor : Dependent<TypeIndexed<T>>
        {
            public uint ID;

            public T Value
            {
                get => Outer.Index[ID];
                set => Outer.Index[ID] = value;
            }

            public T? ValueOrDefault => Outer.Index.GetValueOrDefault(ID);

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

        public static TypeIndexed<T> Global()
        {
            return new TypeIndexed<T> { Lookup = TypeLookup.Global, Index = new Dictionary<uint, T>() };
        }
    }
}