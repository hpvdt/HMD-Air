using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibVLCSharp;
using UnityEngine;
using UnityEngine.Assertions;
using Application = UnityEngine.Device.Application;

namespace HMD.Scripts.Streaming
{
    using JetBrains.Annotations;
    using UnityEngine.UI;
    public class VLCFeed : MonoBehaviourWithLogging
    {
        public VLCArgs Args = new VLCArgs(new List<string> { "https://jakedowns.com/media/sbs2.mp4" }, FromType.FromPath);

        private LibVLC _libVLC;

        //Create a new static LibVLC instance and dispose of the old one. You should only ever have one LibVLC instance.
        private void RefreshLibVLC()
        {
            Log("[MainDisplay] CreateLibVLC");
            //Dispose of the old libVLC if necessary
            if (_libVLC != null)
            {
                _libVLC.Dispose();
                _libVLC = null;
            }

            Core.Initialize(Application.dataPath); //Load VLC dlls
            _libVLC = new LibVLC(
                true); //You can customize LibVLC with advanced CLI options here https://wiki.videolan.org/VLC_command-line_help/

            //Setup Error Logging
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            _libVLC.Log += (s, e) =>
            {
                //Always use try/catch in LibVLC events.
                //LibVLC can freeze Unity if an exception goes unhandled inside an event handler.
                try
                {
                    if (logToConsole) Log(e.FormattedLog);
                }
                catch (Exception ex)
                {
                    Log("Exception caught in libVLC.Log: \n" + ex);
                }
            };
        }

        private LibVLC libVLC
        {
            get
            {
                if (_libVLC == null) RefreshLibVLC();
                return _libVLC;
            }
            set => _libVLC = value;
        }

        private MediaPlayer _mediaPlayer;

        //Create a new MediaPlayer object and dispose of the old one. 
        private void RefreshMediaPlayer()
        {
            Log("[MainDisplay] CreateMediaPlayer");
            if (_mediaPlayer != null) DestroyMediaPlayer();
            _mediaPlayer = new MediaPlayer(libVLC);
            Log("Media Player SET!");
        }

        //Dispose of the MediaPlayer object. 
        public void DestroyMediaPlayer()
        {
            // if (m_lRenderer?.material is not null)
            //     m_lRenderer.material.mainTexture = null;
            //
            // if (m_rRenderer?.material is not null)
            //     m_rRenderer.material.mainTexture = null;
            //
            // if (m_l360Renderer is not null && m_l360Renderer?.material is not null)
            //     m_l360Renderer.material.mainTexture = null;
            //
            // if (m_r360Renderer is not null && m_r360Renderer?.material is not null)
            //     m_r360Renderer.material.mainTexture = null;

            Stop();

            Log("DestroyMediaPlayer");
            mediaPlayer?.Stop();
            mediaPlayer?.Dispose();
            mediaPlayer = null;

            libVLC?.Dispose();
            libVLC = null;

        }

        public MediaPlayer mediaPlayer
        {
            get
            {
                if (_mediaPlayer == null) RefreshMediaPlayer();
                return _mediaPlayer;
            }
            set => _mediaPlayer = value;
        }

        public bool flipTextureX; //No particular reason you'd need this but it is sometimes useful
        public bool flipTextureY; //Set to false on Android, to true on Windows

        public bool automaticallyFlipOnAndroid = true; //Automatically invert Y on Android

        #region unity
        private void Awake()
        {
            //Setup Media Player
            RefreshMediaPlayer();

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

            Debug.Log($"[VLC] LibVLC version and architecture {libVLC.Changeset}");
            Debug.Log($"[VLC] LibVLCSharp version {typeof(LibVLC).Assembly.GetName().Version}");

            //Automatically flip on android
            if (automaticallyFlipOnAndroid && UnityEngine.Application.platform == RuntimePlatform.Android)
            {
                flipTextureX = !flipTextureX;
                flipTextureY = !flipTextureY;
            }
        }
        #endregion

        [CanBeNull]
        public TextureView TryGetTexture([CanBeNull] TextureView existing)
        {
            //Get size every frame
            uint height = 0;
            uint width = 0;
            mediaPlayer?.Size(0, ref width, ref height);

            //Automatically resize output textures if size changes
            var tex = existing;
            if (tex == null || tex.Size.Value != (width, height))
            {
                tex = TryGenerateTexture(width, height); // may fail if video is not ready
            }

            if (tex != null)
            {
                //Update the vlc texture (tex)
                var texPtr = mediaPlayer.GetTexture(width, height, out var updated);
                if (updated)
                {
                    var src = (Texture2D)tex.Source;
                    src.UpdateExternalTexture(texPtr);

                    //Copy the vlc texture into the output texture, flipped over
                    var flip = new Vector2(flipTextureX ? -1 : 1, flipTextureY ? -1 : 1);
                    Graphics.Blit(src, tex.Cache, flip, Vector2.zero);
                    //If you wanted to do post processing outside of VLC you could use a shader here.
                }
            }

            return tex;
        }

        //Resize the output textures to the size of the video
        [CanBeNull]
        private TextureView TryGenerateTexture(uint px, uint py)
        {
            var texptr = mediaPlayer.GetTexture(px, py, out var updated);
            if (px != 0 && py != 0 && updated && texptr != IntPtr.Zero)
            {
                //If the currently playing video uses the Bottom Right orientation, we have to do this to avoid stretching it.
                if (GetVideoOrientation() == VideoOrientation.BottomRight)
                {
                    (px, py) = (py, px);
                }

                var _source = Texture2D.CreateExternalTexture((int)px, (int)py, TextureFormat.RGBA32, false, true, texptr);
                //Make a texture of the proper size for the video to output to

                var result = new TextureView(_source);

                Debug.Log($"texture size {px} {py} | {result.Size}");

                return result;
                // if (m_lRenderer != null)
                //     m_lRenderer.material.mainTexture = texture;
                //
                // if (m_rRenderer != null)
                //     m_rRenderer.material.mainTexture = texture;

//             if (m_l360Renderer != null)
//                 m_l360Renderer.material.mainTexture = texture;
//
//             if (m_r360Renderer != null)
//                 m_r360Renderer.material.mainTexture = texture;
            }

            return null;
        }

        //This returns the video orientation for the currently playing video, if there is one
        public VideoOrientation? GetVideoOrientation()
        {
            var tracks = mediaPlayer?.Tracks(TrackType.Video);

            if (tracks == null || tracks.Count == 0)
                return null;

            var orientation =
                tracks[0]?.Data.Video.Orientation; //At the moment we're assuming the track we're playing is the first track

            return orientation;
        }

        public void SetARNull()
        {
            if (mediaPlayer is not null)
                mediaPlayer.AspectRatio = null;
        }

        public void SetAR4_3()
        {
            if (mediaPlayer is not null)
                mediaPlayer.AspectRatio = "4:3";
        }

        public void SetAR169()
        {
            if (mediaPlayer is not null)
                mediaPlayer.AspectRatio = "16:9";
        }

        // public void SetAR16_10()
        // {
        //     if (mediaPlayer is not null)
        //         mediaPlayer.AspectRatio = "16:10";
        // }

        // public void SetAR_2_35_to_1()
        // {
        //     if (mediaPlayer is not null)
        //         mediaPlayer.AspectRatio = "2.35:1";
        // }

        public void Open(string path)
        {
            Log("[MainDisplay] Open " + path);
            if (path.ToLower().EndsWith(".url") || path.ToLower().EndsWith(".txt"))
            {
                var urlContent = File.ReadAllText(path);
                var lines = urlContent.Split('\n').ToList();
                lines.RemoveAll(string.IsNullOrEmpty);

                if (lines.Count <= 0) throw new IOException($"No line defined in file `${path}`");

                Args = new VLCArgs(lines.ToList(), FromType.FromLocation);
            }
            else
            {
                Args = new VLCArgs(new List<string> { path }, FromType.FromPath);
            }

            Open();
        }

        public void Open()
        {
            Log("[MainDisplay] Open");

            if (mediaPlayer?.Media != null)
                mediaPlayer.Media.Dispose();

            var parameters = Args.Parameters;
            Debug.Log($"Opening `{Args.Location}` with {parameters.Length} parameter(s) {string.Join(" ", parameters)}");

            // mediaPlayer.Media = new Media(new Uri(Args.Location), parameters);
            // mediaPlayer.Media = new Media(Args.Location, Args.FromType, parameters);

            var m = new Media(Args.Location, Args.FromType);
            foreach (var parameter in parameters)
            {
                m.AddOption(parameter);
            }

            mediaPlayer.Media = m;

            Task.Run(async () =>
            {
                var result = await mediaPlayer.Media.ParseAsync(libVLC, MediaParseOptions.ParseNetwork);
                var trackList = mediaPlayer.Media.TrackList(TrackType.Video);

                Debug.Log($"tracklist of {trackList.Count}");
                Debug.Log($"projection {trackList[0].Data.Video.Projection}");

                // TODO: add SBS / OU / TB filename recognition

                // if (_is360)
                // {
                //     Debug.Log("The video is a 360 video");
                //     SetVideoMode(VideoMode._360_2D);
                // }
                //
                // else
                // {
                //     Debug.Log("The video was not identified as a 360 video by VLC");
                // SetVideoMode(VideoMode.Mono);
                // }

                trackList.Dispose();
            });

            // Play();
        }

        public void Play()
        {
            Task.Run(async () =>
                {
                    var isSuccessful = await mediaPlayer.PlayAsync();

                    Assert.IsTrue(isSuccessful && isPlaying, "should be playing");

                    uint height = 0;
                    uint width = 0;
                    mediaPlayer?.Size(0, ref width, ref height);

                    Assert.AreNotEqual(height, 0, "height should not be 0");
                    Assert.AreNotEqual(width, 0, "width should not be 0");

                    Debug.Log("Playing ...");
                }
            );
        }

        public void Pause()
        {
            Log("[MainDisplay] Pause");
            mediaPlayer.Pause();
        }

        public void Stop()
        {
            Log("[MainDisplay] Stop");
            mediaPlayer?.Stop();

            // clear to black
            // TextureView?.Destroy();
        }

        public void SeekForward10()
        {
            SeekSeconds(10);
        }

        public void SeekBack10()
        {
            SeekSeconds(-10);
        }

        public void SeekSeconds(float seconds)
        {
            Seek((long)seconds * 1000);
        }

        public void Seek(long timeDelta)
        {
            Debug.Log("[MainDisplay] Seek " + timeDelta);
            mediaPlayer.SetTime(mediaPlayer.Time + timeDelta);
        }

        public void SetTime(long time)
        {
            Log("[MainDisplay] SetTime " + time);
            mediaPlayer.SetTime(time);
        }

        public void SetVolume(int volume = 100)
        {
            Log("[MainDisplay] SetVolume " + volume);
            mediaPlayer.SetVolume(volume);
        }

        public int Volume
        {
            get
            {
                if (mediaPlayer == null)
                    return 0;
                return mediaPlayer.Volume;
            }
        }

        private bool isPlaying
        {
            get
            {
                if (mediaPlayer == null)
                    return false;
                return mediaPlayer.IsPlaying;
            }
        }

        public long Duration
        {
            get
            {
                if (mediaPlayer == null || mediaPlayer.Media == null)
                    return 0;
                return mediaPlayer.Media.Duration;
            }
        }

        public long Time
        {
            get
            {
                if (mediaPlayer == null)
                    return 0;
                return mediaPlayer.Time;
            }
        }

        //Converts MediaTrackList objects to Unity-friendly generic lists. Might not be worth the trouble.
        private List<MediaTrack> ConvertMediaTrackList(MediaTrackList tracklist)
        {
            if (tracklist == null)
                return new List<MediaTrack>(); //Return an empty list

            var tracks = new List<MediaTrack>((int)tracklist.Count);
            for (uint i = 0; i < tracklist.Count; i++) tracks.Add(tracklist[i]);
            return tracks;
        }

        public List<MediaTrack> Tracks(TrackType type)
        {
            Log("[MainDisplay] Tracks " + type);
            return ConvertMediaTrackList(mediaPlayer?.Tracks(type));
        }

        public MediaTrack SelectedTrack(TrackType type)
        {
            Log("[MainDisplay] SelectedTrack " + type);
            return mediaPlayer?.SelectedTrack(type);
        }

        public void Select(MediaTrack track)
        {
            Log("[MainDisplay] Select " + track.Name);
            mediaPlayer?.Select(track);
        }

        public void Unselect(TrackType type)
        {
            Log("[MainDisplay] Unselect " + type);
            mediaPlayer?.Unselect(type);
        }
    }
}
