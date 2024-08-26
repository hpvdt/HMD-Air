namespace MAVLinkKit.Scripts.API
{
    using System;
    using System.Collections.Generic;

    public class MsgInfoLookup
    {
        public readonly Dictionary<uint, MAVLink.message_info> ByID = new Dictionary<uint, MAVLink.message_info>();
        public readonly Dictionary<Type, MAVLink.message_info> ByType = new Dictionary<Type, MAVLink.message_info>();

        public static MsgInfoLookup global = new MsgInfoLookup();

        // constuctor
        public MsgInfoLookup()
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

    public static class MsgExtension
    {
        public static TypedMsg<T> As<T>(this MAVLink.MAVLinkMessage msg) where T : struct
        {
            return new TypedMsg<T>
            {
                Lookup = MsgInfoLookup.global,
                Msg = msg
            };
        }
    }

    // mavlink msg id is automatically inferred by reflection
    public struct TypedMsg<T> where T : struct
    {
        public MsgInfoLookup Lookup;

        public MAVLink.MAVLinkMessage Msg;

        public T Data
        {
            get
            {
                return (T)Msg.data;
            }
        }

        public MAVLink.message_info Info
        {
            get
            {
                var id1 = Lookup.ByType[typeof(T)];

                return id1;
                // TODO: add verified info that also run the lookup by 
            }
        }


        public MAVLink.MAVLINK_MSG_ID TypeID
        {
            get
            {
                return (MAVLink.MAVLINK_MSG_ID)Info.msgid;
            }
        }
    }
}
