using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeMovement : MonoBehaviour
{

    [SerializeField] Vector3 targetPosition;
    [SerializeField] float velocity = 3f;
    [SerializeField] float smoothTime;
    Vector3 forward = new Vector3(1, 0, 0);

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(forward * Time.deltaTime * velocity, Space.World);
    }
}
