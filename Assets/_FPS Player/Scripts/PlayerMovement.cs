﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    public float walkSpeed = 4.0f;
    public float runSpeed = 8.0f;
    public float slideSpeed = 10.0f;
    public float crouchSpeed = 2f;
    [SerializeField]
    private float jumpSpeed = 8.0f;
    [SerializeField]
    private float gravity = 14.0f;
    [SerializeField]
    private float antiBumpFactor = .75f;
   // [HideInInspector]
    public Vector3 moveDirection = Vector3.zero;
    [HideInInspector]
    public Vector3 contactPoint;
    [HideInInspector]
    public CharacterController controller;
    [HideInInspector]
    public bool playerControl = false;

    public bool grounded = false;
    public Vector3 jump = Vector3.zero;

    private RaycastHit hit;
    private Vector3 force;
    private bool forceGravity;
    private float forceTime = 0;

    private void Awake()
    {
        instance = this;
        // Saving component references to improve performance.
        controller = GetComponent<CharacterController>();
    }
    
    private void Update()
    {
       // Debug.Log(moveDirection);
        if (forceTime > 0)
            forceTime -= Time.deltaTime;

    }

    private void FixedUpdate()
    {
        if(forceTime > 0)
        {
            if(forceGravity)
                moveDirection.y -= gravity * Time.deltaTime;
            grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        }
    }

    public void LateFixedUpdate()
    {

    }

    public void Move(Vector2 input, bool sprint, bool crouching)
    {
        if(forceTime > 0)
            return;

        float speed = (!sprint) ? walkSpeed : runSpeed;
        if (crouching) speed = crouchSpeed;

        if (grounded)
        {
            moveDirection = transform.rotation * new Vector3(input.x, -antiBumpFactor, input.y) * speed;
        }
        else
        {
            Vector3 inputMod = new Vector3(input.x, 0, input.y);
            inputMod = transform.rotation * inputMod;
            moveDirection.x += inputMod.x / 4;
            moveDirection.z += inputMod.z / 4;
            Vector2 normalizedDir = new Vector2(moveDirection.x, moveDirection.z);
            normalizedDir.Normalize();
            moveDirection.x = normalizedDir.x * speed;
            moveDirection.z = normalizedDir.y * speed;
        }
        UpdateJump();
        
        
        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;
        // Move the controller, and set grounded true or false depending on whether we're standing on something
        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }

    public void Move(Vector3 direction, float speed, float appliedGravity)
    {
        if (forceTime > 0)
            return;

        Vector3 move = direction * speed;
        if (appliedGravity > 0)
        {
            moveDirection.x = move.x;
            moveDirection.y -= gravity * Time.deltaTime * appliedGravity;
            moveDirection.z = move.z;
        }
        else
            moveDirection = move;

        UpdateJump();

        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }

    public void Move(Vector2 input, float appliedGravity)
    {
        if (forceTime > 0)
            return;

        if (appliedGravity > 0)
        {
            moveDirection.y -= gravity * Time.deltaTime * appliedGravity;

            Vector3 inputMod = new Vector3(input.x, 0, input.y);
            inputMod = transform.rotation * inputMod;
            moveDirection.x += inputMod.x / 4;
            moveDirection.z += inputMod.z / 4;
        }

        UpdateJump();

        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }

    public void Jump(Vector3 dir, float mult)
    {
        jump = dir * mult;
    }

    public void UpdateJump()
    {
        if (jump != Vector3.zero)
        {
            Vector3 dir = (jump * jumpSpeed);
            if (dir.x != 0) moveDirection.x = dir.x;
            if (dir.y != 0) moveDirection.y = dir.y;
            if (dir.z != 0) moveDirection.z = dir.z;
        }
        jump = Vector3.zero;
    }

    public void ForceMove(Vector3 direction, float speed, float time, bool applyGravity)
    {
        forceTime = time;
        forceGravity = applyGravity;
        moveDirection = direction * speed;
    }
    void CheckForGround()
    {
        Ray downRay = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(downRay.origin, new Vector3(0, -1.25f, 0), Color.red);

        RaycastHit hit;

        if (Physics.Raycast(downRay.origin, downRay.direction, out hit, 1.25f))
        {
            grounded = true;
        }
    }
        void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contactPoint = hit.point;
    }
}
