using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public static Player_Controller instance;

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

    //the direction the player is moving in
    public Vector3 moveDirection = Vector3.zero;

    [Header("Look")]
    //mouse sensitivity
    public float sensitivityX;
    public float sensitivityY;
    private float currentX, currentY;
    public Transform verticalLook;

    [Header("Raycasts")]
    public float downRayDistance;

    //access player components
    public Rigidbody rb;

    //State Booleans
    public bool onGround = false;

    void Start()
    {
<<<<<<< HEAD
        Cursor.lockState = CursorLockMode.Locked;
=======
        instance = this;
>>>>>>> d4be3d4f0a69ff104b86678ae4cdcd4d61522b8e

        //access player components
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
<<<<<<< HEAD
        //Input functions should be used in Update
        CheckInput();
    }
    void FixedUpdate()
    {
        //Applying physics should occur in FixedUpdate
        Movement();
=======
        
    }
    void FixedUpdate()
    {
        if (SwingController.instance.state == SwingController.State.Walking)
        {
            Movement();
            CheckInput();
        }
        if (verticalVelocity <= 0)
        {
            CheckForGround();
        }
>>>>>>> d4be3d4f0a69ff104b86678ae4cdcd4d61522b8e
        Look();
    }

    void CheckInput()
    {
        //Movement Input
        moveDirection = Vector3.zero;
        if (Input.GetKey(forward))
        {
            moveDirection += Vector3.forward;
        }
        if (Input.GetKey(back))
        {
            moveDirection += Vector3.back;
        }
        if (Input.GetKey(left))
        {
            moveDirection += Vector3.left;
        }
        if (Input.GetKey(right))
        {
            moveDirection += Vector3.right;
        }
        moveDirection.Normalize();
        moveDirection *= speed;

        //Jump Input
        if (Input.GetKeyDown(jump))
        {
            verticalVelocity = jumpForce;
            onGround = false;
        }

        //Look Input
        currentX += Input.GetAxis("Mouse X") * sensitivityX;
        currentY += Input.GetAxis("Mouse Y") * sensitivityY;
    }
    void Movement()
    {
        //Rotation with look direction
        moveDirection = transform.rotation * moveDirection;

        //Apply Gravity
        if (!onGround)
        {
           moveDirection.y =  verticalVelocity;
        }

        rb.velocity = moveDirection * speed * Time.deltaTime;

       ApplyGravity();
    }

    void Look()
    {
        if (currentY > 90f)
        {
            currentY = 90f;
        }
        else if (currentY < -90f)
        {
            currentY = -90f;
        }
        verticalLook.localRotation = Quaternion.Euler(-currentY, 0, 0);
        transform.rotation = Quaternion.Euler(0, currentX, 0);
    }

    void ApplyGravity()
    {
        //if the player is not on the ground, gravity will begin to apply to their vertical velocity
        if (!onGround)
        {
            if (rb.velocity.y < 0)
            {
                verticalVelocity -= fallGravity;
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
<<<<<<< HEAD
        //if the player is falling, we check for when they hit the ground
        //if the player is already on the ground, we check to see if they walk off a ledge
        if(verticalVelocity <= 0)
        {
            CheckForGround();
        }
=======
        
>>>>>>> d4be3d4f0a69ff104b86678ae4cdcd4d61522b8e
    }

    void CheckForGround()
    {
        Ray downRay = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(downRay.origin, new Vector3(0, -downRayDistance, 0), Color.red);

        RaycastHit hit;

        if (Physics.Raycast(downRay.origin, downRay.direction, out hit, downRayDistance))
        {
            //Debug.Log(hit.transform.gameObject.name);
            //Debug.Log("ON GROUND");
            onGround = true;
            verticalVelocity = 0;
        }
        else
        {
            onGround = false;
        }

    }
}
