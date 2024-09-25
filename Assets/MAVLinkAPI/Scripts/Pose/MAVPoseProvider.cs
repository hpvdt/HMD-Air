#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HMD.Scripts.Pickle;
using HMD.Scripts.Streaming.VLC;
using LibVLCSharp;
using MAVLinkAPI.Scripts.API;

namespace MAVLinkAPI.Scripts.Pose
{
    using UnityEngine;
    using UnityEngine.Experimental.XR.Interaction;
    using UnityEngine.SpatialTracking;

    public class MAVPoseProvider : BasePoseProvider
    {
        private MAVPoseFeed? _feed;

        private Yaml _pickler = new();

        public void Open(string path)
        {
            var urlContent = File.ReadAllText(path);
            var lines = urlContent.Split('\n').ToList();
            lines.RemoveAll(string.IsNullOrEmpty);

            if (lines.Count <= 0) throw new IOException($"No line defined in file `${path}`");

            var selectorStr = string.Join("\n", lines);
            var selector = _pickler.Rev<CleanSerial.ArgsT>(selectorStr);

            Open(selector);
        }

        public void Open(CleanSerial.ArgsT args)
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

        // private void Start()
        // {
        //     TryConnect();
        // }

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