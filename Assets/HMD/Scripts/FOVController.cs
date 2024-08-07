﻿using System.Collections.Generic;
using HMD.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

public class FOVController : MonoBehaviourWithLogging
{
        
    [SerializeField] private Camera leftCamera;
    [SerializeField] private Camera centerCamera;
    [SerializeField] private Camera rightCamera;

    [SerializeField] private Camera airPoseCamera;
        
    [SerializeField] private Slider fovBar;
    
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
    protected void Start()
    {
        BindUI();
    }
    
    
    private void BindUI()
    {
        fovBar.onValueChanged.AddListener(OnFOVSliderUpdated);
        // brightnessBar.onValueChanged.AddListener(OnBrightnessSliderUpdated);
        // contrastBar.onValueChanged.AddListener(OnContrastSliderUpdated);
        // gammaBar.onValueChanged.AddListener(OnGammaSliderUpdated);
    }
    
    protected void Awake()
    {
        base.Awake();

        fovBar.value = FOV;
    }
    #endregion
    
    private void OnFOVSliderUpdated(float value)
    {
        // NOTE: NRSDK doesn't support custom FOV on cameras
        // NOTE: TESTING COMMENTING OUT camera.projectionMatrix = statements in NRHMDPoseTracker

        FOV = value;
        Log("fov " + FOV);
    }
    
    
    private void OnBrightnessSliderUpdated()
    {
    }

    private void OnGammaSliderUpdated()
    {
    }

    private void OnContrastSliderUpdated()
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