namespace MAVLinkKit.Scripts.API
{
    using System;
    using System.Collections.Generic;

    public class MsgInfoLookup
    {
        public Dictionary<uint, MAVLink.message_info> ByID = new Dictionary<uint, MAVLink.message_info>();
        public Dictionary<Type, MAVLink.message_info> ByType = new Dictionary<Type, MAVLink.message_info>();

        public static MsgInfoLookup global = new MsgInfoLookup();

        public void Compile()
        {
            foreach (var info in MAVLink.MAVLINK_MESSAGE_INFOS)
            {
                ByID.Add(info.msgid, info);
                ByType.Add(info.type, info);
            }
        }
    }

    // mavlink msg id is automatically inferred by reflection
    public class TypedMsg<T> where T : struct
    {
        public T Data;

        public MAVLink.message_info Info
        {
            get
            {
                return MsgInfoLookup.global.ByType[typeof(T)];
            }
        }

        public MAVLink.MAVLINK_MSG_ID MsgType
        {
            get
            {
                return (MAVLink.MAVLINK_MSG_ID)Info.msgid;
            }
        }

        // public TypedMsg(MAVLink.message_info info)
        // {
        //     this.info = info;
        // }
        //
        // public TypedMsg(MAVLink.message_info info, T data)
        // {
        //     this.info = info;
        //     this.data = data;
        // }
        //
        // public TypedMsg(MAVLink.MAVLinkMessage msg)
        // {
        //     info = MsgCache.global.ByID[msg.msgid];
        //     data = (T)info.type.GetConstructor(new Type[] { }).Invoke(new object[] { });
        //     MAVLink.Deserialize(msg, ref data);
        // }

        // public MAVLink.MAVLinkMessage Pack(byte sysid, byte compid)
        // {
        //     return MAVLink.Pack(sysid, compid, info.msgid, data);
        // }
    }
}
