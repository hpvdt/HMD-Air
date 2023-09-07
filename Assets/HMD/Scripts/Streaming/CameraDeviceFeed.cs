namespace HMD.Scripts.Streaming
{
    using System;
    using UnityEngine;
    public class CameraDeviceFeed : MonoBehaviourWithLogging, IFeed
    {
        public struct CameraSelector
        {
            public string name;
            public Resolution res;
        }

        private WebCamTexture _deviceTex;

        WebCamDevice[] Devices
        {
            get
            {
                var result = WebCamTexture.devices;
                return result;
            }
        }

        public void Open(CameraSelector selector)
        {
            var fps = (int)Math.Round(selector.res.refreshRateRatio.value);
            _deviceTex = new WebCamTexture(
                selector.name,
                selector.res.height,
                selector.res.width,
                fps
            );
        }

        public void Stop()
        {
            _deviceTex.Stop();
        }

        public TextureView TryGetTexture(TextureView existing)
        {

            if (existing?.Source == _deviceTex) return existing;

            var result = new TextureView(_deviceTex);
            return result;
        }

        public void Play()
        {
            _deviceTex.Play();
        }

        public void Pause()
        {
            _deviceTex.Pause();
        }
    }
}
