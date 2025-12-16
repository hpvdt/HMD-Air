using System.Collections.Generic;
using System.Linq;
using MAVLinkAPI.Routing;
using UnityEngine;
using ArgsT = HMD.Scripts.Streaming.VCap.VCapFeed.ArgsT;

namespace HMD.Scripts.Streaming.VCap
{
    public class VCapNewFeed : MonoBehaviour
    {
        private static readonly Yaml Pickler = new();

        public void LogAllDevices()
        {
            var devices = WebCamTexture.devices;

            var selectors = new List<ArgsT>();

            foreach (var pair in devices.Select((v, i) => (v, i)))
            {
                var resList = pair.v.availableResolutions;

                {
                    var v = new ArgsT
                    {
                        Index = pair.i,
                        Name = pair.v.name
                    };
                    selectors.Add(v);
                }

                if (resList != null)
                    foreach (var res in resList)
                    {
                        var v = new ArgsT
                        {
                            Index = pair.i,
                            Name = pair.v.name,
                            Resolution = res
                        };
                        selectors.Add(v);
                    }
            }

            var yamls = Pickler.Fwd(selectors);
            Debug.Log($"Found {devices.Length} capture devices >>>>>\n" + yamls + "\n<<<<<");
        }
    }
}