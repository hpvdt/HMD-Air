using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using UnityEditor.PackageManager.UI;
using UnityEngine;


public class BinaryDecoder : MonoBehaviour
{
    //Fields/Attributes
    public static float Airspeed = 9, Altimeter = 12,
            GyroX = 0, GyroY = 0, GyroZ = 0, GyroW = 0,
            heading = 0,
            GPSX = 0, GPSY = 0,
            totalEnergy = 0, energyLossRate = 0, efficiency = 0,
            temp = 0, pressure = 0,
            windX = 0, windY = 0, windZ = 0;

    private Quaternion IMU;
    private Vector3 windDir;

    //SerialPort sp = new SerialPort();

    //getters
    public float getAirspeed() { return Airspeed; }
    public float getAltimeter() { return Altimeter; }
    public Quaternion getIMU() { return IMU; }
    public float getHeading() { return heading; }
    public float getGPSX() { return GPSX; }
    public float getGPSY() { return GPSY; }
    public float getTotalEnergy() { return totalEnergy; }
    public float getEnergyLossRate() { return energyLossRate; }
    public float getEfficiency() { return efficiency; }
    public float getTemp() { return temp; }
    public float getAirPressure() { return pressure; }
    public Vector3 getWinDir() { return windDir; }

    void Start()
    {
        try
        {
            //sp.Open();
        }
        catch
        {
            Debug.Log("Serial port error!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            //string readInput = sp.ReadLine();

            byte[] bytes = BinaryStringToByteArray(readInput);

            //string final = Encoding.UTF8.GetString(bytes);

            string final = "00110101 00101110 00110000 00100000 00110101 00101110 00110000 00100000 00110001 00101110 00110000 00100000 00110001 00101110 00110000 00100000 00110000 00101110 00110000 00100000 00101101 00110001 00101110 00110000 00100000 00110011 00110000 00101110 00110000 00100000 00110001 00110000 00101110 00110101 00100000 00110010 00110000 00101110 00110011 00100000 00110110 00110000 00110000 00101110 00110000 00100000 00110010 00101110 00110000 00100000 00110001 00101110 00110001 00100000 00110010 00110011 00101110 00110001 00100000 00110011 00110100 00101110 00110010 00100000 00111000 00101110 00111001 00100000 00110011 00101110 00110110 00100000 00110000 00101110 00110000 00110010";

            string[] stringArray = final.Split(" ");

            float[] dataArray = new float[stringArray.Length];

            for (int i = 0; i < stringArray.Length; i++)
            {
                dataArray[i] = float.Parse(stringArray[i]);
            }


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
        catch
        {
            Debug.Log("Nuh Uh");
        }
    }

    public static byte[] BinaryStringToByteArray(string binaryString)
    {
        if (binaryString.Length % 8 != 0)
        {
            throw new ArgumentException("Binary string length must be a multiple of 8.");
        }

        int byteCount = binaryString.Length / 8;

        byte[] byteArray = new byte[byteCount];

        for (int i = 0; i < byteCount; i++)
        {
            string byteString = binaryString.Substring(i * 8, 8);
            byteArray[i] = Convert.ToByte(byteString, 2);
        }

        return byteArray;
    }

    public void movement() 
    {
        transform.rotation = IMU;

    }

    private void OnDestroy()
    {
        //sp.Close();
    }
}
