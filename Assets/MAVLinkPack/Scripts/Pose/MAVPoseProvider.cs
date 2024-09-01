#nullable enable
namespace MAVLinkPack.Scripts.Pose
{
    using System.Linq;
    using API;
    using UnityEngine;
    using UnityEngine.Experimental.XR.Interaction;
    using UnityEngine.SpatialTracking;

    public class MAVPoseProvider : BasePoseProvider
    {
        public MAVPoseFeed.ArgsAPI args = MAVPoseFeed.ArgsAPI.MatchAll;

        private MAVPoseFeed? _feed;

        public void TryConnect()
        {
            if (_feed == null)
            {
                _feed = MAVPoseFeed.Of(args);
            }
        }

        public void TryDisconnect()
        {
            _feed?.Dispose();
            _feed = null;
        }

        private void OnDestroy()
        {
            TryDisconnect();
        }

        private void Start()
        {
            TryConnect();
        }

        Reader<Quaternion>? Reader
        {
            get
            {
                if (_feed == null) return null;
                return _feed.Reader;
            }
        }


        // Update Pose
        public override PoseDataFlags GetPoseFromProvider(out Pose output)
        {
            Quaternion q;

            var reader = Reader;

            // TODO: this will cause pose to jump back to origin in case of connection loss or long latency
            q = reader != null ? reader.Drain().LastOrDefault() : Quaternion.identity;

            output = new Pose(new Vector3(0, 0, 0), q);
            return PoseDataFlags.Rotation;
        }
    }
}