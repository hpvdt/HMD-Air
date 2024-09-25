using System.Linq;
using MAVLinkAPI.Scripts.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace MAVLinkAPI.Editor.UI
{
    [TestFixture]
    public class AutoUIGeneratorUGUITests
    {
        private GameObject canvasGO;
        private AutoUIGeneratorUGUI generator;

        [SetUp]
        public void SetUp()
        {
            canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
            var uiRoot = new GameObject("UIRoot").AddComponent<RectTransform>();
            uiRoot.SetParent(canvas.transform, false);
        
            generator = canvasGO.AddComponent<AutoUIGeneratorUGUI>();
            generator.uiRoot = uiRoot;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(canvasGO);
        }

        [Test]
        public void GenerateUIForStruct_CreatesCorrectElements()
        {
            generator.playerData = new PlayerData { name = "Test", level = 5, health = 100f };
            generator.Start();

            var uiElements = generator.uiRoot.GetComponentsInChildren<InputField>();
            Assert.AreEqual(3, uiElements.Length);

            var nameField = uiElements.First(f => f.gameObject.name == "name");
            var levelField = uiElements.First(f => f.gameObject.name == "level");
            var healthField = uiElements.First(f => f.gameObject.name == "health");

            Assert.IsNotNull(nameField);
            Assert.IsNotNull(levelField);
            Assert.IsNotNull(healthField);

            Assert.AreEqual("Test", nameField.text);
            Assert.AreEqual("5", levelField.text);
            Assert.AreEqual("100", healthField.text);
        }

        [Test]
        public void UIElements_UpdateStructWhenChanged()
        {
            generator.playerData = new PlayerData();
            generator.Start();

            var uiElements = generator.uiRoot.GetComponentsInChildren<InputField>();
            var nameField = uiElements.First(f => f.gameObject.name == "name");
            var levelField = uiElements.First(f => f.gameObject.name == "level");
            var healthField = uiElements.First(f => f.gameObject.name == "health");

            nameField.text = "NewPlayer";
            nameField.onValueChanged.Invoke("NewPlayer");
            levelField.text = "10";
            levelField.onValueChanged.Invoke("10");
            healthField.text = "75.5";
            healthField.onValueChanged.Invoke("75.5");

            Assert.AreEqual("NewPlayer", generator.playerData.name);
            Assert.AreEqual(10, generator.playerData.level);
            Assert.AreEqual(75.5f, generator.playerData.health);
        }
    }
}