using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followRotation : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform leader;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = leader.rotation;
    }
}
