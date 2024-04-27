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


    void Start()
    {
        
    }

    void Update()
    {

        if (keyboardController)
        {
            Vector3 inputVector = new Vector3(0, 0, 0);

            if (Input.GetKey(KeyCode.W))
            {
                inputVector.z = +1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputVector.x = -1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                inputVector.z = -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                inputVector.x = +1;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                inputVector.y = +1;
            }
            if (Input.GetKey(KeyCode.C))
            {
                inputVector.y = -1;
            }

            transform.Translate(inputVector * movementSpeed * Time.deltaTime);
            camera.Translate(inputVector * movementSpeed * Time.deltaTime);


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


        }


        else
        {
            halo.rotation = Quaternion.Inverse(BinaryDecoder.IMU);

            Vector3 vector = new Vector3(0, -BinaryDecoder.heading, 0);

            halo.Rotate(0,-BinaryDecoder.heading,0);
        }

    }
}
