using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMovementController : MonoBehaviour
{

    public bool keyboardController = false;
    public float rotationSpeed = 10f;
    public float movementSpeed = 10f;
    public Transform halo;
    public Transform camera;

    Vector3 rotationVector = new Vector3(0, 0, 0);

    void Update()
    {
        float movementAmount = movementSpeed * Time.deltaTime;


        if (keyboardController)
        {



            if (Input.GetKey(KeyCode.W))
            {
                BinaryDecoder.GPSY += movementAmount;
            }
            if (Input.GetKey(KeyCode.A))
            {
                BinaryDecoder.GPSX += -movementAmount;
            }
            if (Input.GetKey(KeyCode.S))
            {
                BinaryDecoder.GPSY += -movementAmount;
            }
            if (Input.GetKey(KeyCode.D))
            {
                BinaryDecoder.GPSX += movementAmount;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                BinaryDecoder.Altimeter += movementAmount;
            }
            if (Input.GetKey(KeyCode.C))
            {
                BinaryDecoder.Altimeter += -movementAmount;
            }

            // Handle rotation inputs
            float rotationAmount = rotationSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.K))
            {
                // Rotate up
                //halo.Rotate(Vector3.right, rotationAmount);
                rotationVector += Vector3.right * rotationAmount;
            }

            if (Input.GetKey(KeyCode.I))
            {
                // Rotate down
                //halo.Rotate(Vector3.left, rotationAmount);
                rotationVector += Vector3.left * rotationAmount;

            }

            if (Input.GetKey(KeyCode.U))
            {
                // Rotate cork screw left
                //halo.Rotate(Vector3.forward, rotationAmount);
                rotationVector += Vector3.forward * rotationAmount;

            }

            if (Input.GetKey(KeyCode.O))
            {
                // Rotate cork screw right
                //halo.Rotate(Vector3.back, rotationAmount);
                rotationVector += Vector3.back * rotationAmount;

            }

            if (Input.GetKey(KeyCode.J))
            {
                BinaryDecoder.heading += rotationAmount;
            }
            if (Input.GetKey(KeyCode.L))
            {
                BinaryDecoder.heading -= rotationAmount;
            }

            Quaternion rotation = Quaternion.Euler(rotationVector);
            Debug.Log("controller: " + rotation.x + " " + rotation.y + " " + rotation.z + " " + rotation.w);

            BinaryDecoder.GyroX = rotation.x;
            BinaryDecoder.GyroY = rotation.y;
            BinaryDecoder.GyroZ = rotation.z;
            BinaryDecoder.GyroW = rotation.w;
        }

        Vector3 target = new Vector3(BinaryDecoder.GPSX, BinaryDecoder.Altimeter, BinaryDecoder.GPSY);

        Debug.Log("target: " + BinaryDecoder.GPSX + " " + BinaryDecoder.Altimeter + " " + BinaryDecoder.GPSY + "\n");
        Debug.Log("transform.position: " + transform.position.x + " " + transform.position.y + " " + transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, target, movementAmount);
        camera.position = Vector3.MoveTowards(camera.position, target, movementAmount);

        halo.rotation = Quaternion.Inverse(BinaryDecoder.IMU);
        Vector3 vector = new Vector3(0, -BinaryDecoder.heading, 0);
        halo.Rotate(0, -BinaryDecoder.heading, 0);

    }
}
