#if UNITY_EDITOR
#if UNITY_XR_MANAGEMENT_EDITOR
using UnityEditor.XR.Management;
#endif

#else
using UnityEngine.XR.Management;
#endif

namespace Unity.Template.VR
{
    using UnityEngine;

    internal class XRPlatformControllerSetup : MonoBehaviour
    {
        [SerializeField] private GameObject m_LeftController;

        [SerializeField] private GameObject m_RightController;

        [SerializeField] private GameObject m_LeftControllerOculusPackage;

        [SerializeField] private GameObject m_RightControllerOculusPackage;

        private void Start()
        {
#if UNITY_EDITOR && UNITY_XR_MANAGEMENT_EDITOR
            var loaders =
 XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone).Manager.activeLoaders;
#elif UNITY_EDITOR
            // Fallback when XR Management Editor is not available
            m_RightController.SetActive(true);
            m_LeftController.SetActive(true);
            m_RightControllerOculusPackage.SetActive(false);
            m_LeftControllerOculusPackage.SetActive(false);
            return;
#else
            var loaders = XRGeneralSettings.Instance.Manager.activeLoaders;
#endif

#if !UNITY_EDITOR || UNITY_XR_MANAGEMENT_EDITOR
            foreach (var loader in loaders)
            {
                if (loader.name.Equals("Oculus Loader"))
                {
                    m_RightController.SetActive(false);
                    m_LeftController.SetActive(false);
                    m_RightControllerOculusPackage.SetActive(true);
                    m_LeftControllerOculusPackage.SetActive(true);
                    return;
                }
            }
#endif

            m_RightController.SetActive(true);
            m_LeftController.SetActive(true);
            m_RightControllerOculusPackage.SetActive(false);
            m_LeftControllerOculusPackage.SetActive(false);
        }
    }
}