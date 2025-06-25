using MAVLinkAPI.Util.NullSafety;
using UnityEngine;

namespace HMD.Scripts
{
    public class NullExample2 : MonoBehaviour
    {
        [SerializeField] [Required] private GameObject field1;

        [Required] public GameObject field2;
    }
}