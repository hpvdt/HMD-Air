using System.Collections.Generic;
using System.Linq;
using HMDCommons.Scripts;
using SFB;
using Unity.VisualScripting;
using UnityEngine;

namespace HMD.Scripts.Streaming.VCap
{
    public class VCapScreen : ScreenLike
    {
        [Required] public VCapFeed feed;

        protected override FeedLike Feed => feed;

        // public void NextCamera(string resText)
        // {
        //     var res = ParseResolution(resText);
        //
        //     captureDeviceFeed.OpenNextDevice(res);
        //     Play();
        // }

        // private static Resolution? ParseResolution(string resText)
        // {
        //     // TODO: remove if Yaml can handle everything
        //
        //     if (string.IsNullOrEmpty(resText)) return null;
        //
        //     var parts = resText.Split(":").ToList();
        //     var nums = parts.Select(x => int.Parse(x)).ToArray();
        //
        //     if (nums.Length == 2)
        //     {
        //         var fps = new RefreshRate
        //         {
        //             numerator = 0,
        //             denominator = 1
        //         };
        //
        //         return new Resolution
        //         {
        //             width = nums[0],
        //             height = nums[1],
        //             refreshRateRatio = fps
        //         };
        //     }
        //
        //     if (nums.Length == 3)
        //     {
        //         var fps = new RefreshRate
        //         {
        //             numerator = (uint)nums[2],
        //             denominator = 1
        //         };
        //         return new Resolution
        //         {
        //             width = nums[0],
        //             height = nums[1],
        //             refreshRateRatio = fps
        //         };
        //     }
        //
        //     throw new ArgumentException($"Illegal resolution format {resText}");
        // }


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

                feed.Open(path!);
                Play();
            }
        }
    }
}