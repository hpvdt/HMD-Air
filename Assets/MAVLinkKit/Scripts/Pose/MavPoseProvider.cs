#nullable enable
namespace MAVLinkKit.Scripts.Pose
{
    using System.Linq;
    using API;
    using UnityEngine;
    using UnityEngine.Experimental.XR.Interaction;
    using UnityEngine.SpatialTracking;

    public class MavPoseProvider : BasePoseProvider
    {
        public string pattern;
        public int baudRate;

        private UsbConnection? _connection;

        private UsbStream<Quaternion>? _attitudeStream;

        public void TryConnect()
        {
            _connection = UsbConnection.OpenFirst(new System.Text.RegularExpressions.Regex(pattern), baudRate);

            _attitudeStream = _connection.ReadStream<Quaternion>()
                .On<MAVLink.mavlink_attitude_quaternion_t>()
                .Bind1(ctx =>
                {
                    var q = new Quaternion(ctx.Msg.Data.q1, ctx.Msg.Data.q2, ctx.Msg.Data.q3, ctx.Msg.Data.q4);
                    return q;
                })
                .Build();
        }

        public void TryDisconnect()
        {
            _connection?.Dispose();
            _connection = null;
            _attitudeStream = null;
        }

        private bool IsConnected()
        {
            return _connection != null && _connection.Port.IsOpen;
        }

        private void OnDestroy()
        {
            TryDisconnect();
        }


        // Update Pose
        public override PoseDataFlags GetPoseFromProvider(out Pose output)
        {
            // var msg = new MAVLink.mavlink_heartbeat_t
            // {
            //     type = (byte)MAVLink.MAV_TYPE.GCS,
            //     autopilot = (byte)MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA,
            //     base_mode =
            //         (byte)(MAVLink.MAV_MODE_FLAG.GUIDED_ENABLED
            //             | MAVLink.MAV_MODE_FLAG.SAFETY_ARMED)
            // }; // this is an example demonstrating the lack of type safety
            //
            // Connection.ReadStream<MAVLink.mavlink_attitude_quaternion_t>()
            //     .AndThen(
            //         msg =>
            //         {
            //             if (msg == null) return;
            //
            //             if (msg.MsgType == MAVLink.MAVLINK_MSG_ID.AHRS)
            //             {
            //                 var ahrs = (MAVLink.mavlink_ahrs_t)msg.Data;
            //                 Attitude.UpdateFromAhrs(ahrs);
            //             }
            //         }
            //     );

            Quaternion q = Quaternion.identity;
            if (IsConnected())
            {
                q = _attitudeStream.Drain().LastOrDefault();
            }


            output = new Pose(new Vector3(0, 0, 0), q);
            return PoseDataFlags.Rotation;
        }
    }
}
