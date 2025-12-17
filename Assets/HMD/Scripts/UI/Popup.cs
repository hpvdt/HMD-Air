using Autofill;
using MAVLinkAPI.Util.NullSafety;
using UnityEngine;
using UnityEngine.UI;

namespace HMD.Scripts.UI
{
    public class Popup : MonoBehaviour
    {
        [Autofill(AutofillType.Parent)] public PopupGroup group = null!;

        [Autofill] [Required] public Button toggleButton = null!;

        [Required] public GameObject panel = null!;

        private void Awake()
        {
            if (group == null)
                group = GetComponentInParent<PopupGroup>();

            if (group == null)
            {
                Debug.LogError(
                    $"{nameof(Popup)} requires a {nameof(PopupGroup)} in parent hierarchy or assigned in inspector",
                    this);
                return;
            }

            if (toggleButton == null)
            {
                Debug.LogError($"{nameof(Popup)} requires {nameof(toggleButton)} to be assigned", this);
                return;
            }

            group.Register(this);
            toggleButton.onClick.AddListener(OnToggleClicked);
        }

        private void OnDestroy()
        {
            if (toggleButton != null)
                toggleButton.onClick.RemoveListener(OnToggleClicked);

            if (group != null)
                group.Unregister(this);
        }

        private void OnToggleClicked()
        {
            TogglePopup();
        }

        //Enable a GameObject if it is disabled, or disable it if it is enabled
        private bool TogglePopup()
        {
            var toggled = !panel.activeInHierarchy;
            group.HideAll();

            panel.SetActive(toggled);
            return toggled;
        }
    }
}