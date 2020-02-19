using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public static Player_Controller instance;

    //References
    public Mist mist;


    [Header("KeyInputs")]
    //all of the key inputs
    public KeyCode forward;
    public KeyCode back;
    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;

    [Header("Mod Values")]
    //movement speed
    public float speed = 6.0f;

    //the ammount of force applied when the player jumps
    public float jumpForce;

    //the gravity applied to the player
    public float verticalVelocity;
    public float jumpHoldGravity;
    public float jumpGravity;
    public float fallGravity;
    public float fallVelocityCap;

    //the direction the player is moving in
    public Vector3 moveDirection = Vector3.zero;
    Vector3 wallMoveDirection;

    [Header("Look")]
    //mouse sensitivity
    public float sensitivityX;
    public float sensitivityY;
    private float currentX, currentY;
    public Transform verticalLook;

    [Header("Raycasts")]
    public float downRayDistance;
    public float wallRayDistance;

    //access player components
    public Rigidbody rb;

    //State Booleans
    public bool onGround = false;
    public bool onWall = false;
    public bool onWallJump = false;

    //Wall Running
    private Vector3 wallNormal;
    private Vector3 wallJumpDir;
    private Vector3 wallRayDirection;
    private GameObject currentWall;

    void Start()
    {
        instance = this;

        //access player components
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!onWall)
        {
            wallRayDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        }
        else
        {
            wallRayDirection =  -wallNormal;
            CheckForWall();
        }
        Ray wallRay = new Ray(transform.position, wallRayDirection);
        Debug.DrawRay(wallRay.origin, wallRayDirection, Color.red);


        //Powers
        if (SwingController.instance.state != SwingController.State.Swinging)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && !mist.isMist)
            {
                StartCoroutine(mist.BecomeMist());       
            }
        }
        if (SwingController.instance.state == SwingController.State.Walking)
        {
            CheckInput();
        }
        Look();
    }
    void FixedUpdate()
    {
     
        if (SwingController.instance.state == SwingController.State.Walking)
        {
            Movement();
        }
        Rotation();
        if (verticalVelocity <= 0)
        {
            CheckForGround();
        }
       // CheckForWall();
    }

   
    void CheckInput()
    {
        if (onWallJump)
        {
            moveDirection = wallJumpDir;
        }
        else
        {
            moveDirection = Vector3.zero;
        }
        if (!onWall)
        {   
                if (Input.GetKey(forward))
                {
                    moveDirection += transform.rotation * Vector3.forward;
                }
                if (Input.GetKey(back))
                {
                    moveDirection += transform.rotation * Vector3.back;
                }
                if (Input.GetKey(left))
                {
                    moveDirection += transform.rotation * Vector3.left;
                }
                if (Input.GetKey(right))
                {
                    moveDirection += transform.rotation * Vector3.right;
                }

            moveDirection.Normalize();
            moveDirection = moveDirection * speed;
        }
        if(onWall)
        {
            moveDirection += wallMoveDirection;
        }

        if (Input.GetKeyDown(jump))
        {
            if (onGround)
            {
                verticalVelocity = jumpForce;
                onGround = false;
            }
            if (onWall)
            {
                verticalVelocity = jumpForce;
                moveDirection = wallJumpDir;
                onWall = false;
                onGround = false;
                onWallJump = true;
            }
        }
    }
    
    void Movement()
    {        
        ApplyGravity();

        rb.velocity = moveDirection * speed * Time.deltaTime;
        //rb.AddForce(moveDirection * Time.deltaTime * speed, ForceMode.Impulse);
    }

    void Rotation()
    {
            //rotate the player based on look direction
            verticalLook.localRotation = Quaternion.Euler(-currentY, 0, 0);
            transform.rotation = Quaternion.Euler(0, currentX, 0);
    }

    void Look()
    {
        currentX += Input.GetAxis("Mouse X") * sensitivityX;
        currentY += Input.GetAxis("Mouse Y") * sensitivityY;

        if (currentY > 90f)
        {
            currentY = 90f;
        }
        else if (currentY < -90f)
        {
            currentY = -90f;
        } 
    }

    void ApplyGravity()
    {
        moveDirection.y = verticalVelocity;
        if (!onGround && !onWall)
        {
            if (rb.velocity.y < 0)
            {
                verticalVelocity -= fallGravity;
                onWallJump = false;
            }
            else if (Input.GetKey(jump))
            {
                verticalVelocity -= jumpHoldGravity;
            }
            else
            {
                verticalVelocity -= jumpGravity;
            }
        }
        if(verticalVelocity <= -fallVelocityCap)
        {
            verticalVelocity = -fallVelocityCap;
        }
    }

    void CheckForGround()
    {
        Ray downRay = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(downRay.origin, new Vector3(0, -downRayDistance, 0), Color.red);

        RaycastHit hit;

        if (Physics.Raycast(downRay.origin, downRay.direction, out hit, downRayDistance))
        {
            onGround = true;
            verticalVelocity = 0;
        }
        else
        {
            onGround = false;
        }
    }

    void CheckForWall()
    {
        Ray wallRay = new Ray(transform.position, wallRayDirection);
        Debug.DrawRay(wallRay.origin, new Vector3(moveDirection.x, 0, moveDirection.z), Color.red);

        RaycastHit hit;

        if (Physics.Raycast(wallRay.origin, wallRay.direction, out hit, wallRayDistance) && (hit.normal.y > -.3f || hit.normal.y < .3f))
        {
            wallNormal = hit.normal;
            Debug.Log("Wall Normal: " + wallNormal.x + " " + wallNormal.y + " " + wallNormal.z);
            Debug.Log("Move Direction: " + moveDirection);
            CalculateWallRunDirection();
            wallJumpDir = wallNormal;

            onWall = true;
            verticalVelocity = 0;
        }
        else
        {
            onWall = false;
        }
    }

    void CalculateWallRunDirection()
    {
        moveDirection.Normalize();
        Debug.Log("Normalized Move Direction: " + moveDirection);
        float perpAngle;
        if(wallNormal.z == 0)
        {
            if(moveDirection.z < 0)
            {
                wallMoveDirection = new Vector3(0, 0, -1) * speed;
            }
            if (moveDirection.z > 0)
            {
                wallMoveDirection = new Vector3(0, 0, 1) * speed;
            }
        }
        if (wallNormal.x == 0)
        {
            if (moveDirection.x < 0)
            {
                wallMoveDirection = new Vector3(-1, 0, 0) * speed;
            }
            if (moveDirection.x > 0)
            {
                wallMoveDirection = new Vector3(1, 0, 0) * speed;
            }
        }
        if ((wallNormal.z > 0 && wallNormal.x > 0)) //the wall normal is in quadrant 1
        {
            if(Mathf.Abs(wallNormal.z) > Mathf.Abs(wallNormal.x)) //Subquadrant 1
            {
                if (moveDirection.x < 0 && Mathf.Abs(moveDirection.z) < Mathf.Abs(wallNormal.z)) //player must be sent left of the normal
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(-invertX, 0, wallNormal.z).normalized * speed;
                }
                else if (Mathf.Abs(moveDirection.z) > Mathf.Abs(wallNormal.z) || (moveDirection.x > 0 && Mathf.Abs(moveDirection.z) < Mathf.Abs(wallNormal.z)))
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(invertX, 0, -wallNormal.z).normalized * speed;
                }
            }
            if (Mathf.Abs(wallNormal.z) < Mathf.Abs(wallNormal.x)) //Subquadrant 2
            {
                if (Mathf.Abs(moveDirection.z) < Mathf.Abs(wallNormal.z) || (Mathf.Abs(moveDirection.z) > Mathf.Abs(wallNormal.z) && moveDirection.z > 0)) //player must be sent left of the normal
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(-invertX, 0, wallNormal.z).normalized * speed;
                }
                else if (Mathf.Abs(moveDirection.z) > Mathf.Abs(wallNormal.z) && moveDirection.z < 0)
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(invertX, 0, -wallNormal.z).normalized * speed;
                }
            }
        }
        if ((wallNormal.z < 0 && wallNormal.x < 0)) //the wall normal is in quadrant 3
        {
            if (Mathf.Abs(wallNormal.z) > Mathf.Abs(wallNormal.x)) //Subquadrant 1
            {
                if (moveDirection.x > 0 && Mathf.Abs(moveDirection.z) < Mathf.Abs(wallNormal.z)) //player must be sent left of the normal
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(-invertX, 0, wallNormal.z).normalized * speed;
                }
                else if (Mathf.Abs(moveDirection.z) > Mathf.Abs(wallNormal.z) || (moveDirection.x < 0 && Mathf.Abs(moveDirection.z) < Mathf.Abs(wallNormal.z)))
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(invertX, 0, -wallNormal.z).normalized * speed;
                }
            }
            if (Mathf.Abs(wallNormal.z) < Mathf.Abs(wallNormal.x)) //Subquadrant 2
            {
                if (Mathf.Abs(moveDirection.z) < Mathf.Abs(wallNormal.z) || (Mathf.Abs(moveDirection.z) > Mathf.Abs(wallNormal.z) && moveDirection.z < 0)) //player must be sent left of the normal
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(-invertX, 0, wallNormal.z).normalized * speed;
                }
                else if (Mathf.Abs(moveDirection.z) > Mathf.Abs(wallNormal.z) && moveDirection.z > 0)
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(invertX, 0, -wallNormal.z).normalized * speed;
                }
            }
        }
        if ((wallNormal.z > 0 && wallNormal.x < 0)) //the wall normal is in quadrant 2
        {
            if (Mathf.Abs(wallNormal.z) > Mathf.Abs(wallNormal.x)) //Subquadrant 1
            {
                if (moveDirection.x > 0 && Mathf.Abs(moveDirection.z) < Mathf.Abs(wallNormal.z)) //player must be sent left of the normal
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(-invertX, 0, wallNormal.z).normalized * speed;
                }
                else if (Mathf.Abs(moveDirection.z) > Mathf.Abs(wallNormal.z) || (moveDirection.x < 0 && Mathf.Abs(moveDirection.z) < Mathf.Abs(wallNormal.z)))
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(invertX, 0, -wallNormal.z).normalized * speed;
                }
            }
            if (Mathf.Abs(wallNormal.z) < Mathf.Abs(wallNormal.x)) //Subquadrant 2
            {
                if (Mathf.Abs(moveDirection.z) < Mathf.Abs(wallNormal.z) || (Mathf.Abs(moveDirection.z) > Mathf.Abs(wallNormal.z) && moveDirection.z < 0)) //player must be sent left of the normal
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(-invertX, 0, wallNormal.z).normalized * speed;
                }
                else if (Mathf.Abs(moveDirection.z) > Mathf.Abs(wallNormal.z) && moveDirection.z > 0)
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(invertX, 0, -wallNormal.z).normalized * speed;
                }
            }
        }
        if ((wallNormal.z < 0 && wallNormal.x > 0)) //the wall normal is in quadrant 4
        {
            if (Mathf.Abs(wallNormal.z) > Mathf.Abs(wallNormal.x)) //Subquadrant 1
            {
                if (moveDirection.x < 0 && Mathf.Abs(moveDirection.z) < Mathf.Abs(wallNormal.z)) //player must be sent left of the normal
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(-invertX, 0, wallNormal.z).normalized * speed;
                }
                else if (Mathf.Abs(moveDirection.z) > Mathf.Abs(wallNormal.z) || (moveDirection.x > 0 && Mathf.Abs(moveDirection.z) < Mathf.Abs(wallNormal.z)))
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(invertX, 0, -wallNormal.z).normalized * speed;
                }
            }
            if (Mathf.Abs(wallNormal.z) < Mathf.Abs(wallNormal.x)) //Subquadrant 2
            {
                if (Mathf.Abs(moveDirection.z) < Mathf.Abs(wallNormal.z) || (Mathf.Abs(moveDirection.z) > Mathf.Abs(wallNormal.z) && moveDirection.z > 0)) //player must be sent left of the normal
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(-invertX, 0, wallNormal.z).normalized * speed;
                }
                else if (Mathf.Abs(moveDirection.z) > Mathf.Abs(wallNormal.z) && moveDirection.z < 0)
                {
                    perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                    float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                    wallMoveDirection = new Vector3(invertX, 0, -wallNormal.z).normalized * speed;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!onWall)
        {
            currentWall = collision.gameObject;
            CheckForWall();
        }
    }
}
