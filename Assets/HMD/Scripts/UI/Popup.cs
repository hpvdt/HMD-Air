#nullable enable
 
 using Autofill;
 using MAVLinkAPI.Util.NullSafety;
 using UnityEngine;
 using UnityEngine.UI;

 namespace HMD.Scripts.UI
 {
     public class Popup : MonoBehaviour
     {
         [Autofill(AutofillType.Parent)]
         public PopupGroup? group;

         [Autofill] [Required] public Button toggleButton = null!;

         [Required] public GameObject panel = null!;

         public static void CenterXY(GameObject o)
         {
             o.transform.localPosition = new Vector3(
                 0.0f,
                 0.0f,
                 o.transform.localPosition.z
             );
         }

         public static bool IsOutsideGroup(GameObject o, GameObject group)
         {
             var groupRect = group.GetComponent<RectTransform>();
             var objRect = o.GetComponent<RectTransform>();

             if (groupRect == null || objRect == null)
                 return false;

            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(groupRect, objRect);
            var rect = groupRect.rect;

            var isInside =
                bounds.min.x >= rect.xMin &&
                bounds.max.x <= rect.xMax &&
                bounds.min.y >= rect.yMin &&
                bounds.max.y <= rect.yMax;


             return !isInside;
         }

        private void Awake()
        {
            group?.Register(this);
            toggleButton.onClick.AddListener(OnToggleClicked);

            if (group != null && IsOutsideGroup(panel, group.gameObject))
            {
                CenterXY(panel);
                panel.SetActive(false);
            }
        }

        private void OnToggleClicked()
        {
            TogglePopup();
        }

        //Enable a GameObject if it is disabled, or disable it if it is enabled
        private bool TogglePopup()
        {
            var toggled = !panel.activeInHierarchy;
            group?.HideAll();

            panel.SetActive(toggled);
            return toggled;
        }

        public void ClosePopup()
        {
            group?.HideAll();
            panel.SetActive(false);
        }
    }
 }