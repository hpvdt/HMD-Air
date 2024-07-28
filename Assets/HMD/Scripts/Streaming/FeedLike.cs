namespace HMD.Scripts.Streaming
{
    using System;
    using UnityEngine;
    using Util;

    public abstract class FeedLike : MonoBehaviourWithLogging, IDisposable
    {
        public abstract (uint, uint) GetSize();

        public bool isValid()
        {
            var size = GetSize();
            if (size.Item1 == 0 || size.Item2 == 0) return false;

            return true;
        }

        protected abstract TextureView? TryGetTextureIfValid(TextureView? existing);

        public TextureView? TryGetTexture(TextureView? existing)
        {
            if (isValid())
            {
                var res = TryGetTextureIfValid(existing);

                if (existing != null && res != null && res != existing)
                {
                    LogWarning($"existing texture {existing} is obsolete\n"
                        + $"creating new one {res}\n");
                }

                return res;
            }

            return null;
        }

        public abstract void Stop();

        public abstract void Play();

        public abstract void Pause();

        // public static Frac DefaultAspectRatio = new Frac(16, 9);

        public Frac NativeAspectRatio()
        {
            var size = GetSize();
            return new Frac((int)size.Item1, (int)size.Item2);
        }

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

        // private string? isTextureConforming(TextureView tex, (uint, uint) size)
        // {
        //     if (tex == null) return $"initializing texture";
        //
        //     if ()
        //
        //         return tex.width == size.Item1 && tex.height == size.Item2;
        // }
    }
}
