using MAVLinkAPI.Util.NullSafety;
using UnityEngine;

namespace HMD.Scripts
{
    [Required]
    public class NullExample1 : MonoBehaviour
    {
        [SerializeField] private GameObject field1;

        public GameObject field2;
    }
}