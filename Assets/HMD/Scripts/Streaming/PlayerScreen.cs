using System.Collections.Generic;
using HMD.Scripts.Util;
using LibVLCSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace HMD.Scripts.Streaming
{

    public abstract class PlayerScreen : MonoBehaviourWithLogging
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


        [SerializeField] private GameObject leftEyeScreen;
        [SerializeField] private GameObject rightEyeScreen;

        private List<GameObject> AllScreens()
        {
            return new List<GameObject> { leftEyeScreen, rightEyeScreen };
        }

        private Renderer _morphLeftRenderer;
        private Renderer _morphRightRenderer;


        // [SerializeField] public Slider scaleBar;

        [SerializeField] public Slider distanceBar;

        [SerializeField] public Slider deformBar;

        [SerializeField] public Slider horizontalBar;

        [SerializeField] public Slider verticalBar;

        // TODO: enable
        // [SerializeField] public Slider depthBar; // affect distance between left/right eyes
        // [SerializeField] public Slider focusBar; // affect viewing angle of left/right eyes

        // TODO: group them
        public GameObject cone;

        private bool _flipStereo = false;

        public Material m_lMaterial;
        public Material m_rMaterial;
        public Material m_monoMaterial;
        public Material m_leftEyeTBMaterial;
        public Material m_rightEyeTBMaterial;

        private float Yaw;
        private float Pitch;
        private float Roll;


        public TextureView Texture;

        //Unity Awake, OnDestroy, and Update functions

        #region unity
        public void BindUI()
        {
            deformBar.onValueChanged.Rebind(OnDeformBarUpdated);
            distanceBar.onValueChanged.Rebind(OnDistanceSliderUpdated);
            horizontalBar.onValueChanged.Rebind(OnHorizontalSliderUpdated);
            verticalBar.onValueChanged.Rebind(OnVerticalSliderUpdated);
        }

        protected new void Awake()
        {
            base.Awake();

            if (deformBar is not null) deformBar.value = 0.0f;

            _morphLeftRenderer = leftEyeScreen.GetComponent<Renderer>();
            _morphRightRenderer = rightEyeScreen.GetComponent<Renderer>();

            SetVideoModeMono();
        }

        private void Update()
        {
            // TODO: can F1 be generalised?

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

        private void OnApplicationQuit()
        {
            OnDestroy();
        }

        private void OnDestroy()
        {
            //Dispose of mediaPlayer, or it will stay in nemory and keep playing audio
            Feed.Dispose();
        }
        #endregion

        private static Vector2 SCALE_RANGE = new Vector2(1f, 4.702173720867682f);

        public void OnDeformBarUpdated(float value)
        {
            // if (deformBar is null) return;

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


        public void OnDistanceSliderUpdated(float value)
        {
            gameObject.transform.localPosition = new Vector3(
                gameObject.transform.localPosition.x,
                gameObject.transform.localPosition.y,
                value
            );
        }

        /* Horizontal (X) axis offset for screen */
        public void OnHorizontalSliderUpdated(float value)
        {
            gameObject.transform.localPosition = new Vector3(
                value,
                gameObject.transform.localPosition.y,
                gameObject.transform.localPosition.z
            );
        }

        /* Vertical (Y) axis offset for screen */
        public void OnVerticalSliderUpdated(float value)
        {
            gameObject.transform.localPosition = new Vector3(
                gameObject.transform.localPosition.x,
                value,
                gameObject.transform.localPosition.z
            );
        }

        private float _sphereScale;

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

            Log("Stop");

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
        public void ToggleFlipStereo()
        {
            _flipStereo = !_flipStereo;
            SetVideoMode(videoMode);
        }


        public virtual Frac AspectRatio
        {
            get { return Feed.NativeAspectRatio(); } // by default, has no setter
            set { }
            // may not have a setter for some feeds
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
            if (_morphLeftRenderer.material is not null)
            {
                _morphLeftRenderer.material.mainTexture = null;
                _morphLeftRenderer.material = null;
            }

            if (_morphRightRenderer.material is not null)
            {
                _morphRightRenderer.material.mainTexture = null;
                _morphRightRenderer.material = null;
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

                _morphLeftRenderer.material = m_monoMaterial; // m_lMaterial;
                _morphLeftRenderer.material.mainTexture = Texture.Effective;
            }
            else
            {
                // 3D

                leftEyeScreen.layer = LayerMask.NameToLayer("LeftEyeOnly");

                rightEyeScreen.SetActive(true);
                rightEyeScreen.layer = LayerMask.NameToLayer("RightEyeOnly");

                if (mode is VideoMode._3D_OU)
                {
                    _morphLeftRenderer.material = _flipStereo ? m_rightEyeTBMaterial : m_leftEyeTBMaterial;
                    _morphRightRenderer.material = _flipStereo ? m_leftEyeTBMaterial : m_rightEyeTBMaterial;
                }
                else
                {
                    _morphLeftRenderer.material = _flipStereo ? m_rMaterial : m_lMaterial;
                    _morphRightRenderer.material = _flipStereo ? m_lMaterial : m_rMaterial;
                }

                _morphLeftRenderer.material.mainTexture = Texture.Effective;
                _morphRightRenderer.material.mainTexture = Texture.Effective;
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
        #endregion
    }
}
