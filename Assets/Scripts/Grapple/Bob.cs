using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Bob 
{
    public float gravity = 20f;
    public Vector3 velocity;
    public Vector3 gravDir = new Vector3(0, 1, 0);
    Vector3 dampingDir;
    public float drag;
    public float maxSpeed;

    public void ApplyGravity()
    {
        velocity -= gravDir * gravity * Time.deltaTime;
    }

    public void ApplyDamping()
    {
        dampingDir = -velocity;
        dampingDir *= drag;
        velocity += dampingDir;
    }

    public void CapMaxSpeed()
    {
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
    }
}
