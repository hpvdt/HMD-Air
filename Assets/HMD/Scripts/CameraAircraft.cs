using UnityEngine;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////// - CameraAircraft Script - Created by Maloke Games 2019 - Visit us here: https://maloke.itch.io/ 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////
////// This script is a very basic camera movement used to demonstrate how the Aircraft HUD GUI works
////// It does not contains proper collision nor physics but feel free to modify and use it as you wish!
//////
////// Controls: W-S (Pitch), A-D (Roll), Q-E (Yaw), R-F (Ligt), T-Space (Reset Attitude), Y (Toogle Sound), Shift-Ctrl (Faster/Slower speed)
//////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class CameraAircraft : MonoBehaviour
{
    public static CameraAircraft current;

    public bool isActive = true, cursorStartLocked = false, turbulence = true;
    public float pitchFactor = 1, rollFactor = 1, yawFactor = 1, thrust = 1, lift = 1;

    public AudioSource audioSource;
    public AudioClip audioClip;
    
    float boost = 1f, brake = 1f;

    ////////////////// Inicialization
    void Awake()
    {
        Application.targetFrameRate = 60;
        if (audioSource == null) audioSource.GetComponent<AudioSource>();
    }
    void Start() { if(cursorStartLocked) Cursor.lockState = CursorLockMode.Locked; else Cursor.lockState = CursorLockMode.None; }
    void OnEnable() { current = this; }
    //////////////////


    ////////////////////////////////////// Aircraft Control
    void Update()
    {
        //Return if control is not activated
        if (!isActive) return;

        //Cursor lock-unlock with Tab key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Cursor.lockState != CursorLockMode.Locked) Cursor.lockState = CursorLockMode.Locked; else Cursor.lockState = CursorLockMode.None;
            if (audioSource != null && audioClip != null) audioSource.PlayOneShot(audioClip);
        }
        //

        //Enable or Disable Sound
        if (Input.GetKeyUp(KeyCode.Y) && audioSource != null) { audioSource.mute = !audioSource.mute; if (audioClip != null) audioSource.PlayOneShot(audioClip); }
        //

        //Reset Aircraft Attitude
        if (Input.GetKey(KeyCode.T) || Input.GetMouseButtonDown(2)) transform.localRotation = Quaternion.identity;
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButtonDown(1)) transform.localEulerAngles = new Vector3( 0, transform.localEulerAngles.y, 0);
        //

        //Boost or Brake
        if (Input.GetKey(KeyCode.LeftShift)) boost = 2; else boost = 1;
        if (Input.GetKey(KeyCode.LeftControl)) brake = .25f; else brake = 1;
        //

        //Aircraft Thrust
        transform.Translate(Vector3.forward * Time.deltaTime * (thrust * 5) * boost * brake);
        //

        //Lift
        transform.Translate(((Input.GetKey(KeyCode.R) ? 1 : 0) - (Input.GetKey(KeyCode.F) ? 1 : 0)) * Vector3.up * Time.deltaTime * (lift * 5) * boost * brake); //Up and Down (Drones+Helicopters)
        //

        //Mouse Control (Only if cursor is locked)
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            transform.Rotate(
                Input.GetAxis("Mouse Y") * Time.deltaTime * (pitchFactor * 100) * boost * brake * 2,
                0, //Input.GetAxis("Mouse X") * Time.deltaTime * (yawFactor * 100) * boost * brake,
                -Input.GetAxis("Mouse X") * Time.deltaTime * (rollFactor * 100) * boost * brake / 2,
                Space.Self);
        }
        //

        //Keyboard Control
        transform.Rotate(
              ((Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0) + (turbulence ?   Random.Range(-0.05f, 0.05f) : 0)) * Time.deltaTime * (pitchFactor * 100) * boost * brake,
              ((Input.GetKey(KeyCode.E) ? 1 : 0) - (Input.GetKey(KeyCode.Q) ? 1 : 0) + (turbulence ?     Random.Range(-0.1f, 0.1f) : 0)) * Time.deltaTime * (yawFactor   * 100) * boost * brake,
              ((Input.GetKey(KeyCode.A) ? 1 : 0) - (Input.GetKey(KeyCode.D) ? 1 : 0) + (turbulence ? Random.Range(-0.125f, 0.125f) : 0)) * Time.deltaTime * (rollFactor  * 100) * boost * brake,
              Space.Self);
        //
    }
    //////////////////////////////////////
}
//