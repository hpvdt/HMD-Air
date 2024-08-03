using HMD.Scripts.Streaming.VLC;
using HMD.Scripts.Util;
using NRKernal;
using UnityEngine;
using UnityEngine.UI;

public class DashPanel : MonoBehaviour
{
    /*bool _menu_visible = false;*/

    [HideInInspector]
    public VlcController controller;

    public Button playerButton;
    public GameObject playerGroup;

    private GameObject _baseButtons;
    
    private GameObject _rootPanel;

    private GameObject _appMenu;

    // GameObject _unlock_3d_sphere_mode_prompt_popup = null;
    private GameObject _menuToggleButton;

    private GameObject _optionsButton;

    // private GameObject _custom_popup = null;
    private GameObject _lockScreenNotice;

    private GameObject _aspectRatioPopup;
    private GameObject _displayPopup;
    private GameObject _formatPopup;
    private GameObject _whatsNewPopup;
    private GameObject _pictureSettingsPopup;

    private bool _baseButtonsVisible = true;

    private MenuID _visibleMenuID;
    
    // public UIStateBeforeCustomPopup stateBeforePopup;

    [SerializeField]
    public enum MenuID
    {
        OG_MENU,
        CONTROLLER_MENU,
        OPTION_MENU
    };

    // [SerializeField]
    // public enum PopupID
    // {
    //     CUSTOM_AR_POPUP,
    //
    //     // MODE_LOCKED,
    //     CUSTOM_POPUP,
    //     PICTURE_SETTINGS_POPUP,
    //     FILE_FORMAT_POPUP,
    //     DISPLAY_SETTINGS_POPUP,
    //     COLOR_POPUP,
    //     WHATS_NEW_POPUP
    // }

    // private PopupID[] popupStack;

    // public class UIStateBeforeCustomPopup
    // {
    //     public UIStateBeforeCustomPopup(MenuID _visible_menu_id)
    //     {
    //         VisibleMenuID = _visible_menu_id;
    //     }
    //
    //     public MenuID VisibleMenuID;
    // }

    // public void SetVLC(MainDisplay instance)
    // {
    //     mainDisplay = instance;
    // }

    // Start is called before the first frame update
    private void Start()
    {
        UpdateReferences();

        _lockScreenNotice = GlobalFinder.Find("LockScreenNotice").Only();

        // hide 6dof button if not supported
        if (NRDevice.Subsystem.GetDeviceType() != NRDeviceType.XrealLight)
            GlobalFinder.Find("ChangeTo6Dof").Only().SetActive(false);

        var versionName = Application.version;
        var versionCode = Application.buildGUID;
        GlobalFinder.Find("AppMenu/AppMenuInner/Subtitle").Only().GetComponent<Text>().text = $"{versionName} ({versionCode})";

        // center UI things that i had spread out in Editor
        CenterPopupLocations();

        // Center Menus/Objects
        CenterXY(_lockScreenNotice);
        CenterXY(_rootPanel);
        CenterXY(_appMenu);

        _lockScreenNotice.SetActive(false);

        HideAllMenus();
        HideAllPopups();

        ShowOgMenu();

        if (PlayerPrefs.GetInt("OnboardingSeen_0_0_5_g") == 1)
        {
            // The user has already seen the onboarding tutorial text
        }
        else
        {
            // The user has not yet seen the onboarding tutorial text
            PlayerPrefs.SetInt("OnboardingSeen_0_0_5_g", 1);
            ShowWhatsNewPopup();
        }
    }

    private void CenterPopupLocations()
    {
        // Get the "Popups" game object, then loop over each of it's top-level children
        // and center them on the screen
        
        var popups = GlobalFinder.Find("Canvas/Popups").Only();
        for (var i = 0; i < popups.transform.childCount; i++)
        {
            var childGameObject = popups.transform.GetChild(i).gameObject;
            //Debug.Log("centering " + childGameObject.name);
            CenterXY(childGameObject);
        }
    }

    private void CenterXY(GameObject o)
    {
        o.transform.localPosition = new Vector3(
            0.0f,
            0.0f,
            o.transform.localPosition.z
        );
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) UpdateReferences();
    }

    public void UpdateReferences()
    {
        _menuToggleButton = gameObject.ByName("MenuToggleButton").Only();

        var basePanel = gameObject.ByName("BaseControllerPanel").Only();
        _baseButtons = basePanel.ByName("Buttons").Only();

        _rootPanel = gameObject.ByName("RootPanel").Only();
        _appMenu = gameObject.ByName("AppMenu").Only();

        // _unlock_3d_sphere_mode_prompt_popup = FindGameObjectsAllFirst("Unlock3DSphereModePopup");

        _aspectRatioPopup = gameObject.ByName("AspectRatioPopup").Only();
        _optionsButton = gameObject.ByName("OptionsButton").Only();
        _displayPopup = gameObject.ByName("DisplayPopup").Only();
        _formatPopup = gameObject.ByName("FormatPopup").Only();
        _whatsNewPopup = gameObject.ByName("WhatsNewPopup").Only();
        _pictureSettingsPopup = gameObject.ByName("PictureSettingsPopup").Only();
    }

    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    private void ShowOgMenu()
    {
        _baseButtons.SetActive(true);
        _baseButtonsVisible = true;

        _menuToggleButton.SetActive(true);
    }

    private void HideOgMenu()
    {
        _baseButtons.SetActive(false);
        _baseButtonsVisible = false;
    }

    private void ShowOptionMenu()
    {
        UpdateReferences();

        _appMenu.SetActive(true);
        CenterXY(_appMenu);

        _menuToggleButton.SetActive(false);
    }

    private void HideAppMenu()
    {
        _appMenu.SetActive(false);
    }

    // public void ShowUnlock3DSphereModePropmptPopup()
    // {
    //     _unlock_3d_sphere_mode_prompt_popup.SetActive(true);
    //
    //     stateBeforePopup = new UIStateBeforeCustomPopup(_visible_menu_id);
    //
    //     HideAllMenus();
    // }

    // public void RestoreStateBeforePopup()
    // {
    //     if (stateBeforePopup == null) return;
    //     ShowMenuByID(stateBeforePopup.VisibleMenuID);
    //     stateBeforePopup = null;
    // }

    // public void HideUnlock3DSphereModePropmptPopup()
    // {
    //     _unlock_3d_sphere_mode_prompt_popup.SetActive(false);
    //
    //     RestoreStateBeforePopup();
    // }

    /*public bool GetTrackpadVisible()
    {
        return OGMenuVisible();
    }*/

    /*public bool MenuIsHidden()
    {
        return !_menu_visible;
    }*/

    // public bool OGMenuVisible()
    // {
    //     return _og_menu_visible;
    // }

    private void ShowControllerMenu()
    {
        _rootPanel?.SetActive(true);

        _optionsButton.SetActive(true);
        _menuToggleButton.SetActive(true);
    }

    private void HideControllerMenu()
    {
        _rootPanel?.SetActive(false);
    }

    public void UIToggleControllerMenu()
    {
        if (_visibleMenuID == MenuID.CONTROLLER_MENU)
            ShowMenuByID(MenuID.OG_MENU);
        else
            ShowMenuByID(MenuID.CONTROLLER_MENU);
    }

    public void UIShowControllerMenu()
    {
        ShowMenuByID(MenuID.CONTROLLER_MENU);
    }

    public void UIShowOptionMenu()
    {
        ShowMenuByID(MenuID.OPTION_MENU);
    }

    public void ShowMenuByID(MenuID id)
    {
        HideAllMenus();
        _visibleMenuID = id;
        _menuToggleButton.SetActive(false);
        _optionsButton.SetActive(false);
        switch (id)
        {
            case MenuID.OG_MENU:
                ShowOgMenu();
                break;
            case MenuID.CONTROLLER_MENU:
                ShowControllerMenu();
                break;
            case MenuID.OPTION_MENU:
                ShowOptionMenu();
                break;
        }
    }

    private void HideAllMenus()
    {
        HideOgMenu();
        HideControllerMenu();
        HideAppMenu();
    }

    public void HideAllPopups()
    {
        // HideUnlock3DSphereModePropmptPopup();
        // HideCustomPopup();
        HideAspectRatioPopup();
        HideDisplayPopup();
        HideFormatPopup();
        HideWhatsNewPopup();
        HidePictureSettingsPopup();
        // HidePopupByID(PopupID.PICTURE_SETTINGS_POPUP);
    }

    // private void HidePopupByID(PopupID popupID)
    // {
    //     switch (popupID)
    //     {
    //         // case PopupID.MODE_LOCKED:
    //         //     HideUnlock3DSphereModePropmptPopup();
    //         //     break;
    //         /*case PopupID.CUSTOM:
    //             HideCustomPopup();
    //             break;*/
    //         case PopupID.CUSTOM_AR_POPUP:
    //             HideAspectRatioPopup();
    //             break;
    //         case PopupID.PICTURE_SETTINGS_POPUP:
    //             HidePictureSettingsPopup();
    //             break;
    //     }
    //
    //     RestoreStateBeforePopup();
    // }

    // TODO: aggregate into a view
    public void ShowAspectRatioPopup()
    {
        _aspectRatioPopup.SetActive(true);

        var updater = new AspectRatioUpdater(controller.display);

        updater.SyncAll();
    }


    private void HideAspectRatioPopup()
    {
        _aspectRatioPopup.SetActive(false);
    }

    public void ShowDisplayPopup()
    {
        _displayPopup.SetActive(true);
    }

    private void HideDisplayPopup()
    {
        _displayPopup.SetActive(false);
    }

    public void ShowFormatPopup()
    {
        _formatPopup.SetActive(true);
    }

    private void HideFormatPopup()
    {
        _formatPopup.SetActive(false);
    }

    public void ShowWhatsNewPopup()
    {
        _whatsNewPopup.SetActive(true);
    }

    private void HideWhatsNewPopup()
    {
        _whatsNewPopup.SetActive(false);
    }

    public void ShowPictureSettingsPopup()
    {
        _pictureSettingsPopup.SetActive(true);
    }

    private void HidePictureSettingsPopup()
    {
        _pictureSettingsPopup.SetActive(false);
    }
    
    
    public class Lock
    {
        private bool _screenLocked = false;
        private float _brightnessOnLock;
    
        protected GameObject HideWhenLocked;

        protected GameObject LockScreenNotice;
        protected GameObject MenuToggleButton;
        //
        protected GameObject Logo;
        
        // TODO: set the following in editor
        // _hideWhenLocked = GameObject.Find("HideWhenScreenLocked");
        // _lockScreenNotice = GameObject.Find("LockScreenNotice");
        // _logo = GameObject.Find("logo");
        // _menuToggleButton = GameObject.Find("MenuToggleButton");
        
        public void ToggleScreenLock()
        {
            _screenLocked = !_screenLocked;

            if (_screenLocked)
            {
                // Hide All UI except for the lock button
                HideWhenLocked.SetActive(false);
                LockScreenNotice.SetActive(true);
                Logo.SetActive(false);
                MenuToggleButton.SetActive(false);
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
                HideWhenLocked.SetActive(true);
                LockScreenNotice.SetActive(false);
                Logo.SetActive(true);
                MenuToggleButton.SetActive(true);
            }
        }
    }

}
