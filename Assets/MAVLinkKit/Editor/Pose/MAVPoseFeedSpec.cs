using System.Linq;
using MAVLinkKit.Scripts.Pose;
using NUnit.Framework;
using UnityEngine;

namespace MAVLinkKit.Editor.Pose
{
    public class MAVPoseFeedSpec
    {
        // [Test]
        // public void Connect()
        // {
        //     // feed.ConnectAndRead10();
        // }

        [Test]
        public void ConnectAndRead10()
        {
            var feed = MAVPoseFeed.Of(MAVPoseFeed.Args.MatchAll);

            for (var i = 0; i < 10; i++)
            {
                var qs = feed.Reader.Value.Drain();

                Debug.Log($"Quaternion:\n" +
                          $"{qs.Aggregate("", (acc, q) => acc + q + "\n")}");
            }
        }
    }
}