﻿using System.Collections;
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
    public float currentX, currentY;
    public Transform verticalLook;

    [Header("Raycasts")]
    public float downRayDistance;
    public float wallRayDistance;

    //access player components
    public Rigidbody rb;

    //State Booleans
    public bool onGround = false;
    public bool onWall = false;

    //Wall Running
    private Vector3 wallNormal;

    void Start()
    {
        instance = this;

        //access player components
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Debug.Log(verticalLook.localEulerAngles.x);

        Ray wallRay = new Ray(transform.position, new Vector3(moveDirection.x, 0, moveDirection.z));
        Debug.DrawRay(wallRay.origin, new Vector3(moveDirection.x, 0, moveDirection.z), Color.red);


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
        moveDirection = Vector3.zero;
        if (!onWall)
        {
            
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
            moveDirection = transform.rotation * moveDirection * speed;
        }
        else
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
                onWall = false;
                onGround = false;
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
        Ray wallRay = new Ray(transform.position, new Vector3(moveDirection.x, 0, moveDirection.z));
        Debug.DrawRay(wallRay.origin, new Vector3(moveDirection.x, 0, moveDirection.z), Color.red);

        RaycastHit hit;

        if (Physics.Raycast(wallRay.origin, wallRay.direction, out hit, wallRayDistance))
        {
            wallNormal = hit.normal;
            Debug.Log("Wall Normal: " + wallNormal.x + " " + wallNormal.y + " " + wallNormal.z);
            Debug.Log("Move Direction: " + moveDirection);
            CalculateWallRunDirection();
            onWall = true;
            verticalVelocity = 0;
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
        if ((wallNormal.z > 0 && wallNormal.x > 0) || (wallNormal.z < 0 && wallNormal.x < 0)) //the wall normal is in quadrant 1 or 3
        {
            if (Mathf.Abs(moveDirection.z) < Mathf.Abs(wallNormal.z)) //player must be sent left of the normal
            {
                perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                wallMoveDirection = new Vector3(-invertX, 0, wallNormal.z) * speed;
            }
            if (Mathf.Abs(moveDirection.z) > Mathf.Abs(wallNormal.z))
            {
                perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.x / wallNormal.z);
                float invertX = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.z;
                wallMoveDirection = new Vector3(invertX, 0, -wallNormal.z) * speed;
            }
        }
        if ((wallNormal.z > 0 && wallNormal.x < 0) || (wallNormal.z < 0 && wallNormal.x > 0)) //the wall normal is in quadrant 2 or 4
        {
            if (Mathf.Abs(moveDirection.z) > Mathf.Abs(wallNormal.z)) //player must be sent left of the normal
            {
                perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.z / wallNormal.x);
                float invertZ = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.x;
                wallMoveDirection = new Vector3(wallNormal.x, 0, -invertZ) * speed;
            }
            if (Mathf.Abs(moveDirection.z) < Mathf.Abs(wallNormal.z))
            {
                perpAngle = 90 - Mathf.Rad2Deg * Mathf.Atan(wallNormal.z / wallNormal.x);
                float invertZ = Mathf.Tan(perpAngle * Mathf.Deg2Rad) * wallNormal.x;
                wallMoveDirection = new Vector3(-wallNormal.x, 0, invertZ) * speed;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!onWall)
        CheckForWall();
    }
}
