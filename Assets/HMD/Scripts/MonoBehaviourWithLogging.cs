using System.Reflection;
using UnityEngine;


public class MonoBehaviourWithLogging : MonoBehaviour
{
    public bool logToConsole = true; //Log function calls and LibVLC logs to Unity console

    protected static void Log(object message)
    {

        var name = MethodBase.GetCurrentMethod().DeclaringType;
        // if (logToConsole)
        Debug.Log($"[{name}] {message}");
    }
}
