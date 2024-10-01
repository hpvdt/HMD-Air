// using System.IO;
// using UnityEngine;
//
// namespace MAVLinkAPI.Scripts.Process
// {
//     internal static class InternalServerProcess
//     {
//         private static uint ptr = 0;
//
//         // TODO: this is for Lavender
//         public static void Start()
//         {
//             var processPath = Directory.GetCurrentDirectory() + "/Lavender.exe";
//
//             Debug.Log($"ProcessPath:{processPath}");
//
//             if (File.Exists(processPath))
//             {
//                 Debug.Log($"File Exists, preping ini settings");
//                 // Settings.SetSetting("Server_StartMap", true);
//                 //
//                 // var settings = Settings.ExportSettings();
//
//                 var settings = "";
//
//                 var args = $" -batchmode -nographics -server -logFile \"server_log.txt\" -ini ~{settings}~";
//                 Debug.Log($"Args:{args}");
//                 if (ptr != 0)
//                 {
//                     Debug.Log("Internal server process already exists");
//                 }
//                 else
//                 {
//                     ptr = StartExternalProcess.Start(Directory.GetCurrentDirectory(), processPath + args);
//                     Debug.Log($"pid:{ptr}");
//                 }
//             }
//             else
//             {
//                 Debug.Log($"File doesnt exist");
//             }
//         }
//
//         public static void Stop()
//         {
//             Debug.Log($"killing if running pid:{ptr}");
//
//             if (ptr != 0)
//             {
//                 Debug.Log($"killing");
//                 StartExternalProcess.KillProcess(ptr);
//             }
//         }
//     }
// }

