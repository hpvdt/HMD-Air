namespace HMD.Scripts.Streaming
{
    using UnityEngine;
    using UnityEngine.UI;
    public abstract class ControllerLike : MonoBehaviour
    {

        // public ScreenLike screen;
        // public DashPanels dashPanels;

        public GameObject icon;

        public Button playButton;
        public Button pauseButton;
        public Button stopButton;

        public abstract void BindUI();
    }
}
