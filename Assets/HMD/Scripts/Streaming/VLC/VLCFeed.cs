namespace HMD.Scripts.Streaming.VLC
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using LibVLCSharp;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Application = UnityEngine.Device.Application;
    public class VlcFeed : FeedLike
    {
        public bool DebugVLCPlayer;

        public VlcArgs Args;

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
                    if (DebugVLCPlayer) Log($"[VLC-{e.Level}] [{s}{e.Module}] " + e.Message);
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

        public MediaPlayer Player
        {
            get
            {
                if (_player == null)
                {
                    Log($"LibVLC version and architecture {libVLC.Changeset}");
                    Log($"LibVLCSharp version {typeof(LibVLC).Assembly.GetName().Version}");
                    _player = new MediaPlayer(libVLC);
                }
                return _player;
            }
        }


        //Dispose of the MediaPlayer object. 
        public void DestroyMediaPlayer()
        {
            if (_player != null)
            {
                Stop();

                _player?.Stop();
                _player?.Dispose();
                _player = null;

                _libVLC?.Dispose();
                _libVLC = null;

                Log("Destroyed");
            }
        }

        public override void Dispose()
        {
            DestroyMediaPlayer();
        }

        protected override TextureView? TryGetTextureIfValid(TextureView? existing)
        {
            //Get size every frame

            //Automatically resize output textures if size changes
            var tex = existing;
            var srcSize = GetSize();

            if (tex == null || tex.Source.GetType().IsInstanceOfType(typeof(Texture2D)) || tex.Size.Value != srcSize)
            {
                var newTex = TryGenerateTexture(srcSize.Item1, srcSize.Item2); // may fail if video is not ready

                tex?.Dispose();
                tex = newTex;
            }

            if (tex != null)
            {
                //Update the vlc texture (tex)
                var texPtr = Player.GetTexture(srcSize.Item1, srcSize.Item2, out var updated);
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
            var texPtr = Player.GetTexture(px, py, out var updated);
            if (px != 0 && py != 0 && updated && texPtr != IntPtr.Zero)
            {
                //If the currently playing video uses the Bottom Right orientation, we have to do this to avoid stretching it.
                if (GetVideoOrientation() == VideoOrientation.BottomRight)
                {
                    (px, py) = (py, px);
                }

                var _source =
                    Texture2D.CreateExternalTexture((int)px, (int)py, TextureFormat.RGBA32, false, true, texPtr);
                //Make a texture of the proper size for the video to output to

                var result = new TextureView(_source);

                // Player.AspectRatio = result.AspectRatioStr.Value;

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

        public void Open(string path)
        {
            var _path = path.ToLower();

            if (_path.EndsWith(".url") || _path.EndsWith(".txt") || _path.EndsWith(".mrl"))
            {
                var urlContent = File.ReadAllText(path);
                var lines = urlContent.Split('\n').ToList();
                lines.RemoveAll(string.IsNullOrEmpty);

                if (lines.Count <= 0) throw new IOException($"No line defined in file `${path}`");

                Args = new VlcArgs(lines.ToList(), FromType.FromLocation);
            }
            else
            {
                Args = new VlcArgs(new List<string> { path }, FromType.FromPath);
            }

            _openArgs();
        }

        private void _openArgs()
        {
            if (Player?.Media != null)
                Player.Media.Dispose();

            var parameters = Args.Parameters;
            Log(
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

                // TODO: add SBS / OU / TB filename recognition

                var projection = trackList[0].Data.Video.Projection;

                Debug.Log($"Video uses {projection} projection");

                trackList.Dispose();
            });
        }

        public override void Stop()
        {
            Log("Stop");
            Player?.Stop();
        }

        public override void Play()
        {
            Task.Run(async () =>
                {
                    var isSuccessful = await Player.PlayAsync();

                    Assert.IsTrue(isSuccessful && isPlaying, "should be playing");

                    // TODO: this should be useless now after VLC DirectShow fix
                    // Assert.AreNotEqual(height, 0, "height should not be 0");
                    // Assert.AreNotEqual(width, 0, "width should not be 0");

                    Debug.Log("Playing ...");
                }
            );
        }

        public override void Pause()
        {
            Player.Pause();
        }

        protected override (uint, uint) GetSize()
        {
            uint height = 0;
            uint width = 0;
            Player.Size(0, ref width, ref height);

            return (width, height);
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
