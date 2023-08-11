namespace HMD.Scripts.Streaming
{
    using System;
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

        private WebCamDevice? _selected1;
        private Resolution? _selected2;

        public void OpenNextDevice()
        {
            var devices = Devices;
            var index = Array.IndexOf(devices, _selected1);
            index = (index + 1) % devices.Length;
            _selected1 = devices[index];

            OpenNextResolution();
        }

        public void OpenNextResolution()
        {
            if (_selected1 == null) return;

            var resList = _selected1?.availableResolutions;
            if (resList == null || resList.Length <= 1)
            {
                Log($"Setting up camera `{_selected1.Value.name}`");
                Open(new CameraSelector
                {
                    Name = _selected1.Value.name
                });
            }
            else
            {
                var index = Array.IndexOf(resList, _selected2);
                index = (index + 1) % resList.Length;
                _selected2 = resList[index];

                Log($"Setting up camera `{_selected1.Value.name}` ({_selected2.Value.ToSafeString()})");
                Open(new CameraSelector
                {
                    Name = _selected1.Value.name,
                    Res = _selected2
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
