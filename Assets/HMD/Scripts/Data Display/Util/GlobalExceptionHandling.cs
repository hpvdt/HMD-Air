using UnityEngine;

namespace HMD.Scripts.Util
{
    public class GlobalExceptionHandling : MonoBehaviourWithLogging
    {
        protected void Awake()
        {
            base.Awake();
            Application.logMessageReceived += HandleException;
            DontDestroyOnLoad(gameObject);
        }

        private void HandleException(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                // do handling
                // LogError(logString + "\n" + stackTrace);
            }
        }
    }
}
