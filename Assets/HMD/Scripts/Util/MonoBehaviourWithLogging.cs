using System;
using MAVLinkAPI.Util;
using UnityEngine;

namespace HMD.Scripts.Util
{
    public class MonoBehaviourWithLogging : MonoBehaviour
    {
        public bool loggerVerbosity = true; // TODO: should be a number
        public string loggerPrefix;

        protected Logger Log => _log();

        protected Logger Warning => _log(LogType.Warning);


        protected Logger Error => _log(LogType.Error);

        protected void Awake()
        {
            if (loggerPrefix == "") loggerPrefix = name;
        }

        private Logger _log(LogType? type = null)
        {
            return new Logger
            {
                Outer = this,
                Type = type
            };
        }

        protected class Logger : HasOuter<MonoBehaviourWithLogging>
        {
            public LogType? Type;

            private LogType ActualType => Type.GetValueOrDefault(LogType.Log);

            public void Write(string message)
            {
                //
                // Debug.unityLogger.Log();
                // var name = MethodBase.GetCurrentMethod().DeclaringType;
                // if (logToConsole)

                try
                {
                    Debug.unityLogger.Log(ActualType, $"[{Outer.loggerPrefix}]", $"{message}", Outer);
                    Debug.Assert(Outer.loggerPrefix != null, "Not Awaken! LoggerPrefix != null");
                }
                catch (Exception ee)
                {
                    Debug.LogError($"[ERROR LOGGING] {message}" + ee.Message, Outer);
                }
            }

            public void V(string message)
            {
                if (Outer.loggerVerbosity) Write(message);
            }
        }
    }
}