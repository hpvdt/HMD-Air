using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaloController : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform leader;

    public BinaryDecoder binaryDecoder;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Inverse(leader.rotation);

        float degree = binaryDecoder.getHeading();

        Vector3 vector = new Vector3(0, -degree, 0);

        transform.Rotate(vector);
    }


}
