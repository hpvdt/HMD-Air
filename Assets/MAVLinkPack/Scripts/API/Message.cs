#nullable enable
namespace MAVLinkPack.Scripts.API
{
    using System;
    using System.Collections.Generic;

    public class TypeLookup
    {
        public readonly Dictionary<uint, MAVLink.message_info> ByID = new();
        public readonly Dictionary<Type, MAVLink.message_info> ByType = new();

        public static readonly TypeLookup Global = new();

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


    public struct Component
    {
        // our target sysid
        public byte SystemID;

        // our target compid
        public byte ComponentID;

        public static Component Gcs(byte compid = 0)
        {
            return new Component
            {
                SystemID = 255,
                ComponentID = compid
            };
        }

        public Message<T> Send<T>(T data) where T : struct
        {
            return new Message<T>
            {
                Data = data,
                Sender = this
            };
        }
    }


    // mavlink msg id is automatically inferred by reflection
    public struct Message<T> where T : struct
    {
        public T Data;
        public Component Sender;

        public MAVLink.message_info Info
        {
            get
            {
                var id1 = TypeLookup.Global.ByType[typeof(T)];

                return id1;
                // TODO: add verified info that also run the lookup by 
            }
        }

        public MAVLink.MAVLINK_MSG_ID TypeID => (MAVLink.MAVLINK_MSG_ID)Info.msgid;

        public static Message<T> FromRaw(MAVLink.MAVLinkMessage msg)
        {
            var sender = new Component
            {
                SystemID = msg.sysid,
                ComponentID = msg.compid
            };

            return new Message<T>
            {
                Data = (T)msg.data,
                Sender = sender
            };
        }
    }


    public static class RawMessageExtension
    {
        public static Message<T> OfType<T>(this MAVLink.MAVLinkMessage msg) where T : struct
        {
            return Message<T>.FromRaw(msg);
        }
    }
}