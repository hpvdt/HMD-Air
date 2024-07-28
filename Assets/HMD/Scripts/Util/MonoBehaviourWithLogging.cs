using UnityEngine;

namespace HMD.Scripts.Util
{
    using System;
    public class MonoBehaviourWithLogging : MonoBehaviour
    {
        public bool logToConsole = true; //Log function calls and LibVLC logs to Unity console

        public string LoggerPrefix;

        // private string LoggerPrefix
        // {
        //     get
        //     {
        //         return name;
        //     }
        // }

        protected void Awake()
        {
            LoggerPrefix = name;
        }

        protected void Log(object message, LogType? type = null)
        {
            var actualType = type.GetValueOrDefault(LogType.Log);
            //
            // Debug.unityLogger.Log();
            // var name = MethodBase.GetCurrentMethod().DeclaringType;
            // if (logToConsole)
            try
            {
                Debug.unityLogger.Log(actualType, $"[{LoggerPrefix}]", $"{message}", this);
                Debug.Assert(LoggerPrefix != null, "Not Awaken! LoggerPrefix != null");
            }
            catch (Exception ee)
            {
                Debug.LogError($"[ERROR LOGGING] {message}" + ee.Message, this);
            }
        }

        protected void LogWarning(object message)
        {
            Log(message, LogType.Warning);
        }

        protected void LogError(object message)
        {
            Log(message, LogType.Error);
        }
    }
}
