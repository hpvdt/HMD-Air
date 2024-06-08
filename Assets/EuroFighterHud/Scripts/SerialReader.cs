using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SerialReader : MonoBehaviour
{
    [Header("Serial Port")]
    public string portName = "/dev/tty.usbserial-1120";  // Set the port name (e.g., /dev/ttyUSB0 for Linux or /dev/tty.usbserial-* for Mac)
    public int baudRate = 115200;             // Set the baud rate

    private SerialPort serialPort;
    private Thread readThread;
    private bool isRunning;

    private byte[] buffer = new byte[4];  // Larger buffer to read multiple floats
    private int bufferIndex = 0;
    private object bufferLock = new object();

    private float[] dataArray = new float[17];
    private int counter = 0;

    //UNITY DATA

    public static float Airspeed = 9,
        Altimeter = 0,
        GyroX = 0,
        GyroY = 0,
        GyroZ = 0,
        GyroW = 0,
        heading = 0,
        GPSX = 0,
        GPSY = 0,
        totalEnergy = 0,
        energyLossRate = 0,
        efficiency = 0,
        temp = 0,
        pressure = 0,
        windX = 0,
        windY = 0,
        windZ = 0;

    public static Quaternion IMU = new Quaternion(GyroX, GyroY, GyroZ, GyroW);
    public static Vector3 windDir = new Vector3(windX, windY, windZ);



    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 50;
        serialPort.WriteTimeout = 50;

        try
        {
            serialPort.Open();
            isRunning = true;
            readThread = new Thread(ReadSerialData);
            readThread.Start();
            Debug.Log("Serial port opened successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error opening serial port: {e.Message}");
        }
    }

    void Update()
    {
        lock (bufferLock)
        {
            while (bufferIndex >= 4)
            {
                float receivedFloat = BitConverter.ToSingle(buffer, 0);
                Debug.Log($"Received Float: {receivedFloat}");

                // Add the received float to the list
                dataArray[counter] = receivedFloat;
                counter += 1;

                // Shift the buffer
                Array.Copy(buffer, 4, buffer, 0, bufferIndex - 4);
                bufferIndex -= 4;

                if (counter > 16)
                {
                    UpdateData();
                    counter = 0;
                }
            }
        }
    }

    void ReadSerialData()
    {
        while (isRunning && serialPort.IsOpen)
        {
            try
            {
                int bytesRead = serialPort.Read(buffer, bufferIndex, buffer.Length - bufferIndex);
                if (bytesRead > 0)
                {
                    lock (bufferLock)
                    {
                        bufferIndex += bytesRead;
                    }
                }
            }
            catch (TimeoutException)
            {
                // Handle the timeout exception if needed
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading from serial port: {e.Message}");
            }
        }
    }

    void UpdateData()
    {

        if (dataArray.Length == 17)
        {
            Airspeed = dataArray[0];
            Altimeter = dataArray[1];

            GyroX = dataArray[2];
            GyroY = dataArray[3];
            GyroZ = dataArray[4];
            GyroW = dataArray[5];
            heading = dataArray[6];

            GPSX = dataArray[7];
            GPSY = dataArray[8];

            totalEnergy = dataArray[9];
            energyLossRate = dataArray[10];
            efficiency = dataArray[11];

            temp = dataArray[12];
            pressure = dataArray[13];

            windX = dataArray[14];
            windY = dataArray[15];
            windZ = dataArray[16];

            IMU.Set(GyroX, GyroY, GyroZ, GyroW);
            windDir.Set(windX, windY, windZ);
        }

        
    }

    void PrintData()
    {
        string print = "";

        foreach (float value in dataArray)
        {
            print += (" " + value.ToString());
        }
        Debug.Log(print);
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join();
        }

        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }

        Debug.Log("Serial port closed.");

        // Optionally, log all the collected floats
        foreach (var value in dataArray)
        {
            Debug.Log(value);
        }
    }
}
