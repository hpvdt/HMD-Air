using UnityEngine;

namespace MAVLinkAPI.Scripts.Util.Lifetime
{
    public class LifetimeController : MonoBehaviour
    {
        private Lifetime lifetime = new();

        // OnDestroy will dispose lifetime
        private void OnDestroy()
        {
            lifetime.Dispose();
        }
    }
}