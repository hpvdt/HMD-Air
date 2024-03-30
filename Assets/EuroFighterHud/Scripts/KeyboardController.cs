using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    /*
     * Built purely for testing purposes.
     * 
     * Allows you to control rotation of the HMD overlay and translation of the camera and overlay (Body)
     */

    public Transform targetObject; // The object whose rotation you want to follow
    public Transform dataDisplay;
    public Transform camera;
    public float rotationSpeed = 100f; // Speed of rotation in degrees per second
    Vector3 inputVector = new Vector3();

    private void Update()
    {
        if (targetObject != null)
        {
            // Copy the rotation from the target object
            transform.rotation = targetObject.rotation;

            inputVector = new Vector3(0, 0, 0);

            if (Input.GetKey(KeyCode.W))
            {
                inputVector.y = +1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputVector.x = -1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                inputVector.y = -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                inputVector.x = +1;
            }

            // Handle rotation inputs
            float rotationAmount = rotationSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.K))
            {
                // Rotate up
                transform.Rotate(Vector3.right, rotationAmount);
            }

            if (Input.GetKey(KeyCode.I))
            {
                // Rotate down
                transform.Rotate(Vector3.left, rotationAmount);
            }

            if (Input.GetKey(KeyCode.U))
            {
                // Rotate cork screw left
                transform.Rotate(Vector3.forward, rotationAmount);
            }

            if (Input.GetKey(KeyCode.O))
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
