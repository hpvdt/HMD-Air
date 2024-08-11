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
