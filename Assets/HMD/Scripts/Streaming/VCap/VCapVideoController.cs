using HMD.Scripts.Util;
using MAVLinkAPI.Util.NullSafety;
using UnityEngine;
using UnityEngine.UI;

namespace HMD.Scripts.Streaming.VCap
{
    public class VCapVideoController : VideoControllerLike
    {
        [Required] public VCapScreen screen = null!;

        public override FeedLike Feed => screen.feed;

        [Required] public Button fileButton = null!;

        // TODO: do we need buttons for next/previous track/vCapDevice?

        private void Start()
        {
            screen.Stop();

            Init();
        }

        public void Init()
        {
            if (screen?.feed is null) Debug.LogError("VCap feed not found");
        }

        public override void BindUI()
        {
            playButton.onClick.Rebind(() => { screen.Play(); });
            pauseButton.onClick.Rebind(() => { screen.Pause(); });
            stopButton.onClick.Rebind(() => { screen.Stop(); });

            fileButton.onClick.Rebind(() => { screen.PromptUserFilePicker(); });

            screen.BindUI();
        }
    }
}