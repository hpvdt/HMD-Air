using UnityEngine;
using UnityEngine.SceneManagement;


public class DebugTools : MonoBehaviour
{
    public bool isActive = true;
    public int awakeFrameRate = 60;

    //public SndPlayer sndPlayer;
    //public DisplayMsg displayMsg;

    void Awake() { if(isActive) Application.targetFrameRate = awakeFrameRate; }
    void Update()
    {
        //Disable component if is not active
        if(!isActive) { this.enabled = false; return; }
        //

        //Quit Playmode by pressing F1
        if (Input.GetKeyDown(KeyCode.F1))
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        //

        //Restart Game
        if (Input.GetKeyDown(KeyCode.F7)) SceneManager.LoadScene(0);
        //

        //Changes FrameRate
        if (Input.GetKeyDown(KeyCode.M) && Input.GetKey(KeyCode.Delete))
        {
            if (Application.targetFrameRate == 60) { Application.targetFrameRate = 120; }
            else if (Application.targetFrameRate == 120) { Application.targetFrameRate = -1; }
            else if (Application.targetFrameRate == -1) { Application.targetFrameRate = 15; }
            else if (Application.targetFrameRate == 15) { Application.targetFrameRate = 30; }
            else if (Application.targetFrameRate == 30) { Application.targetFrameRate = 60; }

            SndPlayer.playClick(); //if (sndPlayer != null) sndPlayer.clickGuiSnd();
            DisplayMsg.show("FPS = " + Application.targetFrameRate, 5); //if (displayMsg != null) displayMsg.displayQuickMsg("FPS = " + Application.targetFrameRate);

            print("FPS = " + Application.targetFrameRate);
        }
        //


        //Show current FrameRate on console
        if (Input.GetKeyDown(KeyCode.N) && Input.GetKey(KeyCode.Delete))
        {
            DisplayMsg.show("Current FPS = " + (1 / Time.deltaTime)); //if (displayMsg != null) displayMsg.displayQuickMsg("Current FPS = " + (1 / Time.deltaTime));
            print("Current FPS = " + 1 / Time.deltaTime);
        }
        //

    }
}
