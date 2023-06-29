using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Logger : MonoBehaviour
{
    public AirPoseProvider AirPoseProvider;

    private Vector3 position;
    private Quaternion orientation;
    private Vector3 euler;
    private Vector3 lastLog = new Vector3(0, 0, 0);
    private Vector3 difference;

    public float waitTime = 10.0f;
    public bool append = true;
    float timer = 0f;
    float totalTimeElapsed = 0f;

    string filePath = "C:\\Users\\Andrew\\Documents\\GitHub\\HMD-Air\\Assets\\Logger\\Log.csv";
    static StreamWriter writer;

    // Start is called before the first frame update
    void Start()
    {
        writer = new StreamWriter(filePath, append: append);
        writer.WriteLine("Time,X_axis,Y_axis,Z_axis,delta_X,delta_Y,delta_Z");

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        totalTimeElapsed += Time.deltaTime;

        if (timer > waitTime)
        {
            // get the IMU info from the AirPoseProvider Class
            orientation = AirPoseProvider.getQuaternion();
            euler = orientation.eulerAngles;
            difference = euler - lastLog;

            //Write the information to the path name csv file.
            Debug.Log(euler);
            Debug.Log(System.DateTime.Now);

            writer.WriteLine((System.DateTime.Now).ToString() + "," + (euler.x).ToString() + "," + (euler.y).ToString() + "," + (euler.z).ToString() + "," + (difference.x).ToString() + "," + (difference.y).ToString() + "," + (difference.z).ToString());

            // Subtracting wait time is more accurate over time than resetting to zero.
            timer = timer - waitTime;
            lastLog = euler;
        }
    }

    void OnDestroy()
    {
        Debug.Log("Closed");
        writer.Close();
    }
}