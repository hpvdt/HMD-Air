using System.Collections.Generic;
using UnityEngine;

namespace HMD.Scripts.UI
{
    public class PopupGroup : MonoBehaviour
    {
        private readonly HashSet<Popup> _popups = new();

        internal void Register(Popup popup)
        {
            _popups.Add(popup);
        }

        internal void Unregister(Popup popup)
        {
            _popups.Remove(popup);
        }

        public void HideAll()
        {
            foreach (var popup in _popups)
                popup.panel.SetActive(false);
        }
    }
}