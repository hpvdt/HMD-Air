using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    public Transform camera;
    [SerializeField] private float speed = 6f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float playerRadius = .7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private bool noZ = false;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float jumpForce = 20;
    [SerializeField] private float gravityScale = 5f;

    float jumpVelocity;
    private bool isGrounded = true;
    //private bool jumpGo = false;
    bool gamePause = false;



    private void Update() {

        //float horizontalInput = Input.GetAxis("Horizontal");
        //float verticalInput = Input.GetAxis("Vertical");

        

    
           HandleMovement();
           HandelJump();
        
        
    }

    private void onCollision()
    {
        
    }

    private void HandleMovement()
    {   
        //get input vector
        Vector2 inputVector = gameInput.getMovementVectorNormalized();

        //sets movement vector to move with respect to camera
        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        if (noZ)
        {
            inputVector.y = 0;
        }

        Vector3 cameraRelativeMovement = right * inputVector.x + forward * inputVector.y;
        cameraRelativeMovement = cameraRelativeMovement.normalized;

        //check if player is not colliding with object
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, cameraRelativeMovement, Time.deltaTime * speed);

 

        if (canMove)
        {
            transform.Translate(cameraRelativeMovement * Time.deltaTime * speed, Space.World);
        }
        else if (!canMove)
        {
            // cannot move in current path

            // attempt only x movement
            Vector3 moveDirX = new Vector3(cameraRelativeMovement.x, 0, 0);
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, Time.deltaTime * speed);

            if (canMove)
            {
                // can only move on X
                cameraRelativeMovement = moveDirX;
                transform.Translate(cameraRelativeMovement * Time.deltaTime * speed, Space.World);
            }
            else if (!canMove)
            {
                // attempt only z movement

                Vector3 moveDirZ = new Vector3(0, 0, cameraRelativeMovement.z);
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, Time.deltaTime * speed);

                if (canMove)
                {
                    // can only move on z
                    cameraRelativeMovement = moveDirZ;
                    transform.Translate(cameraRelativeMovement * Time.deltaTime * speed, Space.World);
                }
            }
            // attempt only z movement
        }
    }

    private void HandelJump()
    {

        isGrounded = Physics.Raycast(GameObject.Find("Player").transform.position, Vector3.down, 1);

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            float velocity = jumpForce;
        }
        transform.Translate(new Vector3(0, jumpVelocity, 0) * Time.deltaTime);
    }
}

