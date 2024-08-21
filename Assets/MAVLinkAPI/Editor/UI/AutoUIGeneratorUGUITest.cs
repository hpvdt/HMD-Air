using System;
using System.Linq;
using MAVLinkAPI.Scripts.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MAVLinkAPI.Editor.UI
{
    [Serializable]
    public struct PlayerData
    {
        public string name;
        public int level;
        public float health;
    }

    public class PlayerUIGen : AutoUIGeneratorUGUI<PlayerData>
    {
    }

    [TestFixture]
    public class AutoUIGeneratorUGUITests
    {
        private GameObject canvasGO;
        private PlayerUIGen _generatorUGUI;

        [SetUp]
        public void SetUp()
        {
            canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var uiRoot = new GameObject("UIRoot").AddComponent<RectTransform>();
            uiRoot.SetParent(canvas.transform, false);

            _generatorUGUI = canvasGO.AddComponent<PlayerUIGen>();
            _generatorUGUI.uiRoot = uiRoot;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(canvasGO);
        }

        // [Test] // this will not pass,
        // see https://stackoverflow.com/questions/6280506/is-there-a-way-to-set-properties-on-struct-instances-using-reflection
        public void SetValueSpike()
        {
            // Arrange
            var playerData = new PlayerData
            {
                name = "Test Player",
                level = 1,
                health = 100f
            };

            // Act
            var healthField = typeof(PlayerData).GetField("health");
            healthField.SetValue(playerData, 75.5f);

            // Assert
            Assert.AreEqual(75.5f, playerData.health);
        }

        [Test]
        public void GenerateUIForStruct_CreatesCorrectElements()
        {
            _generatorUGUI.Value = new PlayerData { name = "Test", level = 5, health = 100f };
            _generatorUGUI.Start();

            var uiElements = _generatorUGUI.uiRoot.GetComponentsInChildren<InputField>();
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
            var uiRoot = new GameObject("UIRoot").AddComponent<RectTransform>();
            var autoUIGenerator = uiRoot.gameObject.AddComponent<PlayerUIGen>();
            autoUIGenerator.Value = new PlayerData();
            autoUIGenerator.uiRoot = uiRoot;

            autoUIGenerator.GenerateUIForStruct(uiRoot);

            var uiElements = uiRoot.GetComponentsInChildren<InputField>();
            var nameField = uiElements.First(f => f.gameObject.name == "name");
            var levelField = uiElements.First(f => f.gameObject.name == "level");
            var healthField = uiElements.First(f => f.gameObject.name == "health");

            nameField.text = "NewPlayer";
            nameField.onValueChanged.Invoke("NewPlayer");
            levelField.text = "10";
            levelField.onValueChanged.Invoke("10");
            healthField.text = "75.5";
            healthField.onValueChanged.Invoke("75.5");

            Assert.AreEqual("NewPlayer", autoUIGenerator.Value.name);
            Assert.AreEqual(10, autoUIGenerator.Value.level);
            Assert.AreEqual(75.5f, autoUIGenerator.Value.health);

            Object.DestroyImmediate(uiRoot.gameObject);
        }
    }
}