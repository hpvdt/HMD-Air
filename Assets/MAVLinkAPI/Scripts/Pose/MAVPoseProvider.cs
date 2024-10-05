#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HMD.Scripts.Pickle;
using MAVLinkAPI.Scripts.API;
using MAVLinkAPI.Scripts.Util;
using SFB;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.SpatialTracking;

namespace MAVLinkAPI.Scripts.Pose
{
    public class MAVPoseProvider : BasePoseProvider
    {
        private MAVPoseFeed? _feed;

        private static readonly Yaml Pickler = new();

        public void PromptUserFilePicker()
        {
            var yaml = new List<string>
            {
                "yaml", "yml"
            };

            // Open file with filter
            var fileTypes = new[]
            {
                new ExtensionFilter("Video Capture Device YAML descriptor", yaml.ToArray()),
                new ExtensionFilter("Any", "*")
            };
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", fileTypes, false);
            var path = paths.FirstOrDefault();

            if (path.NullIfEmpty() == null)
            {
                Debug.Log("Operation cancelled");
            }
            else
            {
                Debug.Log("Picked file: " + path);

                Open(path!);
            }
        }

        public void Open(string path)
        {
            var urlContent = File.ReadAllText(path);
            var lines = urlContent.Split('\n').ToList();
            lines.RemoveAll(string.IsNullOrEmpty);

            // if (lines.Count <= 0) throw new IOException($"No line defined in file `${path}`");

            var selectorStr = string.Join("\n", lines);
            var selector = Pickler.Rev<Routing.ArgsT>(selectorStr);

            Open(selector);
        }

        public void Open(Routing.ArgsT args)
        {
            lock (this)
            {
                if (_feed == null) _feed = new MAVPoseFeed(args);
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
            // testProcess();
        }

        // private async void testProcess()
        // {
        //     // TODO: remove, only used for a spike
        //     var cmd = "ls";
        //     var mgr = new ExternalProcessManager("bash", $"-c '{cmd}'");
        //
        //     var task = mgr.StartAndMonitorAsync();
        //
        //     var result = await task;
        //
        //     Debug.Log($"Result is {result}");
        // }

        public MAVPoseFeed.UpdaterD? UpdaterDaemon
        {
            get
            {
                if (_feed == null) return null;
                if (_feed!.UpdaterDaemon == null) return null;
                return _feed.UpdaterDaemon;
            }
        }

        // Update Pose
        public override PoseDataFlags GetPoseFromProvider(out UnityEngine.Pose output)
        {
            var d = UpdaterDaemon;
            if (d != null)
            {
                output = new UnityEngine.Pose(new Vector3(0, 0, 0), d.Attitude);
                return PoseDataFlags.Rotation;
            }

            output = UnityEngine.Pose.identity;
            return PoseDataFlags.NoData;
        }
    }
}