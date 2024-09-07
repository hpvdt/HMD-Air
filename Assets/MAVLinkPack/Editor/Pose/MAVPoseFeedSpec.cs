using System.Linq;
using MAVLinkPack.Scripts.Pose;
using MAVLinkPack.Scripts.Util;
using NUnit.Framework;
using UnityEngine;

namespace MAVLinkPack.Editor.Pose
{
    public class MAVPoseFeedSpec
    {
        [Test]
        public void ConnectAndRead10()
        {
            var feed = MAVPoseFeed.Of(MAVPoseFeed.ArgsT.MatchAll);

            var counter = new AtomicInt();

            for (var i = 0; i < 1000; i++)
            {
                var qs = feed.Reader.Drain();

                if (qs.Count > 0) counter.Increment();

                Debug.Log($"Quaternion:\n" +
                          $"{qs.Aggregate("", (acc, q) => acc + q + "\n")}");

                if (counter.Get() > 10) break;
            }
        }

        [Test]
        public void ConnectAndUpdate()
        {
        }
    }
}