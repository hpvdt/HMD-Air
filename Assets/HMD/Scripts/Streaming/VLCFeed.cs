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
    public class VLCFeed : FeedLike, IFeed
    {
        public VLCArgs Args;

        private LibVLC _libVLC;

        //Create a new static LibVLC instance and dispose of the old one. You should only ever have one LibVLC instance.
        private void RefreshLibVLC()
        {
            //Dispose of the old libVLC if necessary
            if (_libVLC != null)
            {
                _libVLC.Dispose();
                _libVLC = null;
            }

            Core.Initialize(Application.dataPath); //Load VLC dlls
            _libVLC = new LibVLC(
                true
            ); //You can customize LibVLC with advanced CLI options here https://wiki.videolan.org/VLC_command-line_help/

            //Setup Error Logging
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            _libVLC.Log += (s, e) =>
            {
                //Always use try/catch in LibVLC events.
                //LibVLC can freeze Unity if an exception goes unhandled inside an event handler.
                try
                {
                    Log($"[VLC-{e.Level}] [{s}{e.Module}] " + e.Message);
                }
                catch (Exception ex)
                {
                    LogError("[VLC]Exception caught in libVLC.Log:\n" + ex);
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
        }

        private MediaPlayer _player;

        //Create a new MediaPlayer object and dispose of the old one. 
        private void RefreshMediaPlayer()
        {
            Log("CreateMediaPlayer");
            if (_player != null) DestroyMediaPlayer();
            _player = new MediaPlayer(libVLC);
            Log("Media Player SET!");
        }

        public MediaPlayer Player
        {
            get
            {
                if (_player == null) RefreshMediaPlayer();
                return _player;
            }
        }


        //Dispose of the MediaPlayer object. 
        public void DestroyMediaPlayer()
        {
            Stop();

            Log("DestroyMediaPlayer");
            _player?.Stop();
            _player?.Dispose();
            _player = null;

            _libVLC?.Dispose();
            _libVLC = null;
        }

        public void Dispose()
        {
            DestroyMediaPlayer();
        }

        #region unity
        protected override void Awake()
        {
            base.Awake();

            //Setup Media Player
            RefreshMediaPlayer();

            Log($"[VLC] LibVLC version and architecture {libVLC.Changeset}");
            Log($"[VLC] LibVLCSharp version {typeof(LibVLC).Assembly.GetName().Version}");
        }
        #endregion

        public TextureView? TryGetTexture(TextureView? existing)
        {
            //Get size every frame
            uint height = 0;
            uint width = 0;
            Player?.Size(0, ref width, ref height);

            //Automatically resize output textures if size changes
            var tex = existing;
            if (tex == null || tex.Size.Value != (width, height))
            {
                tex?.Dispose();
                tex = TryGenerateTexture(width, height); // may fail if video is not ready
            }

            if (tex != null)
            {
                //Update the vlc texture (tex)
                var texPtr = Player.GetTexture(width, height, out var updated);
                if (updated)
                {
                    var src = (Texture2D)tex.Source;
                    src.UpdateExternalTexture(texPtr);

                    //Copy the vlc texture into the output texture, flipped over
                    Graphics.Blit(src, tex.Cache, Transform, Offset);
                    //If you wanted to do post processing outside of VLC you could use a shader here.
                }
            }

            return tex;
        }

        //Resize the output textures to the size of the video
        private TextureView? TryGenerateTexture(uint px, uint py)
        {
            var texptr = Player.GetTexture(px, py, out var updated);
            if (px != 0 && py != 0 && updated && texptr != IntPtr.Zero)
            {
                //If the currently playing video uses the Bottom Right orientation, we have to do this to avoid stretching it.
                if (GetVideoOrientation() == VideoOrientation.BottomRight)
                {
                    (px, py) = (py, px);
                }

                var _source =
                    Texture2D.CreateExternalTexture((int)px, (int)py, TextureFormat.RGBA32, false, true, texptr);
                //Make a texture of the proper size for the video to output to

                var result = new TextureView(_source);

                Log($"texture size {px} {py} | {result.Size}");

                return result;
            }

            return null;
        }

        //This returns the video orientation for the currently playing video, if there is one
        private VideoOrientation? GetVideoOrientation()
        {
            var tracks = Player?.Tracks(TrackType.Video);

            if (tracks == null || tracks.Count == 0)
                return null;

            var orientation =
                tracks[0]?.Data.Video
                    .Orientation; //At the moment we're assuming the track we're playing is the first track

            return orientation;
        }

        public void SetARNull()
        {
            if (Player is not null)
                Player.AspectRatio = null;
        }

        public void SetAR4_3()
        {
            if (Player is not null)
                Player.AspectRatio = "4:3";
        }

        public void SetAR16_10()
        {
            if (Player is not null)
                Player.AspectRatio = "16:10";
        }

        public void SetAR16_9()
        {
            if (Player is not null)
                Player.AspectRatio = "16:9";
        }

        public void SetAR2_1()
        {
            if (Player is not null)
                Player.AspectRatio = "2:1";
        }
        // public void SetAR_2_35_to_1()
        // {
        //     if (mediaPlayer is not null)
        //         mediaPlayer.AspectRatio = "2.35:1";
        // }

        public void Open(string path)
        {
            Log("Open " + path);
            var _path = path.ToLower();

            if (_path.EndsWith(".url") || _path.EndsWith(".txt") || _path.EndsWith(".mrl"))
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

            _openArgs();
        }

        private void _openArgs()
        {
            Log("Open");

            if (Player?.Media != null)
                Player.Media.Dispose();

            var parameters = Args.Parameters;
            Debug.Log(
                $"Opening `{Args.Location}` with {parameters.Length} parameter(s)"
                + $" {string.Join(" ", parameters.Select(s => $"`{s}`").ToArray())}"
            );

            // mediaPlayer.Media = new Media(new Uri(Args.Location), parameters);
            // mediaPlayer.Media = new Media(Args.Location, Args.FromType, parameters);

            var m = new Media(Args.Location, Args.FromType);
            foreach (var parameter in parameters)
            {
                m.AddOption(parameter);
            }

            Player.Media = m;

            Task.Run(async () =>
            {
                var result = await Player.Media.ParseAsync(libVLC, MediaParseOptions.ParseNetwork);
                var trackList = Player.Media.TrackList(TrackType.Video);

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
        }

        public void Stop()
        {
            Log("Stop");
            Player?.Stop();
        }

        public void Play()
        {
            Task.Run(async () =>
                {
                    var isSuccessful = await Player.PlayAsync();

                    Assert.IsTrue(isSuccessful && isPlaying, "should be playing");

                    uint height = 0;
                    uint width = 0;
                    Player?.Size(0, ref width, ref height);

                    Assert.AreNotEqual(height, 0, "height should not be 0");
                    Assert.AreNotEqual(width, 0, "width should not be 0");

                    Debug.Log("Playing ...");
                }
            );
        }

        public void Pause()
        {
            Player.Pause();
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
            Debug.Log("Seek " + timeDelta);
            Player.SetTime(Player.Time + timeDelta);
        }

        public void SetTime(long time)
        {
            Log("SetTime " + time);
            Player.SetTime(time);
        }

        public void SetVolume(int volume = 100)
        {
            Log("SetVolume " + volume);
            Player.SetVolume(volume);
        }

        public int Volume
        {
            get
            {
                if (Player == null)
                    return 0;
                return Player.Volume;
            }
        }

        private bool isPlaying
        {
            get
            {
                if (Player == null)
                    return false;
                return Player.IsPlaying;
            }
        }

        public long Duration
        {
            get
            {
                if (Player == null || Player.Media == null)
                    return 0;
                return Player.Media.Duration;
            }
        }

        public long Time
        {
            get
            {
                if (Player == null)
                    return 0;
                return Player.Time;
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
            Log("Tracks " + type);
            return ConvertMediaTrackList(Player?.Tracks(type));
        }

        public MediaTrack SelectedTrack(TrackType type)
        {
            Log("SelectedTrack " + type);
            return Player?.SelectedTrack(type);
        }

        public void Select(MediaTrack track)
        {
            Log("Select " + track.Name);
            Player?.Select(track);
        }

        public void Unselect(TrackType type)
        {
            Log("Unselect " + type);
            Player?.Unselect(type);
        }
    }
}
