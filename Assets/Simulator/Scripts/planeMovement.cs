using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeMovement : MonoBehaviour
{

    [SerializeField] Vector3 targetPosition;
    [SerializeField] float velocity = 10f;
    [SerializeField] float smoothTime;
    Vector3 forward = new Vector3(0, (float)0.15, -1);
    bool canMove = false;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            transform.Translate(forward * Time.deltaTime * velocity, Space.World);
        }
    }

    public void setCanMove(bool a)
    {
        canMove = a;
    }
}
