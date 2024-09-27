using HMDCommons.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace HMD.Scripts.Streaming
{
    public abstract class ControllerLike : MonoBehaviour
    {
        // public ScreenLike screen;
        // public DashPanels dashPanels;

        [Required] public GameObject icon;

        [Required] public Button playButton;
        [Required] public Button pauseButton;
        [Required] public Button stopButton;

        public abstract void BindUI();
    }
}