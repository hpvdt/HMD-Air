using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followRotation : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform leader;

    public bool inverse = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (!inverse)
        {
            transform.rotation = leader.rotation;
        }
        else
        {
            transform.rotation = Quaternion.Inverse(leader.rotation);
        }

    }
}
