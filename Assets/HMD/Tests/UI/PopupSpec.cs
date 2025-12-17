using HMD.Scripts.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace HMD.Tests.UI
{
    public class PopupSpec
    {
        [Test]
        public void ToggleButton_ShowsAndHidesPopupPanel_AndHidesOtherPopupsInGroup()
        {
            var groupGo = new GameObject("PopupGroup");
            var group = groupGo.AddComponent<PopupGroup>();

            var panel1 = new GameObject("Panel1");
            panel1.SetActive(false);
            var panel2 = new GameObject("Panel2");
            panel2.SetActive(false);

            var button1Go = new GameObject("Button1");
            var button1 = button1Go.AddComponent<Button>();
            var button2Go = new GameObject("Button2");
            var button2 = button2Go.AddComponent<Button>();

            var popup1Go = new GameObject("Popup1");
            popup1Go.SetActive(false);
            var popup1 = popup1Go.AddComponent<Popup>();
            popup1.group = group;
            popup1.toggleButton = button1;
            popup1.panel = panel1;
            popup1Go.SetActive(true);

            var popup2Go = new GameObject("Popup2");
            popup2Go.SetActive(false);
            var popup2 = popup2Go.AddComponent<Popup>();
            popup2.group = group;
            popup2.toggleButton = button2;
            popup2.panel = panel2;
            popup2Go.SetActive(true);

            Assert.IsFalse(panel1.activeSelf);
            Assert.IsFalse(panel2.activeSelf);

            button1.onClick.Invoke();
            Assert.IsTrue(panel1.activeSelf);
            Assert.IsFalse(panel2.activeSelf);

            button2.onClick.Invoke();
            Assert.IsFalse(panel1.activeSelf);
            Assert.IsTrue(panel2.activeSelf);

            button2.onClick.Invoke();
            Assert.IsFalse(panel1.activeSelf);
            Assert.IsFalse(panel2.activeSelf);

            Object.DestroyImmediate(popup1Go);
            Object.DestroyImmediate(popup2Go);
            Object.DestroyImmediate(button1Go);
            Object.DestroyImmediate(button2Go);
            Object.DestroyImmediate(panel1);
            Object.DestroyImmediate(panel2);
            Object.DestroyImmediate(groupGo);
        }

        [Test]
        public void CenterXYIfOutsideGroup_WhenInsideGroup_DoesNotCenter()
        {
            var groupGo = new GameObject("PopupGroup");
            var groupRect = groupGo.AddComponent<RectTransform>();
            groupRect.sizeDelta = new Vector2(100.0f, 100.0f);
            groupRect.pivot = new Vector2(0.5f, 0.5f);

            var popupGo = new GameObject("Popup");
            var popupRect = popupGo.AddComponent<RectTransform>();
            popupRect.sizeDelta = new Vector2(10.0f, 10.0f);
            popupRect.pivot = new Vector2(0.5f, 0.5f);
            popupRect.SetParent(groupRect, false);
            popupRect.localPosition = new Vector3(10.0f, 20.0f, 3.0f);

            var didCenter = Popup.IsOutsideGroup(popupGo, groupGo);

            Assert.IsFalse(didCenter);
            Assert.AreEqual(new Vector3(10.0f, 20.0f, 3.0f), popupRect.localPosition);

            Object.DestroyImmediate(popupGo);
            Object.DestroyImmediate(groupGo);
        }

        [Test]
        public void CenterXYIfOutsideGroup_WhenOutsideGroup_CentersXYAndPreservesZ()
        {
            var groupGo = new GameObject("PopupGroup");
            var groupRect = groupGo.AddComponent<RectTransform>();
            groupRect.sizeDelta = new Vector2(100.0f, 100.0f);
            groupRect.pivot = new Vector2(0.5f, 0.5f);

            var popupGo = new GameObject("Popup");
            var popupRect = popupGo.AddComponent<RectTransform>();
            popupRect.sizeDelta = new Vector2(10.0f, 10.0f);
            popupRect.pivot = new Vector2(0.5f, 0.5f);
            popupRect.SetParent(groupRect, false);
            popupRect.localPosition = new Vector3(100.0f, 0.0f, 3.0f);

            var didCenter = Popup.IsOutsideGroup(popupGo, groupGo);

            Assert.IsTrue(didCenter);
            Assert.AreEqual(new Vector3(0.0f, 0.0f, 3.0f), popupRect.localPosition);

            Object.DestroyImmediate(popupGo);
            Object.DestroyImmediate(groupGo);
        }

        [Test]
        public void ToggleButton_WhenNoGroupAssigned_TogglesPanelWithoutThrowing()
        {
            var panel = new GameObject("Panel");
            panel.SetActive(false);

            var buttonGo = new GameObject("Button");
            var button = buttonGo.AddComponent<Button>();

            var popupGo = new GameObject("Popup");
            popupGo.SetActive(false);
            var popup = popupGo.AddComponent<Popup>();
            popup.toggleButton = button;
            popup.panel = panel;
            popupGo.SetActive(true);

            Assert.IsFalse(panel.activeSelf);

            button.onClick.Invoke();
            Assert.IsTrue(panel.activeSelf);

            button.onClick.Invoke();
            Assert.IsFalse(panel.activeSelf);

            Object.DestroyImmediate(popupGo);
            Object.DestroyImmediate(buttonGo);
            Object.DestroyImmediate(panel);
        }
    }
}