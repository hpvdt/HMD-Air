#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using HMD.Scripts.Streaming;
using HMD.Scripts.Util;
using MAVLinkAPI.Util;
using MAVLinkAPI.Util.NullSafety;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HMD.Scripts
{
    public class DisplayController : MonoBehaviourWithLogging
    {
        private List<Display>? _extendDisplay;

        // TODO: this should be bind to a new button in Subsystem
        private List<Display> ExtendDisplayOnce() // In Unity, display cannot be scrapped
        {
            if (_extendDisplay == null)
            {
                Debug.Log("displays connected: " + Display.displays.Length);
                // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
                // Check if additional displays are available and activate each.

                var result = Display.displays.Skip(1).ToList();

                foreach (var d in result)
                {
                    Debug.Log("display" + d.systemWidth + "x" + d.systemHeight + " : " + d.renderingWidth + "x"
                              + d.renderingHeight);
                    d.Activate();
                }

                _extendDisplay = result;
                return result;
            }

            return _extendDisplay!;
        }
    }
}