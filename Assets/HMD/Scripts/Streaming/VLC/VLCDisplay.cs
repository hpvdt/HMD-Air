namespace HMD.Scripts.Streaming.VLC
{
    using UnityEngine;
    using Util;
    
    public class VlcDisplay : MainDisplay
    {
        
        [HideInInspector]
        public VlcController controller;

        [SerializeField] public VlcFeed vlcFeed;

        protected override FeedLike Feed
        {
            get { return vlcFeed; }
        }

        public override Frac AspectRatio
        {
            get
            {
                var text = vlcFeed.Player.AspectRatio;
                if (text == null) return Feed.NativeAspectRatio();

                return Frac.FromRatioText(text);
            }
            set
            {
                vlcFeed.Player.AspectRatio = value?.ToRatioText();

                var actual = AspectRatio;
                var n_actual = (float) (actual.ToDouble());
                if (m_lMaterial is not null)
                    m_lMaterial.SetFloat("AspectRatio", n_actual);

                // todo: are they redundant? why are they affected instead of other materials?
                if (m_rMaterial is not null)
                    m_rMaterial.SetFloat("AspectRatio",  n_actual);

                var updater = new AspectRatioUpdater(this);
                updater.SyncAll();
            }
        }
        
        
        public void PromptUserFilePicker()
        {
            var fileTypes = new[] { "*", "Media resource locator/mrl,url,txt", "video/*", "Video(others)/movie" };

            // Pick image(s) and/or video(s)
            var permission = NativeFilePicker.PickFile(path =>
            {
                if (path == null)
                {
                    Debug.Log("Operation cancelled");
                }
                else
                {
                    Debug.Log("Picked file: " + path);

                    vlcFeed.Open(path);
                    Play();
                }
            }, fileTypes);

            if (permission is not NativeFilePicker.Permission.Granted)
            {
                Debug.Log("Permission not granted: " + permission);
            }
        }
    }
}
