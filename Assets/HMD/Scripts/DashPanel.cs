using System.Collections.Generic;
using System.Linq;
using HMD.Scripts.Streaming.VLC;
using NRKernal;
using UnityEngine;
using UnityEngine.UI;

public class DashPanel : MonoBehaviour
{
    /*bool _menu_visible = false;*/

    [HideInInspector]
    public VlcController controller;

    private GameObject _menuPanel;
    private GameObject _og_menu;

    private GameObject _app_menu;

    // GameObject _unlock_3d_sphere_mode_prompt_popup = null;
    private GameObject _menu_toggle_button;

    private GameObject _options_button;

    // private GameObject _custom_popup = null;
    private GameObject _lockScreenNotice;

    private GameObject _aspect_popup;
    private GameObject _display_popup;
    private GameObject _format_popup;
    private GameObject _whats_new_popup;
    private GameObject _picture_settings_popup;

    private bool _og_menu_visible = true;

    private MenuID _visible_menu_id;

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

        _lockScreenNotice = GameObject.Find("LockScreenNotice");

        // hide 6dof button if not supported
        if (NRDevice.Subsystem.GetDeviceType() != NRDeviceType.NrealLight)
            GameObject.Find("ChangeTo6Dof").SetActive(false);

        var versionName = Application.version;
        var versionCode = Application.buildGUID;
        GameObject.Find("AppMenu/AppMenuInner/Subtitle").GetComponent<Text>().text = $"{versionName} ({versionCode})";

        // center UI things that i had spread out in Editor
        CenterPopupLocations();

        // Center Menus/Objects
        CenterXY(_lockScreenNotice);
        CenterXY(_menuPanel);
        CenterXY(_app_menu);

        _lockScreenNotice.SetActive(false);

        HideAllMenus();
        HideAllPopups();

        ShowOGMenu();

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
        var popups = GameObject.Find("Canvas/Popups");
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

    public static GameObject[] FindGameObjectsAll(string name)
    {
        var Found = new List<GameObject>();
        var All = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var entry in All)
            if (entry.name == name)
                Found.Add(entry);
        return Found.ToArray();
    }

    public static GameObject FindGameObjectsAllFirst(string name)
    {
        return FindGameObjectsAll(name)?.First();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) UpdateReferences();
    }

    public void UpdateReferences()
    {
        _menu_toggle_button = FindGameObjectsAllFirst("MenuToggleButton");

        _menuPanel = FindGameObjectsAllFirst("MyControlPanel");
        _og_menu = FindGameObjectsAllFirst("Buttons");
        _app_menu = FindGameObjectsAllFirst("AppMenu");

        // _unlock_3d_sphere_mode_prompt_popup = FindGameObjectsAllFirst("Unlock3DSphereModePopup");

        _aspect_popup = FindGameObjectsAllFirst("AspectRatioPopup");
        _options_button = FindGameObjectsAllFirst("OptionsButton");
        _display_popup = FindGameObjectsAllFirst("DisplayPopup");
        _format_popup = FindGameObjectsAllFirst("FormatPopup");
        _whats_new_popup = FindGameObjectsAllFirst("WhatsNewPopup");
        _picture_settings_popup = FindGameObjectsAllFirst("PictureSettingsPopup");
    }

    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    private void ShowOGMenu()
    {
        _og_menu.SetActive(true);
        _og_menu_visible = true;

        _menu_toggle_button.SetActive(true);
    }

    private void HideOGMenu()
    {
        _og_menu.SetActive(false);
        _og_menu_visible = false;
    }

    private void ShowOptionMenu()
    {
        UpdateReferences();

        _app_menu.SetActive(true);
        CenterXY(_app_menu);

        _menu_toggle_button.SetActive(false);
    }

    private void HideAppMenu()
    {
        _app_menu.SetActive(false);
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
        _menuPanel?.SetActive(true);

        _options_button.SetActive(true);
        _menu_toggle_button.SetActive(true);
    }

    private void HideControllerMenu()
    {
        _menuPanel?.SetActive(false);
    }

    public void UIToggleControllerMenu()
    {
        if (_visible_menu_id == MenuID.CONTROLLER_MENU)
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
        _visible_menu_id = id;
        _menu_toggle_button.SetActive(false);
        _options_button.SetActive(false);
        switch (id)
        {
            case MenuID.OG_MENU:
                ShowOGMenu();
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
        HideOGMenu();
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
        _aspect_popup.SetActive(true);

        var updater = new AspectRatioUpdater(controller.mainDisplay);

        updater.SyncAll();
    }


    private void HideAspectRatioPopup()
    {
        _aspect_popup.SetActive(false);
    }

    public void ShowDisplayPopup()
    {
        _display_popup.SetActive(true);
    }

    private void HideDisplayPopup()
    {
        _display_popup.SetActive(false);
    }

    public void ShowFormatPopup()
    {
        _format_popup.SetActive(true);
    }

    private void HideFormatPopup()
    {
        _format_popup.SetActive(false);
    }

    public void ShowWhatsNewPopup()
    {
        _whats_new_popup.SetActive(true);
    }

    private void HideWhatsNewPopup()
    {
        _whats_new_popup.SetActive(false);
    }

    public void ShowPictureSettingsPopup()
    {
        _picture_settings_popup.SetActive(true);
    }

    private void HidePictureSettingsPopup()
    {
        _picture_settings_popup.SetActive(false);
    }
}
