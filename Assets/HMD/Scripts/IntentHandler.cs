using UnityEngine;
using UnityEngine.Networking;

public class IntentHandler : MonoBehaviour
{
    // public MainDisplay mainDisplay;

    // Start is called before the first frame update
    private void Start()
    {
        OnIntent();
    }

    // OnApplicationFocus
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            OnIntent();
        }
        else
        {
            Debug.Log("Application lost focus");
        }
    }

    // OnIntent
    private void OnIntent()
    {
        
        Debug.Log ("displays connected: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
        // Check if additional displays are available and activate each.
    
        // TODO: enable later
        // foreach (Display d in Display.displays)
        // {
        //     Debug.Log ("activating display");
        //     d.Activate();
        // }
        
        // if (Application.isEditor) return;
        
        if (Application.platform == RuntimePlatform.Android)
        {
            
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            var intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            Debug.Log("On Intent" + intent.Call<string>("getAction"));

            var result = intent.Call<string>("getDataString");

            if (result != null)
            {
                result = UnityWebRequest.UnEscapeURL(result);
                Debug.Log("On Intent" + result);
                // mainDisplay.VLC.Open(result);
            }

            var extras = intent.Call<AndroidJavaObject>("getExtras");
            if (extras != null)
            {
                var data = extras.Call<string>("getString", "data");
                Debug.Log("Data: " + data);
            }
        }
    }

    // Update is called once per frame
    // private void Update()
    // {
    // }
}
