namespace MAVLinkKit.Scripts.API
{
    public struct MAVComponent
    {
        // our target sysid
        public byte SystemID;

        // our target compid
        public byte ComponentID;

        public static MAVComponent Gcs(byte compid = 0)
        {
            return new MAVComponent
            {
                SystemID = 255,
                ComponentID = compid
            };
        }

        public TypedMsg<T> Send<T>(T data) where T : struct
        {
            return new TypedMsg<T>
            {
                Data = data,
                Sender = this
            };
        }
    }
}