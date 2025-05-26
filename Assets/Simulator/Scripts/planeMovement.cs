using UnityEngine;

public class planeMovement : MonoBehaviour
{
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float velocity = 10f;
    [SerializeField] private float smoothTime;
    private Vector3 forward = new(0, (float)0.15, -1);
    private bool canMove = false;


    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (canMove) transform.Translate(forward * Time.deltaTime * velocity, Space.World);
    }

    public void setCanMove(bool a)
    {
        canMove = a;
    }
}