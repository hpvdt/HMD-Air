using System.Linq;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace MAVLinkPack.Editor.Pose
{
    public class ParallelQuerySpike
    {
        [Test]
        public void Dummy()
        {
            Debug.Log("dummy");
        }

        [Test]
        public void Spike()
        {
            var raw = Enumerable.Range(1, 10);

            var candidates = raw.AsParallel()
                .WithDegreeOfParallelism(16)
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism);

            var p1 = candidates.AsParallel()
                .SelectMany(
                    ii =>
                    {
                        Debug.Log(">>>>> " + ii);

                        Thread.Sleep(1000);
                        Debug.Log("<<<<< " + ii);
                        return new[] { ii };
                    }
                );

            var p2 = p1.Select(
                (v, i) => v);

            var chosen = p2.FirstOrDefault();
        }
    }
}