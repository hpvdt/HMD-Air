#nullable enable
using HMD.Scripts.Util;

namespace MAVLinkPack.Scripts.API
{
    using System;
    using System.Collections.Generic;

    public class TypeLookup
    {
        public readonly Dictionary<uint, MAVLink.message_info> ByID = new();
        public readonly Dictionary<Type, MAVLink.message_info> ByType = new();

        public static readonly TypeLookup Global = new TypeLookup();

        // constructor
        private TypeLookup()
        {
            Compile();
        }

        public void Compile()
        {
            var report = new List<string>();
            foreach (var info in MAVLink.MAVLINK_MESSAGE_INFOS)
            {
                ByID.Add(info.msgid, info);
                ByType.Add(info.type, info);
                report.Add($"{info.msgid} -> {info.type.Name}");
            }

            Console.WriteLine("MAVLink message lookup compiled:\n" + string.Join("\n", report));
        }
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