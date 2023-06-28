using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.Experimental.XR.Interaction;

public class AirPoseProvider : BasePoseProvider
{
    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StartConnection();

    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StopConnection();

    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetQuaternion();

    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetEuler();
    
    public float mouseSensitivity = 100.0f;
    
    protected int ConnectionStatus = 0;
    
    protected Quaternion FromGlasses = Quaternion.identity;
    protected float FromMouseX = 0.0f;
    protected float FromMouseY = 0.0f;
    protected Quaternion FromMouse = Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {
        // Start the connection
        ConnectionStatus =  StartConnection();
    }

    // Update Pose
    public override bool TryGetPoseFromProvider(out Pose output)
    {
        // Debug.Log("status is" + ConnectionStatus);
        
        if (ConnectionStatus == 0)
        {
            IntPtr ptr = GetEuler();
            float[] arr = new float[3];
            Marshal.Copy(ptr, arr, 0, 3);
        
            FromGlasses = Quaternion.Euler(-arr[1] + 90.0f, -arr[2], -arr[0]);
        }

        if (Input.GetMouseButton(1))
        {
            float deltaX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float deltaY = -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
            // Debug.Log("mouse pressed" + mouseX + ";" + mouseY);

            FromMouseX += deltaX;
            FromMouseY += deltaY;
        
            FromMouse = Quaternion.Euler(- FromMouseY, - FromMouseX, 0.0f);
        }
        
        Quaternion compound = FromGlasses * FromMouse;
        
        // Quaternion target = Quaternion.Euler(arr[1], -arr[2], -arr[0]);
        // Quaternion target = Quaternion.Euler(45, 0, 0);
        output = new Pose(new Vector3(0, 0, 0), compound);
        return true;
    }

    // void Update()
    // {
    //     // if (Input.GetMouseButtonDown(1))
    //     // {
    //     // }
    // }

    // Use quaternion directly
    // public override bool TryGetPoseFromProvider(out Pose output)
    // {
    //     IntPtr ptr = GetQuaternion();
    //     float[] arr = new float[4];
    //     Marshal.Copy(ptr, arr, 0, 4);
    //
    //     Quaternion target = Quaternion(arr[1], arr[2], -arr[0] + 180.0f);
    //     output = new Pose(new Vector3(0, 0, 0), target);
    //     return true;
    // }
}