#nullable enable
using System;
using System.Threading.Tasks;

namespace MAVLinkAPI.Scripts.Pose
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
                if (_feed == null) _feed = MAVPoseFeed.Of(args);
            }

            Task.Run(
                () =>
                {
                    try
                    {
                        _feed.StartUpdate();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            );
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

        public MAVPoseFeed.UpdaterD? UpdaterDaemon
        {
            get
            {
                if (_feed != null) return null;
                if (_feed!.UpdaterDaemon == null) return null;
                return _feed.UpdaterDaemon;
            }
        }

        // Update Pose
        public override PoseDataFlags GetPoseFromProvider(out Pose output)
        {
            var d = UpdaterDaemon;
            if (d != null)
            {
                output = new Pose(new Vector3(0, 0, 0), d.Attitude);
                return PoseDataFlags.Rotation;
            }

            output = Pose.identity;
            return PoseDataFlags.NoData;
        }
    }
}