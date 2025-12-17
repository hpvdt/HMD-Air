using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
namespace HMD.Editor.AgentScript
{
    public class RemoveEmptyTextComponents : MonoBehaviour
    {
        [MenuItem("Tools/Agent Scripts/Remove Empty Text Components")]
        public static void RemoveEmptyTextComponentsMenu()
        {
            int removedCount = 0;

            // Find all Text components in the scene
            Text[] textComponents = FindObjectsOfType<Text>(true);
            foreach (Text textComp in textComponents)
            {
                if (string.IsNullOrEmpty(textComp.text))
                {
                    Debug.Log($"Found empty Text component on GameObject: {textComp.gameObject.name}");
                    DestroyImmediate(textComp);
                    removedCount++;
                }
            }

            // Find all TextMeshProUGUI components in the scene
            TextMeshProUGUI[] tmpComponents = FindObjectsOfType<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI tmpComp in tmpComponents)
            {
                if (string.IsNullOrEmpty(tmpComp.text))
                {
                    Debug.Log($"Found empty TextMeshProUGUI component on GameObject: {tmpComp.gameObject.name}");
                    DestroyImmediate(tmpComp);
                    removedCount++;
                }
            }

            // Find all TextMeshPro components (3D text)
            TextMeshPro[] tmp3DComponents = FindObjectsOfType<TextMeshPro>(true);
            foreach (TextMeshPro tmp3DComp in tmp3DComponents)
            {
                if (string.IsNullOrEmpty(tmp3DComp.text))
                {
                    Debug.Log($"Found empty TextMeshPro component on GameObject: {tmp3DComp.gameObject.name}");
                    DestroyImmediate(tmp3DComp);
                    removedCount++;
                }
            }

            Debug.Log($"RemoveEmptyTextComponents: Removed {removedCount} empty text components");
        }

        [MenuItem("Tools/Agent Scripts/Find Empty Text Components")]
        public static void FindEmptyTextComponentsMenu()
        {
            int emptyCount = 0;

            // Find all Text components in the scene
            Text[] textComponents = FindObjectsOfType<Text>(true);
            foreach (Text textComp in textComponents)
            {
                if (string.IsNullOrEmpty(textComp.text))
                {
                    Debug.Log($"Empty Text component found on GameObject: {textComp.gameObject.name}");
                    emptyCount++;
                }
            }

            // Find all TextMeshProUGUI components in the scene
            TextMeshProUGUI[] tmpComponents = FindObjectsOfType<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI tmpComp in tmpComponents)
            {
                if (string.IsNullOrEmpty(tmpComp.text))
                {
                    Debug.Log($"Empty TextMeshProUGUI component found on GameObject: {tmpComp.gameObject.name}");
                    emptyCount++;
                }
            }

            // Find all TextMeshPro components (3D text)
            TextMeshPro[] tmp3DComponents = FindObjectsOfType<TextMeshPro>(true);
            foreach (TextMeshPro tmp3DComp in tmp3DComponents)
            {
                if (string.IsNullOrEmpty(tmp3DComp.text))
                {
                    Debug.Log($"Empty TextMeshPro component found on GameObject: {tmp3DComp.gameObject.name}");
                    emptyCount++;
                }
            }

            Debug.Log($"FindEmptyTextComponents: Found {emptyCount} empty text components");
        }
    }
#endif
}