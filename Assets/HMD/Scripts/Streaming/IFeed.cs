namespace HMD.Scripts.Streaming
{
    using System;
    using UnityEngine;
    using Util;
    public interface IFeed : IDisposable
    {
        public TextureView? TryGetTexture(TextureView? existing);

        public void Stop();

        public void Play();

        public void Pause();
    }

    public abstract class FeedLike : MonoBehaviourWithLogging
    {
        public bool flipTextureX; //No particular reason you'd need this but it is sometimes useful
        public bool flipTextureY; //Set to false on Android, to true on Windows

        public bool automaticallyFlipOnAndroid = true; //Automatically invert Y on Android

        protected Vector2 Transform
        {
            get
            {
                return new Vector2(flipTextureX ? -1 : 1, flipTextureY ? -1 : 1);
            }
        }

        protected Vector2 Offset
        {
            get;
        } = Vector2.one;

        protected virtual void Awake()
        {
            //Automatically flip on android
            if (automaticallyFlipOnAndroid && Application.platform == RuntimePlatform.Android)
            {
                flipTextureX = !flipTextureX;
                flipTextureY = !flipTextureY;
            }

// #if UNITY_ANDROID
//         if (!Application.isEditor)
//         {
//             GetContext();
//
//
//
//             _brightnessHelper = new AndroidJavaClass("com.jakedowns.BrightnessHelper");
//             if (_brightnessHelper == null)
//             {
//                 Debug.Log("error loading _brightnessHelper");
//             }
//         }
// #endif
        }
    }
}
