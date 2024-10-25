using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeadingHaloSpawner : MonoBehaviour
{
    
    [Header("Parameters")]
    public float radius = 10f;

    public float fontSize = 10f;

    public float angleBetweenMajor = 30f;
    public float angleBetweenPins = 30f;

    public GameObject pin;

    TMP_Text newText;

    
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
                GameObject textObject = new GameObject("WorldTextObject");
                TextMeshPro textMeshPro = textObject.AddComponent<TextMeshPro>();
                
                textMeshPro.transform.parent = this.transform;
                textMeshPro.transform.position = newPin.transform.position + (Vector3.up);
                
                textMeshPro.alignment = TextAlignmentOptions.Center;
                textMeshPro.text = degree.ToString();
                textMeshPro.fontSize = this.fontSize;
                textMeshPro.color = new Color32(0, 255, 30, 255);
                textMeshPro.transform.LookAt(new Vector3(textMeshPro.transform.position.x * 2,textMeshPro.transform.position.y,textMeshPro.transform.position.z * 2));
            
            }
            degree += angleBetweenPins;
        }
    }
    
    public Vector3 GetPinLocation(float degree)
    {
        float rad = degree * Mathf.Deg2Rad;
        float x = Mathf.Sin(rad) * radius;
        float z = Mathf.Cos(rad) * radius;
        return new Vector3(x,this.transform.position.y ,z);
    }
}
