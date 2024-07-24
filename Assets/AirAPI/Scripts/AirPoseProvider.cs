using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.SpatialTracking;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class AirPoseProvider : BasePoseProvider
{
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StartConnection();

    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StopConnection();

    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetQuaternion();

    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetEuler();

#elif (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
    [DllImport("libar_drivers.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StartConnection();

    [DllImport("libar_drivers.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StopConnection();

    [DllImport("libar_drivers.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetEuler();

    [DllImport("libar_drivers.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetQuaternion();

#else
    [DllImport("libar_drivers.so", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StartConnection();

    [DllImport("libar_drivers.so", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StopConnection();

    [DllImport("libar_drivers.so", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetEuler();

    [DllImport("libar_drivers.so", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetQuaternion();

#endif

    protected enum ConnectionStates
    {
        Disconnected = 0,
        Offline = 1,
        StandBy = 2,

        Connected = 3
    }

    protected static ConnectionStates connectionState = ConnectionStates.Disconnected;

    public bool IsConnecting()
    {
        return connectionState is ConnectionStates.Connected or ConnectionStates.StandBy;
    }

    public void TryConnect()
    {
        // Start the connection
        var code = StartConnection();
        if (code == 1)
        {
            connectionState = ConnectionStates.StandBy;
            Debug.Log("Glasses standing by");
        }
        else
        {
            Debug.LogError("Connection error: return code " + code);
        }
    }


    public void TryDisconnect()
    {
        if (IsConnecting())
        {
            var code = StopConnection();
            if (code == 1)
            {
                connectionState = ConnectionStates.Disconnected;
                Debug.Log("Glassed disconnected");
            }
            else
            {
                connectionState = ConnectionStates.Offline;
                Debug.LogWarning("Glassed disconnected with error: return code " + code);
            }
        }
        else
        {
            Debug.Log("Glassed not connected, no need to disconnect");
        }
    }

    private static readonly Quaternion Qid = Quaternion.identity.normalized;

    public class Attitude
    {
        public Quaternion Glasses = Quaternion.identity;
        public Quaternion Mouse = Quaternion.identity;
        public Quaternion Zeroing = Quaternion.identity;

        private Vector3 MouseEuler = Vector3.zero;

        private Vector3 ZeroingEuler = Vector3.zero;

        private float mouseSensitivity = 100.0f;

        private float[] GetEulerArray()
        {
            var ptr = GetEuler();
            var r = new float[3];
            Marshal.Copy(ptr, r, 0, 3);
            return r;
        }

        private static readonly Quaternion QNeutral = Quaternion.Euler(90f, 0f, 0f).normalized;

        // the following rules are chosen to be
        //  compatible with alternative Windows driver (https://github.com/wheaney/OpenVR-xrealAirGlassesHMD)

        protected Quaternion Read_direct()
        {
            var ptr = GetQuaternion();
            // receiving in WIJK order (left hand)
            // see https://github.com/xioTechnologies/Fusion/blob/e7d2b41e6506fa9c85492b91becf003262f14977/Fusion/FusionMath.h#L36

            var arr = new float[4];
            Marshal.Copy(ptr, arr, 0, 4);

            var qRaw = new Quaternion(-arr[1], -arr[3], -arr[2], arr[0]);

            // Debug.Log($"Quaternion before: {qRaw.x}, {qRaw.y}, {qRaw.z}, {qRaw.w}");

            // converting to IKJW order (right hand)
            // sequence (1, -3, -2, 0) is for chiral conversion
            // see https://stackoverflow.com/questions/28673777/convert-quaternion-from-right-handed-to-left-handed-coordinate-system
            // neutral position is 90 degree pitch downward

            var q = qRaw * QNeutral;
            // Debug.Log($"Quaternion after: {q.x}, {q.y}, {q.z}, {q.w}");
            return q;
        }

        protected Quaternion Read_euler()
        {
            var arr = GetEulerArray();
            // receiving in FRU order (left hand axes, right hand rotation)
            // Forward - roll
            // Right - pitch
            // Up - yaw

            var roll = arr[0];
            var pitch = arr[1];
            var yaw = arr[2];

            var arr2 = new Vector3(-(pitch - 90f), -yaw, -roll);
            // Debug.Log($"Euler: {arr2[0]}, {arr2[1]}, {arr2[2]}");

            // converting to LDB order (right hand axes, right hand rotation)
            // Left - pitch
            // Down - yaw
            // Backward - roll
            // neutral position is 90 degree pitch downward

            var r = Quaternion.Euler(arr2[0], arr2[1], arr2[2]);
            return r;
        }


        protected virtual Quaternion Read()
        {
            var r2 = Read_euler();
            return r2;
        }

        public void UpdateFromGlasses()
        {
            var reading = Read();

            var effective = reading;
            // var effective = (reading * Q_NEUTRAL).normalized;

            if (connectionState == ConnectionStates.StandBy)
            {
                if (!reading.Equals(Qid))
                {
                    connectionState = ConnectionStates.Connected;
                    Debug.Log("Glasses connected, start reading");
                    Glasses = effective;
                }
            }
            else
            {
                Glasses = effective;
            }
        }

        public void UpdateFromMouse()
        {
            var deltaY = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            var deltaX = -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            // Mouse & Unity XY axis are opposite

            MouseEuler += new Vector3(deltaX, deltaY, 0.0f);

            // Debug.Log("mouse pressed:" + FromMouseXY);

            Mouse = Quaternion.Euler(-MouseEuler);
        }

        public void ZeroY()
        {
            var fromGlassesY = (Glasses * Mouse).eulerAngles.y;
            ZeroingEuler.y = -fromGlassesY;
            Zeroing = Quaternion.Euler(ZeroingEuler);
        }
    }

    protected Attitude _attitude = new Attitude();

    public class Translation // TODO: enable it
    {
        private Vector3 _fromGlasses = Vector3.zero;

        private Vector3 _fromMouse = Vector3.zero;
    }

    // Start is called before the first frame update
    private void Start()
    {
        TryConnect();
    }

    private void OnDestroy()
    {
        TryDisconnect();
    }

    // Update Pose
    public override PoseDataFlags GetPoseFromProvider(out Pose output)
    {
        if (IsConnecting()) _attitude.UpdateFromGlasses();

        if (Input.GetMouseButton(1)) _attitude.UpdateFromMouse();

        var compound = _attitude.Mouse * _attitude.Zeroing * _attitude.Glasses;

        output = new Pose(new Vector3(0, 0, 0), compound);
        return PoseDataFlags.Rotation;
    }
}
