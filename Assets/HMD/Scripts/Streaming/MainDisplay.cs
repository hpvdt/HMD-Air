using System.Collections;
using System.Collections.Generic;
using HMD.Scripts.Util;
using LibVLCSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace HMD.Scripts.Streaming
{
    public abstract class MainDisplay : MonoBehaviourWithLogging
    {
        public enum VideoMode
        {
            Mono,

            _3D, // Side-By-Side/SBS

            _3D_OU // Over-Under
            // TODO: there is no Mono_OU?
        }

        [SerializeField] public VideoMode videoMode = VideoMode.Mono; // 2d by default

        protected abstract FeedLike Feed { get; }

        private AndroidJavaClass _brightnessHelper;

        [SerializeField] private Camera leftCamera;
        [SerializeField] private Camera centerCamera;
        [SerializeField] private Camera rightCamera;

        [SerializeField] private Camera airPoseCamera;

        private List<Camera> MainCameras()
        {
            return new List<Camera> { centerCamera, airPoseCamera };
        }

        private List<Camera> AllCameras()
        {
            var result = MainCameras();
            result.Add(leftCamera);
            result.Add(rightCamera);

            return result;
        }

        // private float _leftCameraXOnStart;
        // private float _rightCameraXOnStart;

        // [SerializeField]
        // // public MyIAPHandler myIAPHandler;
        // private GameObject _hideWhenLocked;

        // private GameObject _lockScreenNotice;
        // private GameObject _menuToggleButton;
        //
        // private GameObject _logo;
        
        [SerializeField] private GameObject leftEyeScreen;
        [SerializeField] private GameObject rightEyeScreen;

        private List<GameObject> AllScreens()
        {
            return new List<GameObject> { leftEyeScreen, rightEyeScreen };
        }

        private Renderer _morphDisplayLeftRenderer;
        private Renderer _morphDisplayRightRenderer;

        [SerializeField] public Slider fovBar;

        // [SerializeField] public Slider scaleBar;

        [SerializeField] public Slider distanceBar;

        [SerializeField] public Slider deformBar;

        // [SerializeField] public Slider brightnessBar; // TODO: enable
        //
        // [SerializeField] public Slider contrastBar;
        //
        // [SerializeField] public Slider gammaBar;
        //
        // [SerializeField] public Slider saturationBar;
        //
        // [SerializeField] public Slider hueBar;
        //
        // [SerializeField] public Slider sharpnessBar;

        [SerializeField] public Slider horizontalBar;

        [SerializeField] public Slider verticalBar;

        [SerializeField] public Slider depthBar;

        [SerializeField] public Slider focusBar;

        // TODO: group them
        public GameObject cone;

        private bool _screenLocked = false;
        private int _brightnessOnLock = 0;
        private int _brightnessModeOnLock = 0;

        private bool _flipStereo = false;

        public Material m_lMaterial;
        public Material m_rMaterial;
        public Material m_monoMaterial;
        public Material m_leftEyeTBMaterial;
        public Material m_rightEyeTBMaterial;

        private float Yaw;
        private float Pitch;
        private float Roll;

        // private int _3DTrialPlaybackStartedAt = 0;
        // private float _MaxTrialPlaybackSeconds = 15.0f;
        // private bool _isTrialing3DMode = false;

        // private bool m_updatedARSinceOpen = false;
        // private float _aspectRatioOverride;
        // private string _currentARString;

        /// <summary> The previous position. </summary>
        private Vector2 m_PreviousPos; // TODO: mark for removal, only kept as an example of Android UI

        private float fov // 20 for 2D 140 for spherical
        {
            get
            {
                return centerCamera.fieldOfView;
            }
            set
            {
                foreach (var c in AllCameras()) c.fieldOfView = value;
            }
        }

        private AndroidJavaClass unityPlayer;
        private AndroidJavaObject activity;
        private AndroidJavaObject context;

        public TextureView Texture;

        //Unity Awake, OnDestroy, and Update functions

        #region unity
        protected void Awake()
        {
            base.Awake();

            if (fovBar is not null) fovBar.value = fov;
            if (deformBar is not null) deformBar.value = 0.0f;

            // _startPosition = new Vector3(
            //     mainDisplay.transform.position.x,
            //     mainDisplay.transform.position.y,
            //     mainDisplay.transform.position.z
            // );

            // UpdateCameraReferences();

            // _leftCameraXOnStart = leftCamera.transform.position.x;
            // _rightCameraXOnStart = rightCamera.transform.position.x;

            // init
            OnFOVSliderUpdated();

            // TODO: extract lockscreen logic into a separate script
            // _hideWhenLocked = GameObject.Find("HideWhenScreenLocked");
            // _lockScreenNotice = GameObject.Find("LockScreenNotice");
            // _logo = GameObject.Find("logo");
            // _menuToggleButton = GameObject.Find("MenuToggleButton");

            //Setup Screen
            /*if (screen == null)
            screen = GetComponent<Renderer>();
        if (canvasScreen == null)
            canvasScreen = GetComponent<RawImage>();*/

            _morphDisplayLeftRenderer = leftEyeScreen.GetComponent<Renderer>();
            _morphDisplayRightRenderer = rightEyeScreen.GetComponent<Renderer>();

            SetVideoModeMono();
        }


        // private void UpdateColorGrade()
        // {
        //     // Get the Color Grading effect from the camera's post-processing profile
        //     /*ColorGrading colorGrading;
        //     if (camera.TryGetComponent(out PostProcessVolume volume))
        //     {
        //         volume.profile.TryGetSettings(out colorGrading);
        //     }
        //     else
        //     {
        //         return;
        //     }*/
        //
        //     // Set the brightness, contrast, and gamma levels
        //     /*colorGrading.brightness.value = brightnessBar.value;
        //     colorGrading.contrast.value = contrastBar.value;
        //     colorGrading.gamma.value = gammaBar.value;*/
        // }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F1))
                EditorWindow.focusedWindow.maximized = !EditorWindow.focusedWindow.maximized;
#endif

            var newTexture = Feed?.TryGetTexture(Texture);
            if (newTexture != null && newTexture != Texture)
            {
                Texture = newTexture;
                SetVideoModeAsap();

                SetARDefault();
            }
        }

        // private void OnDisable()
        // {
        //     OnDestroy();
        // }
        //
        // private void OnApplicationQuit()
        // {
        //     OnDestroy();
        // }
        //
        // private void OnDestroy()
        // {
        //     //Dispose of mediaPlayer, or it will stay in nemory and keep playing audio
        //     // foreach (var feed in _allFeeds) feed.Dispose();
        // }
        
        
        #endregion

        private static Vector2 SCALE_RANGE = new Vector2(1f, 4.702173720867682f);

        public void OnDeformSliderUpdated()
        {
            // if (deformBar is null) return;

            var value = deformBar.value;

            var scale = Mathf.Lerp(SCALE_RANGE.x, SCALE_RANGE.y, value / 100);

            // Debug.Log("value set to " + value);

            var screens = AllScreens();
            foreach (var screen in screens)
                if (screen is not null)
                {
                    screen.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, value);
                    screen.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(1, value);
                }

            gameObject.transform.localScale = new Vector3(scale, scale, scale);
        }

        private float lerpDuration = 1; // TODO: dynamic duration based on startValue
        private float startValue = 0;
        private float endValue = 10;
        private IEnumerator lerpLZero;
        private IEnumerator lerpLOne;
        private IEnumerator lerpRZero;

        private IEnumerator lerpROne;
        //float valueToLerp;

        // public void TogglePlaneToSphere() // TODO: cleanup
        // {
        //     var current = leftEyeScreen.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(0);
        //     if (current < 50)
        //         AnimatePlaneToSphere();
        //     else
        //         AnimateSphereToPlane();
        // }

        // public void AnimatePlaneToSphere()
        // {
        //     DoPlaneSphereLerp(100.0f);
        // }

        // public void AnimateSphereToPlane()
        // {
        //     DoPlaneSphereLerp(0.0f);
        // }
        //
        // public void DoPlaneSphereLerp(float _endValue)
        // {
        //     if (lerpLZero is not null)
        //         StopCoroutine(lerpLZero);
        //
        //     if (lerpLOne is not null)
        //         StopCoroutine(lerpLOne);
        //
        //     if (lerpRZero is not null)
        //         StopCoroutine(lerpRZero);
        //
        //     if (lerpROne is not null)
        //         StopCoroutine(lerpROne);
        //
        //     endValue = _endValue;
        //
        //     lerpLZero = LerpPlaneToSphere(leftEyeScreen.GetComponent<SkinnedMeshRenderer>(), 0);
        //     lerpLOne = LerpPlaneToSphere(leftEyeScreen.GetComponent<SkinnedMeshRenderer>(), 1);
        //
        //     StartCoroutine(lerpLZero);
        //     StartCoroutine(lerpLOne);
        //
        //     lerpRZero = LerpPlaneToSphere(rightEyeScreen.GetComponent<SkinnedMeshRenderer>(), 0);
        //     lerpROne = LerpPlaneToSphere(rightEyeScreen.GetComponent<SkinnedMeshRenderer>(), 1);
        //
        //     StartCoroutine(lerpRZero);
        //     StartCoroutine(lerpROne);
        // }

        // private IEnumerator LerpPlaneToSphere(SkinnedMeshRenderer renderer, int ShapeIndex)
        // {
        //     float timeElapsed = 0;
        //     startValue = renderer.GetBlendShapeWeight(ShapeIndex);
        //     while (timeElapsed < lerpDuration)
        //     {
        //         renderer.SetBlendShapeWeight(ShapeIndex, Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration));
        //         timeElapsed += UnityEngine.Time.deltaTime;
        //         yield return null;
        //     }
        //
        //     renderer.SetBlendShapeWeight(ShapeIndex, endValue);
        // }

        public void OnBrightnessSliderUpdated()
        {
        }

        public void OnGammaSliderUpdated()
        {
        }

        public void OnContrastSliderUpdated()
        {
        }

        public void OnDistanceSliderUpdated()
        {
            var newDistance = distanceBar.value;
            gameObject.transform.localPosition = new Vector3(
                gameObject.transform.localPosition.x,
                gameObject.transform.localPosition.y,
                newDistance
            );
        }

        /* Horizontal (X) axis offset for screen */
        public void OnHorizontalSliderUpdated()
        {
            var newOffset = horizontalBar.value;
            gameObject.transform.localPosition = new Vector3(
                newOffset,
                gameObject.transform.localPosition.y,
                gameObject.transform.localPosition.z
            );
        }

        /* Vertical (Y) axis offset for screen */
        public void OnVerticalSliderUpdated()
        {
            var newOffset = verticalBar.value;
            gameObject.transform.localPosition = new Vector3(
                gameObject.transform.localPosition.x,
                newOffset,
                gameObject.transform.localPosition.z
            );
        }

        // public void ResetDisplayAdjustments()
        // {
        //     mainDisplay.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        //     mainDisplay.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        // }

        private float leftCameraMinX = -1.5f;
        private float rightCameraMaxX = 1.5f;

        // public void OnDepthBarUpdated()
        // {
        //     var newDepth = depthBar.value;
        //
        //     // move left and right camera closer or further to each other depending on the depthbar value
        //     // if the value is 0, the cameras are the min distance apart from each other on their local x axis (leftCameraXOnStart / rightCameraXOnStart)
        //     // if the value is 100, the cameras are at the max distance apart from each other on their local x axis (leftCameraMinX / rightCameraMaxX)
        //
        //     var leftCameraX = Mathf.Lerp(_leftCameraXOnStart, leftCameraMinX, newDepth / 100.0f);
        //     var rightCameraX = Mathf.Lerp(_rightCameraXOnStart, rightCameraMaxX, newDepth / 100.0f);
        //
        //     Debug.Log($"{newDepth} , {leftCameraX} , {rightCameraX}");
        //
        //     leftCamera.transform.localPosition =
        //         new Vector3(leftCameraX, leftCamera.transform.localPosition.y, leftCamera.transform.localPosition.z);
        //     rightCamera.transform.localPosition =
        //         new Vector3(rightCameraX, rightCamera.transform.localPosition.y, rightCamera.transform.localPosition.z);
        // }

        private static float maxFocal = 15.0f;
        private static float minFocal = -15.0f;

        // public void OnFocusBarUpdated()
        // {
        //     var focus = focusBar.value; // percentage 0-100
        //
        //     /* rotate the left and right camera ever so slightly so that the convergence plane / focus plane changes */
        //     var focal = Mathf.Lerp(minFocal, maxFocal, focus / 100.0f);
        //     leftCamera.transform.localRotation = Quaternion.Euler(0.0f, focal, 0.0f);
        //     rightCamera.transform.localRotation = Quaternion.Euler(0.0f, -focal, 0.0f);
        // }

        // public void UpdateCameraReferences()
        // {
        //     LeftCamera = GameObject.Find("LeftCamera")?.GetComponent<Camera>();
        //     CenterCamera = GameObject.Find("CenterCamera")?.GetComponent<Camera>();
        //     RightCamera = GameObject.Find("RightCamera")?.GetComponent<Camera>();
        // }

        public void OnFOVSliderUpdated()
        {
            // NOTE: NRSDK doesn't support custom FOV on cameras
            // NOTE: TESTING COMMENTING OUT camera.projectionMatrix = statements in NRHMDPoseTracker

            fov = fovBar.value;
            Debug.Log("fov " + fov);

            // Do360Navigation();
        }

        private float _sphereScale;


        //Public functions that expose VLC MediaPlayer functions in a Unity-friendly way. You may want to add more of these.

        #region vlc
        public void Play()
        {
            if (Feed != null)
            {
                cone.SetActive(false); // hide cone logo

                gameObject.SetActive(true);

                Feed.Play();
            }
        }

        private void _stopFeed()
        {
            Feed.Stop();
        }

        public void Stop()
        {
            _stopFeed();

            Log("[MainDisplay] Stop");

            // clear to black
            Texture?.Dispose();
            Texture = null;

            ClearMaterialTextureLinks();

            gameObject.SetActive(false);

            cone.SetActive(true);
        }

        public void Pause()
        {
            Log("Pause");
            Feed.Pause();
        }
        #endregion

        //Private functions create and destroy VLC objects and textures

        #region internal
        //Converts MediaTrackList objects to Unity-friendly generic lists. Might not be worth the trouble.
        // private List<MediaTrack> ConvertMediaTrackList(MediaTrackList tracklist)
        // {
        //     if (tracklist == null)
        //         return new List<MediaTrack>(); //Return an empty list
        //
        //     var tracks = new List<MediaTrack>((int)tracklist.Count);
        //     for (uint i = 0; i < tracklist.Count; i++) tracks.Add(tracklist[i]);
        //     return tracks;
        // }

        public void ToggleFlipStereo()
        {
            _flipStereo = !_flipStereo;
            SetVideoMode(videoMode);
        }
        
        
        public virtual Frac AspectRatio
        {
            get { return Feed.NativeAspectRatio(); } // by default, has no setter
            set { }
            // may not have a setter for some display 
        }

        public void SetARNull()
        {
            AspectRatio = null;
        }

        public void SetARDefault()
        {
            if (Feed == null) { }
            else
            {
                AspectRatio = Feed.NativeAspectRatio();
            }
        }

        public void SetAR1_1()
        {
            AspectRatio = new Frac(1, 1);
        }

        public void SetAR4_3()
        {
            AspectRatio = new Frac(4, 3);
        }

        public void SetAR16_10()
        {
            AspectRatio = new Frac(16, 10);
        }

        public void SetAR16_9()
        {
            AspectRatio = new Frac(16, 9);
        }

        public void SetAR2_1()
        {
            AspectRatio = new Frac(2, 1);
        }

        public void SetAR_235_to_100()
        {
            AspectRatio = new Frac(235, 100);
        }

        public void ClearMaterialTextureLinks()
        {
            if (_morphDisplayLeftRenderer.material is not null)
            {
                _morphDisplayLeftRenderer.material.mainTexture = null;
                _morphDisplayLeftRenderer.material = null;
            }

            if (_morphDisplayRightRenderer.material is not null)
            {
                _morphDisplayRightRenderer.material.mainTexture = null;
                _morphDisplayRightRenderer.material = null;
            }
        }

        public void SetVideoMode(VideoMode mode)
        {
            videoMode = mode;
            if (Texture != null) SetVideoModeAsap();
        }

        private void SetVideoModeAsap()
        {
            if (Texture == null) throw new VLCException("[SetVideoMode] texture is null!");

            var mode = videoMode;
            Log($"set video mode {mode}");

            ClearMaterialTextureLinks();

            // TODO: this selection is incomplete, 2D_SBS and 2D_OU should be implemented with higher priority
            if (mode == VideoMode.Mono)
            {
                // 2D
                leftEyeScreen.layer = LayerMask.NameToLayer("Default");
                rightEyeScreen.SetActive(false);

                _morphDisplayLeftRenderer.material = m_monoMaterial; // m_lMaterial;
                _morphDisplayLeftRenderer.material.mainTexture = Texture.Effective;
            }
            else
            {
                // 3D

                leftEyeScreen.layer = LayerMask.NameToLayer("LeftEyeOnly");

                rightEyeScreen.SetActive(true);
                rightEyeScreen.layer = LayerMask.NameToLayer("RightEyeOnly");

                if (mode is VideoMode._3D_OU)
                {
                    _morphDisplayLeftRenderer.material = _flipStereo ? m_rightEyeTBMaterial : m_leftEyeTBMaterial;
                    _morphDisplayRightRenderer.material = _flipStereo ? m_leftEyeTBMaterial : m_rightEyeTBMaterial;
                }
                else
                {
                    _morphDisplayLeftRenderer.material = _flipStereo ? m_rMaterial : m_lMaterial;
                    _morphDisplayRightRenderer.material = _flipStereo ? m_lMaterial : m_rMaterial;
                }

                _morphDisplayLeftRenderer.material.mainTexture = Texture.Effective;
                _morphDisplayRightRenderer.material.mainTexture = Texture.Effective;
            }
        }


        // https://answers.unity.com/questions/1549639/enum-as-a-function-param-in-a-button-onclick.html?page=2&pageSize=5&sort=votes

        public void SetVideoModeMono()
        {
            SetVideoMode(VideoMode.Mono);
        }

        public void SetVideoMode3D()
        {
            SetVideoMode(VideoMode._3D);
        }

        public void SetVideoModeOU()
        {
            SetVideoMode(VideoMode._3D_OU);
        }

        // public void ResetScreen() // TODO: bind it to button
        // {
        //     leftEyeScreen.transform.localPosition = _startPosition;
        //     leftEyeScreen.transform.localRotation = Quaternion.identity;
        //     leftEyeScreen.transform.localScale = new Vector3(1, 1, 1);
        //
        //     rightEyeScreen.transform.localPosition = _startPosition;
        //     rightEyeScreen.transform.localRotation = Quaternion.identity;
        //     rightEyeScreen.transform.localScale = new Vector3(1, 1, 1);
        // }



        /// <param name="message">Message string to show in the toast.</param>
        // private static void _showAndroidToastMessage(string message)
        // {
        //     var unityPlayer = new AndroidJavaClass("com.jakedowns.VLC3D.VLC3DActivity");
        //     var unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        //
        //     if (unityActivity != null)
        //     {
        //         var toastClass = new AndroidJavaClass("android.widget.Toast");
        //         unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        //         {
        //             var toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
        //             toastObject.Call("show");
        //         }));
        //     }
        // }

// public void OnSingleTap(string name)
// {
//     Debug.Log($"[SBSVLC] Single Tap Triggered {name}");
//     if (name == "LockScreenButton")
//         if (!_screenLocked)
//             ToggleScreenLock();
// }

// we require a double-tap to unlock
// public void OnDoubleTap(string name)
// {
//     Debug.Log($"[SBSVLC] Double Tap Triggered {name}");
//     if (name == "LockScreenButton")
//         if (_screenLocked)
//             ToggleScreenLock();
// }

// private void GetContext()
// {
//     unityPlayer = new AndroidJavaClass("com.jakedowns.VLC3D.VLC3DActivity");
//     try
//     {
//         activity = unityPlayer?.GetStatic<AndroidJavaObject>("currentActivity");
//         context = activity?.Call<AndroidJavaObject>("getApplicationContext");
//     }
//     catch (Exception e)
//     {
//         Debug.Log("error getting context " + e);
//     }
// }

//     public void ToggleScreenLock()
//     {
//         _screenLocked = !_screenLocked;
//
//         if (_screenLocked)
//         {
//             // Hide All UI except for the lock button
//             _hideWhenLocked.SetActive(false);
//             _lockScreenNotice.SetActive(true);
//             _logo.SetActive(false);
//             _menuToggleButton.SetActive(false);
//             // Lower Brightness
//             var _unityBrightnessOnLock = Screen.brightness;
//             Debug.Log($"lockbrightness Unity brightness on lock {_unityBrightnessOnLock}");
//
// #if UNITY_ANDROID
//             if (!Application.isEditor)
//             {
//                 try
//                 {
//                     // get int from _brightnessHelper
//                     _brightnessOnLock = (int)(_brightnessHelper?.CallStatic<int>("getBrightness"));
//
//                     _brightnessModeOnLock = (int)(_brightnessHelper?.CallStatic<int>("getBrightnessMode"));
//
//                     Debug.Log($"lockbrightness Android brightness on lock {_brightnessOnLock}");
//                 }catch(Exception e)
//                 {
//                     Debug.Log("Error getting brightness " + e.ToString());
//                 }
//
//                 // Set it to 0? 0.1?
//                 //Debug.Log($"set brightness with unity");
//                 //Screen.brightness = 0.1f;
//
//                 if (context is null)
//                 {
//                     Debug.Log("context is null");
//                     GetContext();
//                 }
//                 if (context is not null)
//                 {
//                     // TODO: maybe try to fetch it again now?
//
//                     object _args = new object[2] { context, 1 };
//
//                     // call _brightnessHelper
//                     _brightnessHelper?.CallStatic("SetBrightness", _args);
//                 }
//                  
//
//             }
// #endif
//         }
//         else
//         {
// #if UNITY_ANDROID
//             if (!Application.isEditor)
//             {
//                 if (context is null)
//                 {
//                     Debug.Log("context is null");
//                     GetContext();
//                 }
//                 if (context is not null)
//                 {
//                     try
//                     {
//                         object _args = new object[2] { context, _brightnessOnLock };
//                         _brightnessHelper?.CallStatic("setBrightness", _args);
//
//                         // restore brightness mode
//                         object _args_mode = new object[2] { context, _brightnessModeOnLock };
//                         _brightnessHelper?.CallStatic("setBrightnessMode", _args_mode);
//                     }
//                     catch(Exception e)
//                     {
//                         Debug.Log("error setting brightness " + e.ToString());
//                     }
//                     
//                 }
//                 
//             }
// #else
//             // Restore Brightness
//             Screen.brightness = _brightnessOnLock;
// #endif
//
//             // Show All UI when screen is unlocked
//             _hideWhenLocked.SetActive(true);
//             _lockScreenNotice.SetActive(false);
//             _logo.SetActive(true);
//             _menuToggleButton.SetActive(true);
//         }
//     }
        #endregion
    }
}