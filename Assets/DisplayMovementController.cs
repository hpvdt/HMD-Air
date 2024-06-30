using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMovementController : MonoBehaviour
{

    public bool keyboardController = false;
    public float rotationSpeed = 10f;
    public float movementSpeed = 10f;
    public Transform compassHalo;
    public Transform windHalo;
    public Transform camera;
    public float windOscillationSpeed;

    Vector3 rotationVector = new Vector3(0, 0, 0);

    void Update()
    {
        float movementAmount = movementSpeed * Time.deltaTime;


        if (keyboardController)
        {
            if (Input.GetKey(KeyCode.W))
            {
                SerialReader.GPSY += movementAmount;
            }
            if (Input.GetKey(KeyCode.A))
            {
                SerialReader.GPSX += -movementAmount;
            }
            if (Input.GetKey(KeyCode.S))
            {
                SerialReader.GPSY += -movementAmount;
            }
            if (Input.GetKey(KeyCode.D))
            {
                SerialReader.GPSX += movementAmount;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                SerialReader.Altimeter += movementAmount;
            }
            if (Input.GetKey(KeyCode.C))
            {
                SerialReader.Altimeter += -movementAmount;
            }

            // Handle rotation inputs
            float rotationAmount = rotationSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.K))
            {
                // Rotate up
                rotationVector += Vector3.right * rotationAmount;
            }

            if (Input.GetKey(KeyCode.I))
            {
                // Rotate down
                rotationVector += Vector3.left * rotationAmount;
            }

            if (Input.GetKey(KeyCode.U))
            {
                // Rotate cork screw left
                rotationVector += Vector3.forward * rotationAmount;
            }

            if (Input.GetKey(KeyCode.O))
            {
                // Rotate cork screw right
                rotationVector += Vector3.back * rotationAmount;
            }

            if (Input.GetKey(KeyCode.J))
            {
                SerialReader.heading += rotationAmount;
            }
            if (Input.GetKey(KeyCode.L))
            {
                SerialReader.heading -= rotationAmount;
            }

            if (SerialReader.heading > 360)
            {
                SerialReader.heading -= 360;
            }
            else if (SerialReader.heading < 0)
            {
                SerialReader.heading += 360;
            }

            Quaternion rotation = Quaternion.Euler(rotationVector);

            SerialReader.IMU = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);

            SerialReader.windX = Mathf.Sin(Time.time * windOscillationSpeed);
            SerialReader.windY = Mathf.Cos(Time.time * windOscillationSpeed);
        }

        Vector3 target = new Vector3(SerialReader.GPSX, SerialReader.Altimeter, SerialReader.GPSY);

        transform.position = Vector3.MoveTowards(transform.position, target, movementAmount);
        camera.position = Vector3.MoveTowards(camera.position, target, movementAmount);

        //Calculate degree of rotation for wind halo

        //Perform rotations
        compassHalo.rotation = Quaternion.Inverse(SerialReader.IMU);
        windHalo.rotation = Quaternion.Inverse(SerialReader.IMU);
        compassHalo.Rotate(0, -SerialReader.heading, 0);
        windHalo.Rotate(0, GetAngleFromVector(SerialReader.windDir), 0);
        
    }

    public float GetAngleFromVector(Vector3 vector)
    {
        // Calculate the angle in radians
        float angleInRadians = Mathf.Atan2(vector.y, vector.x);

        // Convert the angle to degrees
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg;

        // Ensure the angle is between 0 and 360 degrees
        if (angleInDegrees < 0)
        {
            angleInDegrees += 360;
        }
        else if (angleInDegrees > 360)
        {
            angleInDegrees -= 360;
        }

        return angleInDegrees;
    }
}
