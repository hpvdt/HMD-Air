using HMD.Scripts.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace HMD.Scripts
{
    public class IntentHandler : MonoBehaviourWithLogging
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
                Log.V("Application gained focus");
                OnIntent();
            }
            else
            {
                Log.V("Application lost focus");
            }
        }

        private void Update()
        {
            // TODO: can F1 be generalised?
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F1))
                EditorWindow.focusedWindow.maximized = !EditorWindow.focusedWindow.maximized;
#endif
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
                Log.V("On Intent" + intent.Call<string>("getAction"));

                var result = intent.Call<string>("getDataString");

                if (result != null)
                {
                    result = UnityWebRequest.UnEscapeURL(result);
                    Log.V("On Intent" + result);
                    // mainDisplay.VLC.Open(result);
                }

                var extras = intent.Call<AndroidJavaObject>("getExtras");
                if (extras != null)
                {
                    var data = extras.Call<string>("getString", "data");
                    Log.V("Data: " + data);
                }
            }
        }

        // Update is called once per frame
        // private void Update()
        // {
        // }
    }
}