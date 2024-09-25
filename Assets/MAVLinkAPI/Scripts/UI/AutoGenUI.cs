using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace MAVLinkAPI.Scripts.UI
{
    [System.Serializable]
    public struct PlayerData
    {
        public string name;
        public int level;
        public float health;
    }

    public class AutoUIGeneratorUGUI : MonoBehaviour
    {
        public PlayerData playerData;
        public RectTransform uiRoot;

        public void Start()
        {
            GenerateUIForStruct(uiRoot, playerData);
        }

        void GenerateUIForStruct<T>(RectTransform container, T data) where T : struct
        {
            foreach (var field in typeof(T).GetFields())
            {
                var element = CreateElementForField(field, data);
                if (element != null)
                    element.SetParent(container, false);
            }
        }

        RectTransform CreateElementForField<T>(FieldInfo field, T data) where T : struct
        {
            GameObject go = new GameObject(field.Name);
            RectTransform rt = go.AddComponent<RectTransform>();
        
            switch (field.FieldType.Name)
            {
                case "String":
                    var inputField = go.AddComponent<InputField>();
                    inputField.text = (string)field.GetValue(data);
                    inputField.onValueChanged.AddListener((value) => field.SetValue(data, value));
                    return rt;
                case "Int32":
                    var intInput = go.AddComponent<InputField>();
                    intInput.text = ((int)field.GetValue(data)).ToString();
                    intInput.onValueChanged.AddListener((value) => 
                    {
                        if (int.TryParse(value, out int result))
                            field.SetValue(data, result);
                    });
                    return rt;
                case "Single":
                    var floatInput = go.AddComponent<InputField>();
                    floatInput.text = ((float)field.GetValue(data)).ToString();
                    floatInput.onValueChanged.AddListener((value) => 
                    {
                        if (float.TryParse(value, out float result))
                            field.SetValue(data, result);
                    });
                    return rt;
                default:
                    return null;
            }
        }
    }
}