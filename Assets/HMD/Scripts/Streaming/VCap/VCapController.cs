namespace HMD.Scripts.Streaming.VCap
{
    using HMD_Commons.Scripts;
    using UnityEngine;
    using UnityEngine.UI;
    using Util;
    public class VCapController : ControllerLike
    {
        [Required] public VCapScreen screen;

        [Required] public Button fileButton;

        [Required] public Button devicesButton;

        // TODO: do we need buttons for next/previous track/vCapDevice?

        private void Start()
        {
            screen.Stop();

            Init();
        }

        public void Init()
        {
            if (screen?.feed is null)
            {
                Debug.LogError("VCap feed not found");
                return;
            }
        }

        public override void BindUI()
        {
            playButton.onClick.Rebind(() =>
            {
                screen.Play();
            });
            pauseButton.onClick.Rebind(() => { screen.Pause(); });
            stopButton.onClick.Rebind(() => { screen.Stop(); });

            fileButton.onClick.Rebind(() => { screen.PromptUserFilePicker(); });

            devicesButton.onClick.Rebind(() => { screen.feed.LogAllDevices(); });

            screen.BindUI();
        }
    }
}
