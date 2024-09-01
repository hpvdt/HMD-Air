using System.Linq;
using MAVLinkPack.Scripts.Pose;
using NUnit.Framework;
using UnityEngine;

namespace MAVLinkPack.Editor.Pose
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
            var feed = MAVPoseFeed.Of(MAVPoseFeed.ArgsAPI.MatchAll);

            for (var i = 0; i < 10; i++)
            {
                var qs = feed.Reader.Drain();

                Debug.Log($"Quaternion:\n" +
                          $"{qs.Aggregate("", (acc, q) => acc + q + "\n")}");
            }
        }
    }
}