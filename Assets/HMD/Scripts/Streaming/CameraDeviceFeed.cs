namespace HMD.Scripts.Streaming
{
    using System;
    using System.Linq;
    using Unity.VisualScripting;
    using UnityEngine;
    public class CameraDeviceFeed : FeedLike, IFeed
    {
        public struct CameraSelector
        {
            public string Name;
            public Resolution? Res;
        }

        private WebCamTexture _sourceTexture;
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

            OpenResolution(res);
        }

        public void OpenResolution(Resolution? res)
        {
            if (_device == null) return;

            var resList = _device?.availableResolutions;

            if (res == null)
            {
                Log($"Setting up camera `{_device.Value.name}`");
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
                    LogWarning(
                        $"resolution `{_res.ToString()}` may be unsupported:\n"
                        + $"supported resolutions are [{string.Join(", ", resList.Select(x => x.ToString()))}]"
                    );
                }


                Log($"Setting up camera `{_device.Value.name}` ({_res.ToSafeString()})");
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
                var fps = (int)Math.Round(selector.Res.Value.refreshRateRatio.value);
                _sourceTexture = new WebCamTexture(
                    selector.Name,
                    res.height,
                    res.width,
                    fps
                );
            }
            else
            {
                _sourceTexture = new WebCamTexture(
                    selector.Name
                );
            }

        }

        public void Stop()
        {
            Log("Stop");
            _sourceTexture?.Stop();
        }

        public TextureView? TryGetTexture(TextureView? existing)
        {

            var tex = existing;
            if (_sourceTexture == null || existing?.Source != _sourceTexture)
            {
                existing?.Dispose();
                tex = new TextureView(_sourceTexture);
            }

            Graphics.Blit(tex.Source, tex.Cache, Transform, Offset);

            return tex;
        }

        public void Play()
        {
            Log("Play");
            _sourceTexture?.Play();
        }

        public void Pause()
        {
            _sourceTexture?.Pause();
        }

        public void Dispose()
        {
            Stop();

            Log("Destroy Camera Feed");
            _sourceTexture = null;
        }
    }
}