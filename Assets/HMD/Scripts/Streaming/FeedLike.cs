using System;
using HMD.Scripts.Util;
using UnityEngine;

namespace HMD.Scripts.Streaming
{
    public abstract class FeedLike : MonoBehaviourWithLogging, IDisposable
    {
        protected abstract (uint, uint) GetSize();

        public (uint, uint)? GetSizeNonZero()
        {
            var size = GetSize();
            if (size.Item1 == 0 || size.Item2 == 0) return null;
            return size;
        }

        protected abstract TextureView? TryGetTextureIfValid(TextureView? existing);

        public TextureView? TryGetTexture(TextureView? existing)
        {
            if (GetSizeNonZero() != null)
            {
                var res = TryGetTextureIfValid(existing);

                if (existing != null && res != null && res != existing)
                    Warning.V($"existing texture {existing} is obsolete\n"
                              + $"creating new one {res}\n");

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
            var s = GetSizeNonZero();
            var _s = s.GetValueOrDefault((1, 1));
            return new Frac((int)_s.Item1, (int)_s.Item2);
        }

        public bool flipTextureX; //No particular reason you'd need this but it is sometimes useful
        public bool flipTextureY; //Set to false on Android, to true on Windows
        public bool automaticallyFlipOnAndroid = true; //Automatically invert Y on Android
        protected Vector2 Transform => new(flipTextureX ? -1 : 1, flipTextureY ? -1 : 1);

        protected Vector2 Offset { get; } = Vector2.one;

        protected new virtual void Awake()
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