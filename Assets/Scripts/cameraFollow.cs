using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player game object
    public float smoothTime = 0.3f; // Time it takes for the camera to reach its target position
    public Vector3 offset; // Offset from the player position

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // Sets the camera's location and direction before first frame
        transform.position = player.position + offset;
        transform.LookAt(player);
    }

    void LateUpdate()
    {
        // Create a target position by adding the player's position and the offset
        Vector3 targetPosition = player.position + offset;

        // Smoothly move the camera towards the target position using Mathf.SmoothDamp()
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        
        
    }
}

