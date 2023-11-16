using UnityEngine;

public class BackfaceCullingToggle : MonoBehaviour
{
    public bool enableBackfaceCulling = true;

    void Update()
    {
        if (enableBackfaceCulling)
            Shader.DisableKeyword("_DISABLE_BACKFACE_CULL");
        else
            Shader.EnableKeyword("_DISABLE_BACKFACE_CULL");
    }
}