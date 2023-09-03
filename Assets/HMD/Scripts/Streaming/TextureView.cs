using UnityEngine;

namespace HMD.Scripts.Streaming
{
    using System;
    public class TextureView // immutable, can only be initialised once, all derivative textures can only be destroyed together
    {
        private Texture2D _source; //This is the texture libVLC writes to directly.
        private RenderTexture _cache; //We copy it into this texture which we actually use in unity.


        public Lazy<(int, int)> Size;

        public Lazy<float> AspectRatio;

        public TextureView(Texture2D source)
        {

            _source = source;
            _cache = new RenderTexture(_source.width, _source.height, 0, RenderTextureFormat.ARGB32);
            //Make a renderTexture the same size as _source.

            _cache.Create();

            Size = new Lazy<(int, int)>((_source.width, _source.height));
            AspectRatio = new Lazy<float>((float)_source.width / _source.height);
        }

        public void Destroy()
        {
            _cache.Release();
        }


        public Texture2D Source
        {
            get { return _source; }
        }

        public RenderTexture Texture
        {
            get { return _cache; }
        }
    }
}
