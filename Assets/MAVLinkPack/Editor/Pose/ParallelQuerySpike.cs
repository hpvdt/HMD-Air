using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace MAVLinkPack.Editor.Pose
{
    public class ParallelQuerySpike
    {
        [Test]
        public void LINQPredicatePushDown()
        {
            // Debug.Log("dummy");

            var numbers = Enumerable.Range(1, 10);

            var n2 = numbers.Select(
                v =>
                {
                    Debug.Log(v);
                    // some long operation
                    return v;
                }
            );

            var query1 = n2.Where(n => n == 2); // Even numbers
            var query2 = n2.Where(n => n == 3); // Numbers divisible by 3

            var unionResult = query1.Union(query2).ToList();

            foreach (var i in unionResult) Console.WriteLine(i);
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