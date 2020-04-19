﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemySight : MonoBehaviour
{
    public static float fieldOfViewAngle = 60f;
    public bool playerInSight;
    public Vector3 personalLastSighting;

    private NavMeshAgent nav;
    public SphereCollider col;
    private Animator anim;
    private LastPlayerSighting lastPlayerSighting;
    private GameObject player;
    private Animator playerAnim;
    //private PlayerHealth playerHealth;
    private HashIDs hash;
    private Vector3 previousSighting;


    public bool inRange;
    public float dist;
    
    // Start is called before the first frame update
    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        lastPlayerSighting = GameObject.FindGameObjectWithTag("GameController").GetComponent<LastPlayerSighting>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerAnim = GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag("GameController").GetComponent<HashIDs>();

        personalLastSighting = lastPlayerSighting.resetPosition;
        previousSighting = lastPlayerSighting.resetPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastPlayerSighting.position != previousSighting)
        {
            personalLastSighting = lastPlayerSighting.position;
        }
        anim.SetBool(hash.playerInSightBool, playerInSight);

        Vector3 endDir1 = Quaternion.Euler(0, fieldOfViewAngle/2, 0) * -transform.forward;
        Vector3 endDir2 = Quaternion.Euler(0, -fieldOfViewAngle/2, 0) * -transform.forward;
        Vector3 endPoint1 = transform.position - endDir1 * col.radius;
        Vector3 endPoint2 = transform.position - endDir2 * col.radius;

        Vector3 periphDir1 = Quaternion.Euler(0, 80, 0) * -transform.forward;
        Vector3 periphDir2 = Quaternion.Euler(0, -80, 0) * -transform.forward;
        Vector3 periphPoint1 = transform.position - periphDir1 * 3;
        Vector3 periphPoint2 = transform.position - periphDir2 * 3;
        //  Debug.Log(string.Format("Enemy location: {0} {1}, End location: {2} {3}", transform.position.x, transform.position.z, endPoint1.x, endPoint1.z));
        Debug.DrawLine(transform.position, endPoint1, Color.red);

        Debug.DrawLine(transform.position, endPoint2, Color.red);

        //Peripheral
        Debug.DrawLine(transform.position, periphPoint1, Color.red);
        Debug.DrawLine(transform.position, periphPoint2, Color.red);

        if (inRange)
        {
           
            Vector3 direction = player.transform.position - transform.position;
            dist = Vector3.Distance(player.transform.position, transform.position);
            float angle = Vector3.Angle(direction, transform.forward);

            if (angle < fieldOfViewAngle / 2)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, direction.normalized, out hit, col.radius))
                {
                    if (hit.collider.gameObject == player)
                    {
                        playerInSight = true;
                        lastPlayerSighting.position = player.transform.position;
                    }
                    else
                        playerInSight = false;
                }
                else playerInSight = false;
            } // peripheral vision
            else if (angle < 80 && dist < 3)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, direction.normalized, out hit, col.radius))
                {
                    if (hit.collider.gameObject == player)
                    {
                        playerInSight = true;
                        lastPlayerSighting.position = player.transform.position;
                    }
                    else
                        playerInSight = false;
                }

            } else 
                playerInSight = false;


            // if state sprinting/wallrunning etc
            if (PlayerController.instance.status == Status.moving)
            {
                if (CalculatePathLength(player.transform.position) <= col.radius)
                {
                    personalLastSighting = player.transform.position;
                    playerInSight = true;
                }
            }
        }

    }



   /* private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInSight = false;
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            if (angle < fieldOfViewAngle / 2)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, direction.normalized, out hit, col.radius))
                {
                    if (hit.collider.gameObject == player)
                    {
                        playerInSight = true;
                        lastPlayerSighting.position = player.transform.position;
                    }
                }
            }
            // if state sprinting/wallrunning etc
           // if (CalculatePathLength(player.transform.position) <= col.radius)
            //{
              //  personalLastSighting = player.transform.position;
            //}
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInSight = false;
        }
    }
    */


   public float CalculatePathLength(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();

        if (nav.enabled)
            nav.CalculatePath(targetPosition, path);

        Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];

        allWayPoints[0] = transform.position;
        allWayPoints[allWayPoints.Length - 1] = targetPosition;

        for (int i = 0; i < path.corners.Length; i++)
        {
            allWayPoints[i + 1] = path.corners[i];
        }
        float pathLength = 0;
        for (int i = 0; i <allWayPoints.Length-1; i++)
        {
            pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
        }
        return pathLength;
    }
}
