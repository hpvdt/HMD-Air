#nullable enable
using MAVLinkAPI.Util.NullSafety;
using UnityEngine;
using UnityEngine.InputSystem;


namespace HMD.Scripts
{
    public class KeyMapHandler : MonoBehaviour
    {
        public static KeyMapHandler? Instance { get; private set; }

        [Required] public GameObject dashUI = null!;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void Update()
        {
            // TODO: can F1 be generalised?
#if UNITY_EDITOR
            if (Keyboard.current?.f1Key.wasPressedThisFrame ?? false)
                UnityEditor.EditorWindow.focusedWindow.maximized = !UnityEditor.EditorWindow.focusedWindow.maximized;
#endif
            if (Keyboard.current?.escapeKey.wasPressedThisFrame ?? false)
                ToggleDashUIVisibility();
        }

        private void ToggleDashUIVisibility()
        {
            dashUI.SetActive(!dashUI.activeSelf);
        }
    }
}