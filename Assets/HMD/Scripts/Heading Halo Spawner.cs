using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadingHaloSpawner : MonoBehaviour
{
    
    [Header("Parameters")]
    public float radius = 10f;
    public float angleBetweenMajor = 30f;
    public float angleBetweenPins = 30f;

    public GameObject pin;

    
    // Start is called before the first frame update
    void Start()
    {
        int numberOfIterations = (int)(360/angleBetweenPins);
        int numberBetweenMajors = (int)(angleBetweenMajor/angleBetweenPins);
        float degree = 0f;

        Debug.Log(numberOfIterations);
        for (int i = 0; i < numberOfIterations; i++)
        {
            GameObject newPin = Instantiate(pin, GetPinLocation(degree), Quaternion.identity, this.transform);
            if (i % numberBetweenMajors == 0)
            {
                newPin.transform.localScale += new Vector3(0,0.1f,0);
            }
            degree += angleBetweenPins;
            Debug.Log(degree);
        }
    }


    public Vector3 GetPinLocation(float degree)
    {
        float rad = degree * Mathf.Deg2Rad;
        float x = Mathf.Sin(rad) * radius;
        float z = Mathf.Cos(rad) * radius;
        return new Vector3(x, 0 ,z);
    }
}
