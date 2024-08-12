namespace HMD.Scripts.Streaming.Capture
{
    using System;
    using System.Linq;
    using UnityEngine;

    public class CaptureDeviceScreen : PlayerScreen
    {
        public CaptureDeviceFeed captureDeviceFeed;

        protected override FeedLike Feed
        {
            get { return captureDeviceFeed; }
        }
        
        // public void NextCamera(string resText)
        // {
        //     var res = ParseResolution(resText);
        //
        //     captureDeviceFeed.OpenNextDevice(res);
        //     Play();
        // }
        // TODO: need a single text I/O representation to specify both device & resolution
        
        private static Resolution? ParseResolution(string resText)
        {
            if (string.IsNullOrEmpty(resText)) return null;

            var parts = resText.Split(":").ToList();
            var nums = parts.Select(x => int.Parse(x)).ToArray();

            if (nums.Length == 2)
            {
                var fps = new RefreshRate
                {
                    numerator = 0,
                    denominator = 1
                };

                return new Resolution
                {
                    width = nums[0],
                    height = nums[1],
                    refreshRateRatio = fps
                };
            }
            else if (nums.Length == 3)
            {
                var fps = new RefreshRate
                {
                    numerator = (uint)nums[2],
                    denominator = 1
                };
                return new Resolution
                {
                    width = nums[0],
                    height = nums[1],
                    refreshRateRatio = fps
                };
            }

            throw new ArgumentException($"Illegal resolution format {resText}");
        }
    }
    
    
    
    

}
