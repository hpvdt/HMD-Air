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

    void Update()
    {

        if (keyboardController)
        {
            //Vector3 inputVector = new Vector3(0, 0, 0);

            if (Input.GetKey(KeyCode.W))
            {
                BinaryDecoder.GPSY += movementSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                BinaryDecoder.GPSX += -movementSpeed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                BinaryDecoder.GPSY += -movementSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                BinaryDecoder.GPSX += movementSpeed;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                BinaryDecoder.Altimeter += movementSpeed;
            }
            if (Input.GetKey(KeyCode.C))
            {
                BinaryDecoder.Altimeter += -movementSpeed;
            }

            Vector3 target = new Vector3(BinaryDecoder.GPSX, BinaryDecoder.Altimeter, BinaryDecoder.GPSY);
            //Debug.Log("target: " + BinaryDecoder.GPSX + " " + BinaryDecoder.GPSY + " " + BinaryDecoder.Altimeter + "\n");
            //Debug.Log("transform.position: " + transform.position.x + " " + transform.position.y + " " + transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);




            // Handle rotation inputs
            float rotationAmount = rotationSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.K))
            {
                // Rotate up

                halo.Rotate(Vector3.right, rotationAmount);
            }

            if (Input.GetKey(KeyCode.I))
            {
                // Rotate down
                halo.Rotate(Vector3.left, rotationAmount);
            }

            if (Input.GetKey(KeyCode.U))
            {
                // Rotate cork screw left
                halo.Rotate(Vector3.forward, rotationAmount);
            }

            if (Input.GetKey(KeyCode.O))
            {
                // Rotate cork screw right
                halo.Rotate(Vector3.back, rotationAmount);
            }

            if (Input.GetKey(KeyCode.J))
            {
                BinaryDecoder.heading += rotationAmount;
            }
            if (Input.GetKey(KeyCode.L))
            {
                BinaryDecoder.heading -= rotationAmount;
            }

            halo.rotation = Quaternion.Inverse(BinaryDecoder.IMU);
            Vector3 vector = new Vector3(0, -BinaryDecoder.heading, 0);
            halo.Rotate(0, -BinaryDecoder.heading, 0);
        }

        else
        {
            halo.rotation = Quaternion.Inverse(BinaryDecoder.IMU);

            Vector3 vector = new Vector3(0, -BinaryDecoder.heading, 0);

            halo.Rotate(0, -BinaryDecoder.heading, 0);
        }

    }
}
