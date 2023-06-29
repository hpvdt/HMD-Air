using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO.Ports;

public class hudBrain : MonoBehaviour
{
    Quaternion orientation;
    Vector3 windDirection;
    
    SerialPort sp = new SerialPort();

    string[] splitStringSensorData = new string[16];
    float[] sensorData = new float[16];

    //Airspeed         0
    //Altimeter        1
    //orientation.x    2
    //orientation.y    3
    //orientation.z    4
    //orientation.w    5
    //GPS.x            6
    //GPS.y            7
    //totalEnergy      8
    //energyLoss       9
    //efficiency      10
    //thermometer     11
    //barometer       12
    //windDirection.x 13
    //windDirection.y 14
    //windDirection.z 15

    //SerialPort binary order of information 

    //Airspeed, Altimeter,  Gyroscope X axis, Gyroscope Y axis, Gyroscope Z axis, Gyroscope W axis, GPS X axis, GPS Y axis, total energy, energy loss rate, efficiency, thermometer, barometer, wind direction X axis, wind direction Y axis, wind direction Z axis

    // Start is called before the first frame update
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
            //I hate these variable names. They're too generic.
            //string binaryString = sp.ReadLine();
            string binaryString = "0011001000101110001100000011010000110011001000000011011000101110001100000011001000100000001101010010111000110011001101000011010100110101001100100011001100100000001100010010111000110100001101010011010000110011001101100011000000100000001100100011000100101110001101000011001100110101001100100011010000110011001000000011000100101110001100000011000000110000001100000011000000110000001000000011010000110101001011100011001100110110001100000011001000110100001000000011001000111000001011100011010000110101001100100011000000110101001000000011001000110000001100000010111000110000001100000010000000110001001101100010111000110000001100000010000000110010001011100011000000110000001000000011001000110001001011100011000000110011001000000011000100111001001100010010111000110000001100000010000000110001001100110010111000110101001000000011000100111000001100010010111000110000001100110010000000110000001011100011000000110101";
            byte[] binaryData = new byte[binaryString.Length / 8];
            for (int i = 0; i < binaryData.Length; i++)
            {
                string byteString = binaryString.Substring(i * 8, 8);
                binaryData[i] = Convert.ToByte(byteString, 2);
            }

            string stringData = Encoding.UTF8.GetString(binaryData);

            splitStringSensorData = stringData.Split(" ");

            for (int i = 0; i < sensorData.Length; i++)
            {
                sensorData[i] = float.Parse(splitStringSensorData[i]);
            }
            

        }

        catch
        {

        }

        
        //Import Serial binary data into unity. 

        //convert binary data to float values 

        //Move assigned Indicator objects according to float info
            //Airspeed: (float)
             //Altimeter: (float)
            //Vertical speed indicator: possibly derived from Attitude indicator and airspeed
            //Total energy measure: (float)
            //Rate of energy loss (float)
            //Efficiency measure: (float)
            //thermometer, barometer: (float)
            //plane relative wind speed (Vector)
            //GPS coordinates: 2 floats


            //Attitude indicator(pitch): (Quaternion)
            //Heading compass: (Quaternion)
            //Turn coordinator(roll and yaw): (Quaternion)

    }
}
