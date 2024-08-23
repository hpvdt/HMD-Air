namespace MAVLinkKit.Scripts.API
{
    using UnityEngine;
    using UnityEngine.Experimental.XR.Interaction;
    using UnityEngine.SpatialTracking;

    public class MavPoseProvider : BasePoseProvider
    {
        public SerialConnection _connection;

        private void TryDisconnect()
        {
            _connection.Dispose();
        }

        private void OnDestroy()
        {
            TryDisconnect();
        }

        // Update Pose
        public override PoseDataFlags GetPoseFromProvider(out Pose output)
        {
            var msg = new MAVLink.mavlink_heartbeat_t
            {
                type = (byte)MAVLink.MAV_TYPE.GCS,
                autopilot = (byte)MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA,
                base_mode =
                    (byte)(MAVLink.MAV_MODE_FLAG.GUIDED_ENABLED
                        | MAVLink.MAV_MODE_FLAG.SAFETY_ARMED)
            }; // this is an example demonstrating the lack of type safety

            _connection.Read<MAVLink.mavlink_attitude_quaternion_t>()
                .AndThen(
                    msg =>
                    {
                        if (msg == null) return;

                        if (msg.MsgType == MAVLink.MAVLINK_MSG_ID.AHRS)
                        {
                            var ahrs = (MAVLink.mavlink_ahrs_t)msg.Data;
                            Attitude.UpdateFromAhrs(ahrs);
                        }
                    }
                );

            if (IsConnecting()) Attitude.UpdateFromGlasses();

            var mousePressed = Input.GetMouseButton(1);
            var keyPressed = Input.GetKey(KeyCode.LeftAlt);

            if (mousePressed || keyPressed)
            {
                Attitude.UpdateFromMouse();
            }

            var compound = Attitude.Mouse * Attitude.Zeroing * Attitude.Glasses;

            output = new Pose(new Vector3(0, 0, 0), compound);
            return PoseDataFlags.Rotation;
        }
    }
}
