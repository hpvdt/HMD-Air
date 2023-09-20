namespace HMD.Scripts.Streaming
{
    using System;
    using UnityEngine;
    using Util;

    public abstract class FeedLike : MonoBehaviourWithLogging, IDisposable
    {
        public abstract TextureView? TryGetTexture(TextureView? existing);

        public abstract void Stop();

        public abstract void Play();

        public abstract void Pause();

        public (uint, uint) Size { get; }

        public string NativeAspectRatioStr
        {
            get
            {
                return $"{Size.Item1}:{Size.Item2}";
            }
        }

        public virtual string GetAspectRatioStr() { return NativeAspectRatioStr; }

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
            base.Awake();
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

        public abstract void Dispose();
    }
}
