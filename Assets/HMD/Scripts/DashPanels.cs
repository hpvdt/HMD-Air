#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using HMD.Scripts.Streaming;
using HMD.Scripts.Util;
using MAVLinkAPI.Util;
using MAVLinkAPI.Util.NullSafety;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HMD.Scripts
{
    public class DashPanels : MonoBehaviourWithLogging
    {
        // private const string WHATS_NEW = "OnboardingSeen_0_0_5_g";

        private const string NEW_VLC_WINDOWS = "New VLC ...";

        private const string NEW_V_CAP = "New Video Capture ...";
        // [HideInInspector]
        // public VlcController controller;

        [Required] public GameObject playerParent = null!;
        [Required] public GameObject vlcPlayerTemplate = null!;
        [Required] public GameObject vCapPlayerTemplate = null!;

        [Required] public Dropdown playerMenu = null!;

        [Required] public FOVController fovController = null!;

        [Required] public GameObject playerTab = null!;
        [Required] public GameObject locatorTab = null!;
        [Required] public GameObject trackTab = null!;
        [Required] public GameObject volumeTab = null!;

        public Button? playerMove2DButton;
        public Button? playerMove3DButton;

        [Required] public GameObject optionsButton = null!;

        [Required] public GameObject appMenu = null!;

        [Required] public GameObject aspectRatioPopup = null!;
        [Required] public GameObject formatPopup = null!;
        [Required] public GameObject pictureSettingsPopup = null!;
        [Required] public GameObject releaseInfoPopup = null!;


        // the following are set in `UpdateReferences`

        [Required] public GameObject rootMenu = null!;
        [Required] public GameObject screenPopup = null!;

        [Required] public GameObject lockScreenNotice = null!;

        [Required] public Text versionInfo = null!;

        private readonly Dictionary<string, Player> _activePlayers = new();

        private readonly AtomicLong _incCounter = new();


        private List<Display>? _extendDisplay;

        private string FocusedPlayerID
        {
            get => playerMenu.options[playerMenu.value].text;
            set
            {
                var newIndex = playerMenu.options.FindIndex(option => option.text == value);

                if (newIndex == -1) throw new IndexOutOfRangeException($"cannot find player {value}");

                playerMenu.value = newIndex;
                playerMenu.RefreshShownValue();

                Log.V($"player menu set to {newIndex} | {playerMenu.value}");
            }
        }

        public List<Player> FocusedPlayers
        {
            get
            {
                var p = GetFocusedPlayer();
                return p.Wrap().ToList();

                // var result = new List<Player> { GetFocusedPlayer()! };
                // return result.Where(v => v != null).ToList();
            }
        }

        private Maybe<List<GameObject>> _allMenus;

        private List<GameObject> AllMenus => _allMenus.Lazy(() => new List<GameObject>
        {
            rootMenu,
            appMenu
        });

        private List<GameObject> PopupsNotCentered => new List<GameObject>
        {
            aspectRatioPopup,
            screenPopup,
            formatPopup,
            releaseInfoPopup,
            pictureSettingsPopup,
        };

        private void CenterPopupLocations()
        {
            // Get the "Popups" game object, then loop over each of it's top-level children
            // and center them on the screen

            foreach (var popup in PopupsNotCentered)
                // Log.V("centering " + childGameObject.name);
                CenterXY(popup);
        }

        private Maybe<List<GameObject>> _allPopups;

        private List<GameObject> AllPopups => _allPopups.Lazy(() => PopupsNotCentered
            .Concat(new[] { playerTab, locatorTab, trackTab, volumeTab })
            .ToList());


        // Start is called before the first frame update
        private void Start()
        {
            BindUI();

            var versionName = Application.version;
            var versionCode = Application.buildGUID;
            versionInfo.text =
                $"{versionName} ({versionCode})";


            // center UI things that i had spread out in Editor
            CenterPopupLocations();

            // Center Menus/Objects
            CenterXY(lockScreenNotice);
            CenterXY(rootMenu);
            CenterXY(appMenu);

            lockScreenNotice.SetActive(false);

            HideAllPopups();
            HideAllMenus();

            ShowRootMenu();
        }


        // private Player? _focusedPlayer;

        private void Update()
        {
            foreach (var player in FocusedPlayers)
                if (player?.Dragging is Player.DraggingMode._3D v)
                {
                    var rotation = v.Offset * fovController.mainCamera.transform.rotation;
                    player.Prefab.transform.rotation = rotation;
                }
                else if (player?.Dragging is Player.DraggingMode._2D v2)
                {
                    var rotation = v2.Offset * fovController.mainCamera.transform.rotation.DropRoll();
                    player.Prefab.transform.rotation = rotation;
                }
            // TODO: add 2D
        }

        private Player? GetFocusedPlayer()
        {
            return _activePlayers.GetValueOrDefault(FocusedPlayerID);
        }

        private Player _setupPlayerFromPrefab(GameObject prefab, string playerName)
        {
            prefab.SetActive(true);
            var id = playerName + "(" + _incCounter.Increment() + ")";

            var player = new Player { Outer = this, Prefab = prefab, ID = id };
            _activePlayers.Add(id, player);

            playerMenu.options.Add(new Dropdown.OptionData(player.ID));
            playerMenu.RefreshShownValue();

            return player;
        }

        private Player _setupPlayerFromTemplate(GameObject template, string prefix, bool focus = true)
        {
            var heading = fovController.mainCamera.transform.rotation;


            var prefab =
                Instantiate(template, Vector3.zero, heading, playerParent.transform);

            var player = _setupPlayerFromPrefab(prefab, prefix);

            if (focus) player.Focus();

            return player;
        }

        public void SetupVlc()
        {
            _setupPlayerFromTemplate(vlcPlayerTemplate, "VLC");
        }


        public void SetupVCap()
        {
            _setupPlayerFromTemplate(vCapPlayerTemplate, "Video Capture");
        }


        private void _syncIcons()
        {
            foreach (var player in _activePlayers.Values) player.IconIsVisible = false;

            if (playerTab.activeInHierarchy)
                foreach (var player in FocusedPlayers)
                    player.IconIsVisible = true;
        }

        public void HideIcons()
        {
        }

        // TODO: this should be bind to a new button in Subsystem
        private List<Display> ExtendDisplayOnce() // In Unity, display cannot be scrapped
        {
            if (_extendDisplay == null)
            {
                Debug.Log("displays connected: " + Display.displays.Length);
                // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
                // Check if additional displays are available and activate each.

                var result = Display.displays.Skip(1).ToList();

                foreach (var d in result)
                {
                    Debug.Log("display" + d.systemWidth + "x" + d.systemHeight + " : " + d.renderingWidth + "x"
                              + d.renderingHeight);
                    d.Activate();
                }

                _extendDisplay = result;
                return result;
            }

            return _extendDisplay!;
        }

        public void TogglePlayerTab()
        {
            // ExtendDisplayOnce();
            TogglePopup(playerTab);

            _syncIcons();
        }

        public void ToggleLocatorTab()
        {
            TogglePopup(locatorTab);
        }

        public void ToggleTrackTab()
        {
            TogglePopup(trackTab);
        }

        public void ToggleVolumeTab()
        {
            TogglePopup(volumeTab);
        }

        //Enable a GameObject if it is disabled, or disable it if it is enabled
        private bool TogglePopup(GameObject element)
        {
            var toggled = !element.activeInHierarchy;
            HideAllPopups();

            element.SetActive(toggled);
            return toggled;
        }


        private void CenterXY(GameObject o)


        {
            o.transform.localPosition = new Vector3(
                0.0f,
                0.0f,
                o.transform.localPosition.z
            );
        }

        // private void OnApplicationFocus(bool hasFocus)
        // { TODO: remove, useless
        // }


        private void BindUI()
        {
            // playerDropdown.RefreshShownValue();

            playerMenu.options.Add(new Dropdown.OptionData(NEW_VLC_WINDOWS));
            playerMenu.options.Add(new Dropdown.OptionData(NEW_V_CAP));
            playerMenu.RefreshShownValue();

            playerMenu.onValueChanged.AddListener(value =>
                {
                    var option = playerMenu.options[value];

                    if (option.text == NEW_VLC_WINDOWS)
                    {
                        SetupVlc();
                    }
                    else if (option.text == NEW_V_CAP)
                    {
                        SetupVCap();
                    }
                    else if (option.text == "")
                    {
                        // do nothing
                    }
                    else
                    {
                        var selected = _activePlayers[option.text];
                        selected.Focus();
                    }
                }
            );


            if (playerMove3DButton != null)
            {
                // 3D
                playerMove3DButton.OnEvent(EventTriggerType.PointerDown)
                    .AddListener(_ =>
                        {
                            Log.V("dragging in 3D ...");
                            foreach (var player in FocusedPlayers)
                                player.Dragging = new Player.DraggingMode._3D
                                {
                                    Offset = player.Prefab.transform.rotation
                                             * Quaternion.Inverse(fovController.mainCamera.transform.rotation)
                                };
                        }
                    );

                playerMove3DButton.OnEvent(EventTriggerType.PointerUp)
                    .AddListener(_ =>
                        {
                            Log.V("... done");
                            foreach (var player in FocusedPlayers) player.Dragging = Player.DraggingMode.Disabled;
                        }
                    );
            }

            if (playerMove2DButton != null)
            {
                // 2D
                playerMove2DButton.OnEvent(EventTriggerType.PointerDown)
                    .AddListener(_ =>
                        {
                            Log.V("dragging in 2D ...");
                            foreach (var player in FocusedPlayers)
                            {
                                var withoutRoll = fovController.mainCamera.transform.rotation.DropRoll();

                                player.Dragging = new Player.DraggingMode._2D
                                {
                                    Offset = player.Prefab.transform.rotation * Quaternion.Inverse(withoutRoll)
                                };
                            }
                        }
                    );

                playerMove2DButton.OnEvent(EventTriggerType.PointerUp)
                    .AddListener(_ =>
                        {
                            Log.V("... done");
                            foreach (var player in FocusedPlayers) player.Dragging = Player.DraggingMode.Disabled;
                        }
                    );
            }
        }

        public void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        public void ShowAppMenu()
        {
            HideAllMenus();
            // UpdateReferences();

            appMenu.SetActive(true);
            CenterXY(appMenu);
        }

        public void ShowRootMenu()
        {
            HideAllMenus();
            rootMenu.SetActive(true);
            optionsButton.SetActive(true);
        }

        private void HideAllMenus()
        {
            foreach (var p in AllMenus) p.SetActive(false);
        }

        public void HideAllPopups()
        {
            foreach (var p in AllPopups) p.SetActive(false);
        }

        public void ShowAspectRatioPopup()
        {
            TogglePopup(aspectRatioPopup);
        }

        public void ShowScreenPopup()
        {
            TogglePopup(screenPopup);
        }

        public void ShowFormatPopup()
        {
            TogglePopup(formatPopup);
        }

        public void ShowWhatsNewPopup()
        {
            TogglePopup(releaseInfoPopup);
        }

        public void ShowPictureSettingsPopup()
        {
            TogglePopup(pictureSettingsPopup);
        }

        public class Player : HasOuter<DashPanels>, IDisposable
        {
            private ControllerLike? _controller;

            private Maybe<ControllerLike> _controllerExisting;

            public DraggingMode Dragging = DraggingMode.Disabled;
            public string ID = null!;

            public GameObject Prefab = null!;

            public ControllerLike Controller =>
                _controllerExisting.Lazy(() =>
                {
                    var controller = Prefab.GetComponent<ControllerLike>();
                    _controller = controller;
                    return controller;
                });

            public bool IconIsVisible
            {
                set => Controller.icon.SetActive(value);
            }

            public void Dispose()
            {
                Destroy(Prefab);
                Outer._activePlayers.Remove(ID);
            }

            public void Focus()
            {
                Controller.BindUI();
                Outer.FocusedPlayerID = ID;

                Outer._syncIcons();
            }

            public class DraggingMode
            {
                public static readonly DraggingMode Disabled = new();

                public class Enabled : DraggingMode
                {
                    public Quaternion Offset;
                }

                public class _2D : Enabled
                {
                }

                public class _3D : Enabled
                {
                }
            }
        }

        public class Lock // TODO: it should be used
        {
            private float _brightnessOnLock;
            private bool _screenLocked;

            private readonly GameObject _hideWhenLocked = null!;

            private readonly GameObject _lockScreenNotice = null!;

            private readonly GameObject _logo = null!;

            private readonly GameObject _menuToggleButton = null!;

            public void ToggleScreenLock()
            {
                _screenLocked = !_screenLocked;

                if (_screenLocked)
                {
                    // Hide All UI except for the lock button
                    _hideWhenLocked.SetActive(false);
                    _lockScreenNotice.SetActive(true);
                    _logo.SetActive(false);
                    _menuToggleButton.SetActive(false);
                    // Lower Brightness
                    var unityBrightnessOnLock = Screen.brightness;
                    Debug.Log($"lockbrightness Unity brightness on lock {unityBrightnessOnLock}");

                    _brightnessOnLock = Screen.brightness;
                }
                else
                {
                    // Restore Brightness
                    Screen.brightness = _brightnessOnLock;

                    // Show All UI when screen is unlocked
                    _hideWhenLocked.SetActive(true);
                    _lockScreenNotice.SetActive(false);
                    _logo.SetActive(true);
                    _menuToggleButton.SetActive(true);
                }
            }
        }
    }
}