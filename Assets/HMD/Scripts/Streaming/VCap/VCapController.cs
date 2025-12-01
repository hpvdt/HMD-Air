using HMD.Scripts.Util;
using MAVLinkAPI.Util.NullSafety;
using UnityEngine;
using UnityEngine.UI;

namespace HMD.Scripts.Streaming.VCap
{
    public class VCapController : ControllerLike
    {
        [Required] public VCapScreen screen = null!;

        [Required] public Button fileButton = null!;

        [Required] public Button devicesButton = null!;

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

            devicesButton.onClick.Rebind(() => { screen.feed.LogAllDevices(); });

            screen.BindUI();
        }
    }
}