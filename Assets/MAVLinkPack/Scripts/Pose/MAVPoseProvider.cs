#nullable enable
namespace MAVLinkPack.Scripts.Pose
{
    using UnityEngine;
    using UnityEngine.Experimental.XR.Interaction;
    using UnityEngine.SpatialTracking;

    public class MAVPoseProvider : BasePoseProvider
    {
        public MAVPoseFeed.ArgsT args = MAVPoseFeed.ArgsT.MatchAll;

        private MAVPoseFeed? _feed;

        public void TryConnect()
        {
            lock (this)
            {
                if (_feed == null)
                {
                    _feed = MAVPoseFeed.Of(args);
                    _feed.Pose.Start();
                }
            }
        }

        public void TryDisconnect()
        {
            lock (this)
            {
                _feed?.Dispose();
                _feed = null;
            }
        }

        private void OnDestroy()
        {
            TryDisconnect();
        }

        private void Start()
        {
            TryConnect();
        }

        // private Reader<Quaternion>? Reader
        // {
        //     get
        //     {
        //         if (_feed == null) return null;
        //
        //         if (_feed.ExistingReader == null) return null;
        //
        //         return _feed.ExistingReader.Value;
        //     }
        // }


        // Update Pose
        public override PoseDataFlags GetPoseFromProvider(out Pose output)
        {
            // TODO: this will cause pose to jump back to origin in case of connection loss or long latency
            if (_feed != null)
            {
                output = new Pose(new Vector3(0, 0, 0), _feed.Pose.Attitude);
                return PoseDataFlags.Rotation;
            }

            output = Pose.identity;
            return PoseDataFlags.NoData;
        }
    }
}