namespace HMD.Scripts.Streaming.VLC
{
    using System.Collections.Generic;
    using System.Linq;
    using HMD_Commons.Scripts;
    using SFB;
    using UnityEngine;
    using Util;

    public class VLCScreen : ScreenLike
    {
        [HideInInspector] public VLCController controller;

        [Required] [SerializeField] public VLCFeed feed;

        protected override FeedLike Feed => feed;

        public override Frac AspectRatio
        {
            get
            {
                var text = feed.Player.AspectRatio;
                if (text == null) return Feed.NativeAspectRatio();

                return Frac.FromRatioText(text);
            }
            set
            {
                feed.Player.AspectRatio = value?.ToRatioText();

                var actual = AspectRatio;
                var actualF = (float)actual.ToDouble();
                if (m_lMaterial is not null)
                    m_lMaterial.SetFloat("AspectRatio", actualF);

                // todo: are they redundant? why are they affected instead of other materials?
                if (m_rMaterial is not null)
                    m_rMaterial.SetFloat("AspectRatio", actualF);

                var updater = new AspectRatioUpdater(this);
                updater.SyncAll();
            }
        }


        public void PromptUserFilePicker()
        {
            // from Claude 3.5
            var video = new List<string>
            {
                // Video formats
                "mp4", "avi", "mkv", "mov", "wmv", "flv", "webm", "m4v", "mpg", "mpeg", "3gp"
            };

            var audio = new List<string>
            {
                // Audio formats
                "mp3", "wav", "ogg", "flac", "aac", "wma", "m4a", "opus"
            };

            var locator = new List<string>
            {
                "mrl", "url", "txt"
            };

            var supported = video.Union(audio).Union(locator);

            var unused = new List<string>
            {
                // Playlist formats
                "m3u", "pls",

                // DVD and Blu-ray formats
                "vob", "iso", "m2ts",

                // Streaming formats
                "m3u8", "ts",

                // Subtitle formats
                "srt", "sub", "ass"
            };

            // Open file with filter
            var fileTypes = new[]
            {
                new ExtensionFilter("Supported", supported.ToArray()),
                new ExtensionFilter("Video", video.ToArray()),
                new ExtensionFilter("Audio", audio.ToArray()),
                new ExtensionFilter("Media resource locator", locator.ToArray()),
                new ExtensionFilter("Any", "*")
            };
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", fileTypes, false);
            var path = paths.FirstOrDefault();

            if (path == null)
            {
                Debug.Log("Operation cancelled");
            }
            else
            {
                Debug.Log("Picked file: " + path);

                feed.Open(path);
                Play();
            }
        }
    }
}