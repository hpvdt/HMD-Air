using UnityEngine;

namespace MAVLinkAPI.Scripts.Util
{
    public static class UnityQuaternionExtensions
    {
        public static Quaternion InvertPitch(this Quaternion q)
        {
            return new Quaternion(-q.x, q.y, -q.z, q.w);
            // return q;
        }

        public abstract class FrameT
        {
            public abstract Quaternion From(float w, float x, float y, float z);

            public Quaternion FromWXYZ(float[] q)
            {
                return From(q[0], q[1], q[2], q[3]);
            }

            public Quaternion FromXYZW(float[] q)
            {
                return From(q[1], q[2], q[3], q[0]);
            }
        }

        public class AeronauticFrameT : FrameT
        {
            public override Quaternion From(float w, float x, float y, float z)
            {
                var mapped = new Quaternion(y, z, x, w);

                var bias = Quaternion.Euler(0, 180, 0);

                return mapped * bias;
                // return new Quaternion(0, 0, 0, 1);
            }
        }

        public static readonly AeronauticFrameT AeronauticFrame = new();
    }
}