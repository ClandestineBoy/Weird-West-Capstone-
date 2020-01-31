﻿using UnityEngine.SceneManagement;
using UnityEngine;

public class SwingController : MonoBehaviour
{
    public static SwingController instance;

    public float speed = 6.0F;
    public float jumpSpeed = 20.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    CharacterController controller;
    public Camera cam;
    public enum State { Swinging, Falling, Walking };
    public State state;
    public Pendulum pendulum;
    Vector3 previousPosition;
    float distToGround;
    Vector3 hitPos;


    void Start()
    {
        instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        state = State.Walking;
        pendulum.bobTransform.transform.parent = pendulum.tether.tetherTransform;
        previousPosition = transform.localPosition;

        distToGround = GetComponent<CapsuleCollider>().bounds.extents.y;
    }

    void Update()
    {

        DetermineState();

        switch (state)
        {
            case State.Swinging:
                DoSwingAction();
                break;
            case State.Falling:
                DoFallingAction();
                break;
            //case State.Walking:
              //  DoWalkingAction();
                //break;
        }
        previousPosition = transform.localPosition;
    }

    bool IsGrounded()
    {
        print("Grounded");
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void DetermineState()
    {
        // Determine State
        if (Player_Controller.instance.onGround)
        {
            state = State.Walking;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.distance > 5)
                {
                    if (state == State.Walking)
                    {
                        pendulum.bob.velocity +=  new Vector3(0,Player_Controller.instance.verticalVelocity/2,0);
                    }
                    Player_Controller.instance.onGround = false;
                    Player_Controller.instance.rb.velocity = Vector3.zero;
                    Player_Controller.instance.verticalVelocity = 0;
                    pendulum.SwitchTether(hit.point);
                    state = State.Swinging;
                }

            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (state == State.Swinging)
            {
                state = State.Falling;
            }
        }
    }

    void DoSwingAction()
    {
        if (Input.GetKey(KeyCode.W))
        {
            pendulum.bob.velocity += pendulum.bob.velocity.normalized * .1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            pendulum.bob.velocity += -cam.transform.right * .1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            pendulum.bob.velocity += cam.transform.right * .1f;
        }
        transform.localPosition = pendulum.MoveBob(transform.localPosition, previousPosition, Time.deltaTime);
        previousPosition = transform.localPosition;
    }

    void DoFallingAction()
    {
        pendulum.arm.length = Mathf.Infinity;
        transform.localPosition = pendulum.Fall(transform.localPosition, Time.deltaTime);
        previousPosition = transform.localPosition;
    }

 /*   void DoWalkingAction()
    {
        pendulum.bob.velocity = Vector3.zero;
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = Camera.main.transform.TransformDirection(moveDirection);
            moveDirection.y = 0.0f;
            moveDirection *= speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }*/

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.name == "Respawn")
        {
            //if too far from arena, reset level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        Vector3 undesiredMotion = collision.contacts[0].normal * Vector3.Dot(pendulum.bob.velocity, collision.contacts[0].normal);
        pendulum.bob.velocity = -pendulum.bob.velocity/2;
        hitPos = transform.position;

        if (collision.gameObject.name == "Respawn")
        {
            //if too far from arena, reset level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
