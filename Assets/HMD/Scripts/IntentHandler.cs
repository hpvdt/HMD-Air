using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
public class IntentHandler : MonoBehaviour
{
    [FormerlySerializedAs("jakesSBSVLC")] public VLCMainDisplay vlcMainDisplay;

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
            //Debug.Log("Application lost focus");
        }
    }

    // OnIntent
    private void OnIntent()
    {
        if (Application.isEditor) return;

        var UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        var intent = currentActivity.Call<AndroidJavaObject>("getIntent");
        Debug.Log("On Intent" + intent.Call<string>("getAction"));

        var result = intent.Call<string>("getDataString");

        if (result != null)
        {
            result = UnityWebRequest.UnEscapeURL(result);
            Debug.Log("On Intent" + result);
            vlcMainDisplay.Open(result);
        }

        var extras = intent.Call<AndroidJavaObject>("getExtras");
        if (extras != null)
        {
            var data = extras.Call<string>("getString", "data");
            Debug.Log("Data: " + data);
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
