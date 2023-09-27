using UnityEngine;

public class KeyboardRotationController : MonoBehaviour
{
    public Transform targetObject; // The object whose rotation you want to follow
    public float rotationSpeed = 100f; // Speed of rotation in degrees per second

    private void Update()
    {
        if (targetObject != null)
        {
            // Copy the rotation from the target object
            transform.rotation = targetObject.rotation;

            // Handle rotation inputs
            float rotationAmount = rotationSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.S))
            {
                // Rotate up
                transform.Rotate(Vector3.right, rotationAmount);
            }

            if (Input.GetKey(KeyCode.W))
            {
                // Rotate down
                transform.Rotate(Vector3.left, rotationAmount);
            }

            if (Input.GetKey(KeyCode.A))
            {
                // Rotate left
                transform.Rotate(Vector3.down, rotationAmount);
            }

            if (Input.GetKey(KeyCode.D))
            {
                // Rotate right
                transform.Rotate(Vector3.up, rotationAmount);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                // Rotate cork screw left
                transform.Rotate(Vector3.forward, rotationAmount);
            }

            if (Input.GetKey(KeyCode.E))
            {
                // Rotate cork screw right
                transform.Rotate(Vector3.back, rotationAmount);
            }
        }
        else
        {
            Debug.LogWarning("Target object is not assigned.");
        }
    }
}