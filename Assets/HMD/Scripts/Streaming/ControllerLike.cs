using MAVLinkAPI.Util.NullSafety;
using UnityEngine;
using UnityEngine.UI;

namespace HMD.Scripts.Streaming
{
    public abstract class ControllerLike : MonoBehaviour
    {
        // public ScreenLike screen;
        // public DashPanels dashPanels;

        [Required] public GameObject icon = null!;

        public abstract FeedLike Feed { get; }

        [Required] public Button playButton = null!;
        [Required] public Button pauseButton = null!;
        [Required] public Button stopButton = null!;

        public abstract void BindUI();
    }
}