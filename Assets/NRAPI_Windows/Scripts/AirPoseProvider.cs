using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
public class AirPoseProvider : BasePoseProvider
{
    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StartConnection();

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
            Debug.LogError("Connection error: return code " + code);
    }

    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StopConnection();

    public void TryDisconnect()
    {
        if (IsConnecting())
        {
            var code = StopConnection();
            if (code == 1)
            {
                connectionState = ConnectionStates.Disconnected;
                Debug.Log("Glass disconnected");
            }
            else
            {
                connectionState = ConnectionStates.Offline;
                Debug.LogWarning("Glass disconnected with error: return code " + code);
            }
        }
        else
        {
            Debug.Log("Glass not connected, no need to disconnect");
        }
    }

    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetQuaternion();

    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetEuler();

    public float mouseSensitivity = 100.0f;

    protected Quaternion FromGlasses = Quaternion.identity;

    protected static Quaternion NO_READING_Q = Quaternion.Euler(90f, 0, 0);

    protected Vector3 FromMouse_Euler = Vector3.zero;
    protected Quaternion FromMouse = Quaternion.identity;

    protected Vector3 FromZeroing_Euler = Vector3.zero;
    protected Quaternion FromZeroing = Quaternion.identity;

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
    public override bool TryGetPoseFromProvider(out Pose output)
    {
        if (IsConnecting()) UpdateFromGlasses();

        if (Input.GetMouseButton(1)) UpdateFromMouse();

        var compound = FromMouse * FromZeroing * FromGlasses;

        output = new Pose(new Vector3(0, 0, 0), compound);
        return true;
    }

    private void UpdateFromGlasses()
    {
        var ptr = GetEuler();
        var arr = new float[3];
        Marshal.Copy(ptr, arr, 0, 3);

        // Debug.Log("glasses input (Euler): " + arr);

        var reading = Quaternion.Euler(-arr[1] + 90.0f, -arr[2], -arr[0]);
        if (connectionState == ConnectionStates.StandBy)
        {
            if (!reading.Equals(NO_READING_Q))
            {
                connectionState = ConnectionStates.Connected;
                Debug.Log("Glasses connected, start reading");
                FromGlasses = reading;
            }
        }
        else
        {
            FromGlasses = reading;
        }
    }

    private void UpdateFromMouse()
    {
        var deltaY = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        var deltaX = -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        // Mouse & Unity XY axis are opposite

        FromMouse_Euler += new Vector3(deltaX, deltaY, 0.0f);

        // Debug.Log("mouse pressed:" + FromMouseXY);

        FromMouse = Quaternion.Euler(-FromMouse_Euler);
    }

    public void ZeroY()
    {
        var fromGlassesY = (FromGlasses * FromMouse).eulerAngles.y;
        FromZeroing_Euler.y = -fromGlassesY;
        FromZeroing = Quaternion.Euler(FromZeroing_Euler);
    }
}
