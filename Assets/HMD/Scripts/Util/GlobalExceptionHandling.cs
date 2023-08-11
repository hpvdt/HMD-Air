using UnityEngine;

namespace HMD.Scripts.Util
{
    public class GlobalExceptionHandling : MonoBehaviourWithLogging
    {
        void Awake()
        {
            Application.logMessageReceived += HandleException;
            DontDestroyOnLoad(gameObject);
        }

        void HandleException(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                // do handling
                // LogError(logString + "\n" + stackTrace);
            }
        }
    }
}
