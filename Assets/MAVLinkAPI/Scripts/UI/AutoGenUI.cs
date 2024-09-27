using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace MAVLinkAPI.Scripts.UI
{
    public abstract class AutoUIGeneratorUGUI<T> : MonoBehaviour
    {
        // public T original; // can only bind once
        public RectTransform uiRoot;

        private object _boxed;
        // public object Boxed => LazyHelper.EnsureInitialized(ref _boxed, () => original);

        [SerializeField]
        public T Value
        {
            get => (T)_boxed;
            set => _boxed = value;
        }

        public void Start()
        {
            GenerateUIForStruct(uiRoot);
        }

        public void GenerateUIForStruct(RectTransform container)
        {
            foreach (var field in typeof(T).GetFields())
            {
                var element = CreateElementForField(field);
                if (element != null)
                    element.SetParent(container, false);
            }
        }

        private RectTransform CreateElementForField(FieldInfo field)
        {
            var go = new GameObject(field.Name);
            var rt = go.AddComponent<RectTransform>();

            switch (field.FieldType.Name)
            {
                case "String":
                    var inputField = go.AddComponent<InputField>();
                    inputField.text = (string)field.GetValue(_boxed);
                    inputField.onValueChanged.AddListener((value) => { field.SetValue(_boxed, value); });
                    return rt;
                case "Int32":
                    var intInput = go.AddComponent<InputField>();
                    intInput.text = ((int)field.GetValue(_boxed)).ToString();
                    intInput.onValueChanged.AddListener((value) =>
                    {
                        if (int.TryParse(value, out var result)) field.SetValue(_boxed, result);
                    });
                    return rt;
                case "Single":
                    var floatInput = go.AddComponent<InputField>();
                    floatInput.text = ((float)field.GetValue(_boxed)).ToString();
                    floatInput.onValueChanged.AddListener((value) =>
                    {
                        if (float.TryParse(value, out var result)) field.SetValue(_boxed, result);
                    });
                    return rt;
                default:
                    return null;
            }
        }
    }
}