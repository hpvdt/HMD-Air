#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using HMD.Scripts.Streaming;
using HMD.Scripts.UI;
using HMD.Scripts.Util;
using MAVLinkAPI.Util;
using MAVLinkAPI.Util.NullSafety;
using MAVLinkAPI.Util.Resource;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HMD.Scripts
{
    public class DashPanels : MonoBehaviourWithLogging
    {
        // private const string WHATS_NEW = "OnboardingSeen_0_0_5_g";


        // [HideInInspector]
        // public VlcController controller;

        [Required] public GameObject playerParent = null!;

        [Required] public LifetimeBinding lifetimeBinding = null!;

        private const string NEW_VLC_WINDOWS = "New VLC ...";
        [Required] public GameObject vlcPlayerTemplate = null!;

        private const string NEW_V_CAP = "New Video Capture ...";
        [Required] public GameObject vCapPlayerTemplate = null!;

        [Required] public FOVController fovController = null!;

        [Required] public GameObject playerTab = null!;

        public Button? playerMove2DButton;
        public Button? playerMove3DButton;

        public Button? playerFlipXButton;
        public Button? playerFlipYButton;

        [Required] public Button playerDestroyButton = null!;

        [Required] public GameObject optionsButton = null!;

        [Required] public GameObject appMenu = null!;

        // the following are set in `UpdateReferences`

        [Required] public GameObject rootMenu = null!;

        [Required] public GameObject lockScreenNotice = null!;

        [Required] public Text versionInfo = null!;

        [Required] public Dropdown playerMenu = null!;
        private readonly Dictionary<string, Player> _activePlayers = new();

        private readonly AtomicLong _incCounter = new();

        private string FocusedPlayerID
        {
            get
            {
                if (playerMenu.options.Count == 0) return "";
                if (playerMenu.value >= playerMenu.options.Count) playerMenu.value = 0;
                return playerMenu.options[playerMenu.value].text;
            }
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

        private Player? GetFocusedPlayer()
        {
            return _activePlayers.GetValueOrDefault(FocusedPlayerID);
        }

        private Maybe<List<GameObject>> _allMenus;

        private List<GameObject> AllMenus => _allMenus.Lazy(() => new List<GameObject>
        {
            rootMenu,
            appMenu
        });


        // Start is called before the first frame update
        private GameObjectActiveStateRelay? _playerTabActiveStateRelay;

        protected new void Awake()
        {
            base.Awake();

            if (playerTab == null)
                return;

            var relay = playerTab.GetComponent<GameObjectActiveStateRelay>();
            if (relay == null) relay = playerTab.AddComponent<GameObjectActiveStateRelay>();

            _playerTabActiveStateRelay = relay;
            relay.ActiveStateChanged -= _syncIcons;
            relay.ActiveStateChanged += _syncIcons;

            _syncIcons();
        }

        private void OnDestroy()
        {
            if (_playerTabActiveStateRelay != null)
                _playerTabActiveStateRelay.ActiveStateChanged -= _syncIcons;
        }

        private void Start()
        {
            BindUI();

            var versionName = Application.version;
            var versionCode = Application.buildGUID;
            versionInfo.text =
                $"{versionName} ({versionCode})";


            // Center Menus/Objects
            Popup.CenterXY(lockScreenNotice);
            Popup.CenterXY(rootMenu);
            Popup.CenterXY(appMenu);

            lockScreenNotice.SetActive(false);

            HideAllMenus();

            ShowRootMenu();
        }

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
        }


        private Player AddPlayer(GameObject prefab, string textID)
        {
            var player = new Player(lifetimeBinding.Lifetime) { Outer = this, Prefab = prefab, TextID = textID };

            _activePlayers.Add(player.TextID, player);
            playerMenu.options.Add(new Dropdown.OptionData(player.TextID));
            playerMenu.RefreshShownValue();
            return player;
        }


        private Player _setupPlayerFromPrefab(GameObject prefab, string playerName)
        {
            var textID = playerName + "(" + _incCounter.Increment() + ")";

            prefab.SetActive(true);
            var player = new Player(lifetimeBinding.Lifetime) { Outer = this, Prefab = prefab, TextID = textID };
            player.Setup();

            return player;
        }

        private Player _setupPlayerFromTemplate(GameObject template, string prefix, bool focus = true)
        {
            var heading = fovController.mainCamera.transform.rotation;

            var prefab = Instantiate(template, Vector3.zero, heading, playerParent.transform);

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

        private void BindUI()
        {
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

            playerFlipXButton?.onClick.AddListener(() =>
                {
                    foreach (var player in FocusedPlayers)
                    {
                        player.VideoController.Feed.invertedX ^= true;
                    }
                }
            );


            playerFlipYButton?.onClick.AddListener(() =>
                {
                    foreach (var player in FocusedPlayers)
                    {
                        player.VideoController.Feed.invertedY ^= true;
                    }
                }
            );

            playerDestroyButton.onClick.AddListener(() =>
                {
                    foreach (var player in FocusedPlayers)
                    {
                        player.Dispose();
                    }
                }
            );
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
            Popup.CenterXY(appMenu);
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


        public class Player : Cleanable
        {
            public Player(Lifetime? lifetime = null) : base(lifetime)
            {
            }

            public DashPanels Outer { get; init; } = null!;
            public string TextID { get; init; } = null!;
            public GameObject Prefab { get; init; } = null!;

            public DraggingMode Dragging = DraggingMode.Disabled;

            private Maybe<VideoControllerLike> _videoController;

            public VideoControllerLike VideoController =>
                _videoController.Lazy(() =>
                {
                    var controller = Prefab.GetComponent<VideoControllerLike>();
                    return controller;
                });

            public bool IconIsVisible
            {
                set => VideoController.icon.SetActive(value);
            }


            public override void DoClean()
            {
                TearDown();
            }

            public void Setup()
            {
                Outer._activePlayers.Add(TextID, this);

                // TODO: check if the playerMenu already has this textID

                Outer.playerMenu.options.Add(new Dropdown.OptionData(TextID));
                Outer.playerMenu.RefreshShownValue();
            }

            public void TearDown()
            {
                FromAnyThread.Queue(() =>
                {
                    Destroy(Prefab);
                    Outer.playerMenu.options.Remove(Outer.playerMenu.options.Find(x => x.text == TextID));
                    Outer.playerMenu.RefreshShownValue();
                    return (object)null;
                });
                Outer._activePlayers.Remove(TextID);
            }

            public void Focus()
            {
                VideoController.BindUI();
                Outer.FocusedPlayerID = TextID;

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

    internal sealed class GameObjectActiveStateRelay : MonoBehaviour
    {
        public event Action? ActiveStateChanged;

        private void OnEnable() => ActiveStateChanged?.Invoke();

        private void OnDisable() => ActiveStateChanged?.Invoke();
    }
}