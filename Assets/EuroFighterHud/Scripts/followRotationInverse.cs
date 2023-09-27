using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followRotationInverse : MonoBehaviour
{
    public Transform leader;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Inverse(leader.rotation);
    }
}
