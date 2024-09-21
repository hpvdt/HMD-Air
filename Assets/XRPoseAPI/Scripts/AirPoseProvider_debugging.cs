namespace XRPoseAPI.Scripts
{
    using UnityEngine;
    public class AirPoseProvider_debugging : AirPoseProvider
    {
        class Rotation_debugging : Rotation
        {
            public Rotation_debugging(AirPoseProvider outer) : base(outer)
            {
            }

            private static Vector3 ClampTo180(Vector3 v)
            {
                return new Vector3(
                    ClampAngle180(v.x),
                    ClampAngle180(v.y),
                    ClampAngle180(v.z)
                );
            }

            private static float ClampAngle180(float angle)
            {
                angle = angle % 360;
                if (angle > 180)
                    angle -= 360;
                else if (angle < -180)
                    angle += 360;
                return angle;
            }

            protected override Quaternion Read()
            {
                var r1 = Read_direct();
                var r2 = Read_euler();

                var errorBound = 10f;
                Debug.Assert(r1 == r1.normalized, "unnormalised quaternion");
                Debug.Assert(r2 == r2.normalized, "unnormalised quaternion from Euler angle");

                // {
                //     var error = ClampTo180(r1.eulerAngles - r2.eulerAngles);
                //
                //     Debug.Assert(error.magnitude <= errorBound,
                //         $"error = {error}");
                // }

                {
                    Vector3 fwd;
                    {
                        var q = Quaternion.Inverse(r1) * r2;
                        Debug.Assert(r1 * q == r2, "fwd error!");
                        fwd = ClampTo180(q.eulerAngles);
                    }

                    Vector3 rev;
                    {
                        var q = Quaternion.Inverse(r2) * r1;
                        Debug.Assert(r2 * q == r1, "rev error!");
                        rev = ClampTo180(q.eulerAngles);
                    }

                    var angle = Quaternion.Angle(r1, r2);

                    Debug.Assert(fwd.magnitude <= errorBound,
                        $"fwd = {fwd} ; rev = {rev} ; angle = {angle}");
                }


                if (Outer.useQuaternion)
                {
                    return r1;
                }
                else
                {
                    return r2;
                }
            }
        }

        protected override Rotation Attitude
        {
            get
            {
                if (AttitudeVar == null) AttitudeVar = new Rotation_debugging(this);
                return AttitudeVar;
            }
        }
    }
}
