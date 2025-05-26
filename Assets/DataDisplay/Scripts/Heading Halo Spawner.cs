namespace DataDisplay.Scripts
{
#if UNITY_TEXTMESHPRO
    using TMPro;
#endif
    using UnityEngine;

    public class HeadingHaloSpawner : MonoBehaviour
    {
        [Header("Parameters")] public float radius = 10f;

        public float fontSize = 10f;

        public float angleBetweenMajor = 30f;
        public float angleBetweenPins = 30f;

        public GameObject pin;

#if UNITY_TEXTMESHPRO
        private TMP_Text newText;
#else
        private UnityEngine.UI.Text newText;
#endif

        // Start is called before the first frame update
        private void Start()
        {
            var numberOfIterations = (int)(360 / angleBetweenPins);
            var numberBetweenMajors = (int)(angleBetweenMajor / angleBetweenPins);
            var degree = 0f;

            Debug.Log(numberOfIterations);
            for (var i = 0; i < numberOfIterations; i++)
            {
                var newPin = Instantiate(pin, GetPinLocation(degree), Quaternion.identity, transform);
                if (i % numberBetweenMajors == 0)
                {
                    newPin.transform.localScale += new Vector3(0, 0.1f, 0);
                    var textObject = new GameObject("WorldTextObject");
#if UNITY_TEXTMESHPRO
                    var textMeshPro = textObject.AddComponent<TextMeshPro>();
#else
                    var textMeshPro = textObject.AddComponent<UnityEngine.UI.Text>();
#endif

                    textMeshPro.transform.parent = transform;
                    textMeshPro.transform.position = newPin.transform.position + Vector3.up;

#if UNITY_TEXTMESHPRO
                    textMeshPro.alignment = TextAlignmentOptions.Center;
                    textMeshPro.fontSize = fontSize;
#else
                    textMeshPro.alignment = TextAnchor.MiddleCenter;
                    textMeshPro.fontSize = (int)fontSize;
#endif
                    textMeshPro.text = degree.ToString();
#if UNITY_TEXTMESHPRO
                    textMeshPro.color = new Color32(0, 255, 30, 255);
#else
                    textMeshPro.color = new Color(0, 1, 0.1176470588f, 1);
#endif
                    textMeshPro.transform.LookAt(new Vector3(textMeshPro.transform.position.x * 2,
                        textMeshPro.transform.position.y, textMeshPro.transform.position.z * 2));
                }

                degree += angleBetweenPins;
            }
        }

        public Vector3 GetPinLocation(float degree)
        {
            var rad = degree * Mathf.Deg2Rad;
            var x = Mathf.Sin(rad) * radius;
            var z = Mathf.Cos(rad) * radius;
            return new Vector3(x, transform.position.y, z);
        }
    }
}