using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;


public class MonoBehaviourWithLogging : MonoBehaviour
{
    public bool logToConsole = true; //Log function calls and LibVLC logs to Unity console

    private string LoggerName
    {
        get
        {
            return this.name;
        }
    }

    protected void Log(object message)
    {
        // var name = MethodBase.GetCurrentMethod().DeclaringType;
        // if (logToConsole)
        Debug.Log($"[{LoggerName}] {message}");
    }


    protected void LogError(object message)
    {
        // var name = MethodBase.GetCurrentMethod().DeclaringType;
        // if (logToConsole)
        Debug.LogError($"[{LoggerName}] {message}");
    }
}
