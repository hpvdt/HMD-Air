using HMD.Scripts.Util;

namespace MAVLinkKit.Scripts.API
{
    using System;
    using System.Collections.Generic;

    public class TypeLookup
    {
        public readonly Dictionary<uint, MAVLink.message_info> ByID = new();
        public readonly Dictionary<Type, MAVLink.message_info> ByType = new();

        public static readonly TypeLookup Global = new TypeLookup();

        // constuctor
        private TypeLookup()
        {
            Compile();
        }

        public void Compile()
        {
            foreach (var info in MAVLink.MAVLINK_MESSAGE_INFOS)
            {
                ByID.Add(info.msgid, info);
                ByType.Add(info.type, info);
            }
        }
    }

    public struct IndexedByType<T>
    {
        public TypeLookup Lookup;

        public Dictionary<uint, T> Index;

        public class Accessor : Dependent<IndexedByType<T>>
        {
            public uint ID;

            public T Value
            {
                get => Outer.Index[ID];
                set => Outer.Index[ID] = value;
            }

            public T ValueOrDefault => Outer.Index.GetValueOrDefault(ID);

            public void Remove()
            {
                Outer.Index.Remove(ID);
            }

            public MAVLink.message_info Info => Outer.Lookup.ByID[ID];
        }

        public readonly Accessor Get(uint id)
        {
            return new Accessor { ID = id };
        }

        public readonly Accessor Get<TMav>() where TMav : struct
        {
            var id = TypeLookup.Global.ByType[typeof(TMav)].msgid;
            return Get(id);
        }

        // do we need by systemID and componentID?
    }

    // mavlink msg id is automatically inferred by reflection
    public struct TypedMsg<T> where T : struct
    {
        public T Data;
        public MAVComponent Sender;

        public MAVLink.message_info Info
        {
            get
            {
                var id1 = TypeLookup.Global.ByType[typeof(T)];

                return id1;
                // TODO: add verified info that also run the lookup by 
            }
        }


        public MAVLink.MAVLINK_MSG_ID TypeID
        {
            get { return (MAVLink.MAVLINK_MSG_ID)Info.msgid; }
        }
    }


    public static class FromPacket
    {
        public static TypedMsg<T> As<T>(this MAVLink.MAVLinkMessage msg) where T : struct
        {
            var sender = new MAVComponent
            {
                SystemID = msg.sysid,
                ComponentID = msg.compid
            };

            return new TypedMsg<T>
            {
                Data = (T)msg.data,
                Sender = sender
            };
        }
    }
}