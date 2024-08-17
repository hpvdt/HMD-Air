namespace HMD.Scripts.Streaming.Capture
{
    using System;
    using System.Linq;
    using Unity.VisualScripting;
    using UnityEngine;

    public class CaptureDeviceFeed : FeedLike
    {
        public struct CameraSelector
        {
            public string Name;
            public Resolution? Res;
        }

        private WebCamTexture _webCamTex;
        private static WebCamDevice[] Devices
        {
            get
            {
                var result = WebCamTexture.devices;
                return result;
            }
        }

        private WebCamDevice? _device;

        public void OpenNextDevice(Resolution? res)
        {
            var devices = Devices;
            var index = Array.IndexOf(devices, _device);
            index = (index + 1) % devices.Length;
            _device = devices[index];

            OpenWithResolution(res);
        }

        public void OpenWithResolution(Resolution? res)
        {
            if (_device == null) return;

            var resList = _device?.availableResolutions;

            if (res == null)
            {
                Log.V($"Setting up camera `{_device.Value.name}`");
                Open(new CameraSelector
                {
                    Name = _device.Value.name
                });
            }
            else
            {
                var _res = res.Value;
                if (resList != null && !resList.Contains(_res))
                {
                    Warning.V(
                        $"resolution `{_res.ToString()}` may be unsupported:\n"
                        + $"supported resolutions are [{string.Join(", ", resList.Select(x => x.ToString()))}]"
                    );
                }

                Open(new CameraSelector
                {
                    Name = _device.Value.name,
                    Res = res
                });
            }
        }

        public void Open(CameraSelector selector)
        {
            Stop();
            // _cameraTexture?.IsDestroyed()

            if (selector.Res.HasValue)
            {
                var res = selector.Res.Value;
                var fps = selector.Res.Value.refreshRate;
                _webCamTex = new WebCamTexture(
                    selector.Name,
                    res.height,
                    res.width,
                    fps
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
                + $"    Seleccted: `{selector.Name}` ({selector.Res.ToSafeString()})\n"
                + $"    Actual: `{_webCamTex.deviceName}` ({_webCamTex.width}x{_webCamTex.height} @ {_webCamTex.requestedFPS}fps)"
            );

        }

        public override void Stop()
        {
            Log.V("Stop");
            _webCamTex?.Stop();
        }

        protected override TextureView? TryGetTextureIfValid(TextureView? existing)
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
            return ((uint)_webCamTex.width, (uint)_webCamTex.height);
        }


        public override void Dispose()
        {
            Stop();

            Log.V("Destroy Camera Feed");
            _webCamTex = null;
        }
    }
}
