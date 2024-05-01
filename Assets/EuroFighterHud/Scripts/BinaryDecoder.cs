using System;
using System.Collections;
using UnityEngine;


public class BinaryDecoder : MonoBehaviour
{
    //Fields/Attributes
    public bool keyboardControl = false;

    public static float Airspeed = 9,
        Altimeter = 12,
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

    public static Quaternion IMU = new Quaternion(GyroX,GyroY,GyroZ,GyroW);
    public static Vector3 windDir = new Vector3(windX,windY,windZ);

    

    //SerialPort sp = new SerialPort();

    private void Start()
    {
        try
        {
            //sp.Open();
        }
        catch
        {
            Debug.Log("Serial port error!");
        }

        if (keyboardControl)
        {

        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!keyboardControl)
        {
            try
            {
                //string readInput = sp.ReadLine();

                //byte[] bytes = BinaryStringToByteArray(readInput);

                //string deCoded = Encoding.UTF8.GetString(bytes);

                // Test input

                string deCoded = "00110101 00101110 00110000 00100000 00110101 00101110 00110000 00100000 00110001 00101110 00110000 00100000 00110001 00101110 00110000 00100000 00110000 00101110 00110000 00100000 00101101 00110001 00101110 00110000 00100000 00110011 00110000 00101110 00110000 00100000 00110001 00110000 00101110 00110101 00100000 00110010 00110000 00101110 00110011 00100000 00110110 00110000 00110000 00101110 00110000 00100000 00110010 00101110 00110000 00100000 00110001 00101110 00110001 00100000 00110010 00110011 00101110 00110001 00100000 00110011 00110100 00101110 00110010 00100000 00111000 00101110 00111001 00100000 00110011 00101110 00110110 00100000 00110000 00101110 00110000 00110010";

                float[] dataArray = binaryDecoder(deCoded);

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

            }
        }

        
    }

    public float[] binaryDecoder(string binaryString)
    {
        string[] binaryNumbers = binaryString.Split(' ');
        byte[] bytes = new byte[4];
        float[] floats = new float[binaryNumbers.Length / 8]; // Each float takes 32 bits (4 bytes)

        for (int i = 0; i < floats.Length; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                bytes[j] = Convert.ToByte(binaryNumbers[i * 8 + j], 2);
            }

            floats[i] = BitConverter.ToSingle(bytes, 0);
        }

        return floats;
    }

    private void OnDestroy()
    {
        //sp.Close();
    }
}
