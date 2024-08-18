#nullable enable
namespace HMD.Scripts.Streaming.VCap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NullableExtension;
    using Pickle;
    using Unity.VisualScripting;
    using UnityEngine;

    public class VCapFeed : FeedLike
    {

        private Yaml _pickler = new Yaml();

        public struct DeviceSelector
        {
            public string Name;
            public Resolution? Resolution;
        }

        private WebCamTexture? _webCamTex;
        private static WebCamDevice[] Devices
        {
            get
            {
                var result = WebCamTexture.devices;
                return result;
            }
        }

        private WebCamDevice? _activeDevice;


        public void PlayNext(Resolution? res)
        {
            var devices = Devices;
            var index = Array.IndexOf(devices, _activeDevice);
            index = (index + 1) % devices.Length;
            _activeDevice = devices[index];

            PlayCurrent(res);
        }

        public void PlayCurrent(Resolution? res)
        {
            foreach (var d in _activeDevice.Wrap())
            {

                var resList = _activeDevice?.availableResolutions;

                if (res == null)
                {
                    Log.V($"Setting up camera `{d.name}`");
                    Open(
                        new DeviceSelector { Name = d.name }
                    );
                }
                else
                {
                    var resV = res.Value;
                    if (resList != null && !resList.Contains(resV))
                    {
                        Warning.V(
                            $"resolution `{resV.ToString()}` may be unsupported:\n"
                            + $"supported resolutions are [{string.Join(", ", resList.Select(x => x.ToString()))}]"
                        );
                    }

                    Open(
                        new DeviceSelector
                        {
                            Name = d.name,
                            Resolution = res
                        }
                    );
                }
            }
        }

        public void LogAllDevices()
        {
            var devices = Devices;

            var selectors = new List<DeviceSelector>();

            foreach (var dd in devices)
            {
                var resList = dd.availableResolutions;

                {
                    var v = new DeviceSelector()
                    {
                        Name = dd.name
                    };
                    selectors.Add(v);
                }

                if (resList != null)
                {
                    foreach (var res in resList)
                    {
                        var v = new DeviceSelector()
                        {
                            Name = dd.name,
                            Resolution = res
                        };
                        selectors.Add(v);
                    }
                }
            }

            var yamls = _pickler.Fwd(selectors);
            Debug.Log($"Found {devices.Length} capture devices >>>>>\n" + yamls + "\n<<<<<");
        }

        public void Open(string selectorStr)
        {

            var selector = _pickler.Rev<DeviceSelector>(selectorStr);

            Open(selector);
        }

        public void Open(DeviceSelector selector)
        {
            Stop();

            var found = Devices.FirstOrDefault(x => x.name.Contains(selector.Name));

            if (selector.Resolution.HasValue)
            {
                var res = selector.Resolution.Value;
                var fps = selector.Resolution.Value.refreshRateRatio.value;
                _webCamTex = new WebCamTexture(
                    found.name,
                    res.height,
                    res.width,
                    (int)fps
                );

                // _sourceTexture.requestedWidth = res.width;
                // _sourceTexture.requestedHeight = res.height;
                // _sourceTexture.requestedFPS = fps;
            }
            else
            {
                _webCamTex = new WebCamTexture(
                    selector.Name
                );
            }

            Play();
            _webCamTex.GetPixels(); // otherwise width and height will always be 16x16

            Log.V(
                $"Setting up camera:\n"
                + $"    Seleccted: `{selector.Name}` ({selector.Resolution.ToSafeString()})\n"
                + $"    Actual: `{_webCamTex.deviceName}` ({_webCamTex.width}x{_webCamTex.height} @ {_webCamTex.requestedFPS}fps)"
            );

        }

        public override void Stop()
        {
            Log.V("Stop");
            _webCamTex?.Stop();
        }

        protected override TextureView TryGetTextureIfValid(TextureView? existing)
        {
            var tex = existing;
            var srcSize = GetSize();

            if (tex == null || tex.Source != _webCamTex || tex.Size.Value != srcSize)
            {
                var newTex = new TextureView(_webCamTex);

                tex?.Dispose();
                tex = newTex;
            }

            Graphics.Blit(tex.Source, tex.Cache, Transform, Offset);

            return tex;
        }

        public override void Play()
        {
            Log.V("Play");
            _webCamTex?.Play();
        }

        public override void Pause()
        {
            _webCamTex?.Pause();
        }

        protected override (uint, uint) GetSize()
        {
            if (_webCamTex == null)
            {
                return (0, 0);
            }
            else
            {
                return ((uint)_webCamTex.width, (uint)_webCamTex.height);
            }
        }

        public override void Dispose()
        {
            Stop();

            Log.V("Destroy Camera Feed");
            _webCamTex = null;
        }
    }
}
