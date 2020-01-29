using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
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
    private Vector3 moveDirection = Vector3.zero;

    [Header("Look")]
    //mouse sensitivity
    public float sensitivityX;
    public float sensitivityY;
    private float currentX, currentY;
    public Transform verticalLook;

    [Header("Raycasts")]
    public float downRayDistance;

    //access player components
    Rigidbody rb;

    //State Booleans
    public bool onGround = false;

    void Start()
    {
        //access player components
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
        Look();
    }

    void Movement()
    {
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

        //Rotation with look direction
        moveDirection = transform.rotation * moveDirection;

        //Apply Gravity
        if (!onGround)
        {
           moveDirection.y =  verticalVelocity;
        }

        rb.velocity = moveDirection * speed * Time.deltaTime;

        Jump();
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
        verticalLook.localRotation = Quaternion.Euler(-currentY, 0, 0);
        transform.rotation = Quaternion.Euler(0, currentX, 0);
    }

    void Jump()
    {
        if (onGround)
        {
            if (Input.GetKeyDown(jump))
            {
                verticalVelocity = jumpForce;
                onGround = false;
            }
        }
        else
        {
            if (rb.velocity.y < 0)
            {
                CheckForGround();
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
        }

    }
}
