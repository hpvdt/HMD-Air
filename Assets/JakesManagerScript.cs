using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JakesManagerScript : MonoBehaviour
{
    private bool tracking_enabled = true;

    // Start is called before the first frame update
    private void Start()
    {
    }

    private void OnToggleTrackingClicked()
    {
        // log
        Debug.Log("OnToggleClicked");

        // toggle tracking
        tracking_enabled = !tracking_enabled;

        // get the Scene Picker Popup
        var statusText = GameObject.Find("Scene Picker Popup/TrackingStatusText");
        var tc = statusText?.GetComponent<Text>();
        var disabled_or_enabled = tracking_enabled ? "Enabled" : "Disabled";
        if (tc) tc.text = "Tracking: " + disabled_or_enabled;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}