using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Pendulum
{
    public Transform bobTransform;
    public Tether tether;
    public Arm arm;
    public Bob bob;

    Vector3 previousPos;

    public void Initialize()
    {
        bobTransform.parent = tether.tetherTransform;
        arm.length = Vector3.Distance(bobTransform.localPosition, tether.tetherTransform.position);
    }
    public Vector3 MoveBob(Vector3 pos, float time)
    {
        bob.velocity += GetConstrainedVelocity(pos,previousPos,time);
        bob.ApplyGravity();
        bob.ApplyDamping();
        bob.CapMaxSpeed();

        pos += bob.velocity * time;

        if (Vector3.Distance(pos,tether.position) < arm.length)
        {
            pos = Vector3.Normalize(pos - tether.position) * arm.length;
            arm.length = (Vector3.Distance(pos, tether.position));
            return pos;
        }

        previousPos = pos;

        return pos;
    }
    public Vector3 MoveBob(Vector3 pos, Vector3 prevPos, float time)
    {
        bob.velocity += GetConstrainedVelocity(pos, prevPos, time);
        bob.ApplyGravity();
        bob.ApplyDamping();
        bob.CapMaxSpeed();

        pos += bob.velocity * time;

        if (Vector3.Distance(pos, tether.position) < arm.length)
        {
            pos = Vector3.Normalize(pos - tether.position) * arm.length;
            arm.length = (Vector3.Distance(pos, tether.position));
            return pos;
        }

        previousPos = pos;

        return pos;
    }

    public Vector3 GetConstrainedVelocity(Vector3 currentPos, Vector3 previousPos, float time)
    {
        float distanceToTether;
        Vector3 constrainedPosition;
        Vector3 predictedPosition;

        distanceToTether = Vector3.Distance(currentPos, tether.position);
        if (distanceToTether > arm.length) { 
            constrainedPosition = Vector3.Normalize(currentPos - tether.position) * arm.length;
            //velocity because distance/time
            predictedPosition = (constrainedPosition - previousPos) / time;
            return predictedPosition;
        }
        return Vector3.zero;
    }

    public void SwitchTether(Vector3 newPos)
    {
        bobTransform.parent = null;
        tether.tetherTransform.position = newPos;
        bobTransform.parent = tether.tetherTransform;
        tether.position = tether.tetherTransform.InverseTransformPoint(newPos);
        arm.length = Vector3.Distance(bobTransform.localPosition, tether.position);
    }

    public Vector3 Fall(Vector3 pos, float time)
    {
        bob.ApplyDamping();
        bob.ApplyGravity();
        bob.CapMaxSpeed();

        pos += bob.velocity *time;
        return pos;
    }

}
