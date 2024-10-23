using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform leader;

    public bool inverse = false;

    // Update is called once per frame
    private void Update()
    {

        if (!inverse)
        {
            transform.rotation = leader.rotation;
        }
        else
        {
            transform.rotation = Quaternion.Inverse(leader.rotation);
        }

    }
}
