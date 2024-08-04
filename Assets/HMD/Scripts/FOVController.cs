using System.Collections.Generic;
using HMD.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

public class FOVController : MonoBehaviourWithLogging
{
        
    [SerializeField] private Camera leftCamera;
    [SerializeField] private Camera centerCamera;
    [SerializeField] private Camera rightCamera;

    [SerializeField] private Camera airPoseCamera;
        
    [SerializeField] public Slider fovBar;
    
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

    private float leftCameraMinX = -1.5f;
    private float rightCameraMaxX = 1.5f;

    private static float maxFocal = 15.0f;
    private static float minFocal = -15.0f;

    
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
        
        
    private float FOV // 20 for 2D 140 for spherical
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
        
        
    #region unity
    protected void Awake()
    {
        base.Awake();

        if (fovBar is not null) fovBar.value = FOV;
            
        // init
        OnFOVSliderUpdated();
    }
    #endregion
        
        
    public void OnFOVSliderUpdated()
    {
        // NOTE: NRSDK doesn't support custom FOV on cameras
        // NOTE: TESTING COMMENTING OUT camera.projectionMatrix = statements in NRHMDPoseTracker

        FOV = fovBar.value;
        Debug.Log("fov " + FOV);
    }
    
    
    public void OnBrightnessSliderUpdated()
    {
    }

    public void OnGammaSliderUpdated()
    {
    }

    public void OnContrastSliderUpdated()
    {
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
}
