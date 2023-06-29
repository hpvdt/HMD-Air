using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public planeMovement planeMovement;
    public Transform player; // Reference to the player game object
    private Vector3 offset; // Offset from the player position
    private Vector3 forward = new(0, (float)0.35, -1);
    private Vector3[] sf = new Vector3[2];
    [SerializeField] private float velocity = 10f;
    private bool canMove = false;
    [SerializeField] private bool freeCamera = false;
    private int count = 0;

    //Camera side view coord:  (100, 15, 0)
    //Camera front view coord: (0, 15, -100)

    private void Start()
    {
        if (!freeCamera)
        {
            cameraMove(50, 15, 0);
        }
        else
        {
            offset = transform.position - player.position;
            canMove = true;
        }
    }

    private void Update()
    {
        if (canMove)
        {
            player.Translate(forward * Time.deltaTime * velocity, Space.World);
            transform.Translate(forward * Time.deltaTime * velocity, Space.World);
        }
        else if (Input.GetMouseButtonDown(0) && count < 2 && !freeCamera)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.point);
                sf[count] = hit.point;
                count++;
            }

            if (count == 1) cameraMove(0, 15, -50);
        }
        else if (count == 2)
        {
            cameraMove(sf[0].x, (sf[1].y + sf[1].y) / 2, sf[0].z);
            canMove = true;
        }
    }

    private void cameraMove(float a, float b, float c)
    {
        transform.position = new Vector3(a, b, c);
    }
}