using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float patrolSpeed = 3f;
    public float runSpeed = 5;
    public float chaseWaitTime = 10;
    public float endChaseWaitTime = 10;
    public float patrolWaitTime = 1;
    public Transform[] patrolWaypoints;

    private EnemySight enemySight;
    private NavMeshAgent nav;
    private Transform player;
    private LastPlayerSighting lastPlayerSighting;
    private AINav aINav;
    private float chaseTimer;
    private float endChaseTimer;
    private float patrolTimer;
    public float alertMeter = 0;
    private float alertMax = 1;
    private int wayPointIndex;
    private bool alerting;


    private void Awake()
    {
        enemySight = GetComponent<EnemySight>();
        nav = GetComponent<NavMeshAgent>();
        aINav = GetComponent<AINav>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastPlayerSighting = GameObject.FindGameObjectWithTag("GameController").GetComponent<LastPlayerSighting>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!aINav.ragDolled)
        {
            if (enemySight.playerInSight && alertMeter < alertMax)
            {
                if (!alerting)
                    StartCoroutine(Alerting());
            }
            else if (enemySight.playerInSight)
            {
                Debug.Log("Shoot!");
                Shooting();
            }
            else if (chaseTimer > chaseWaitTime && alertMeter >= alertMax)
            {
                StopChasing();
            }
            else if (enemySight.personalLastSighting != lastPlayerSighting.resetPosition && alertMeter >= alertMax)
            {
                Chasing();
            }
            else
            {
                Patrolling();
            }
        }
    }
   
    IEnumerator Alerting()
    {
        alerting = true;
        while (alertMeter < 1)
        {
            alertMeter += Time.deltaTime/5;
            yield return 0;
            if (!enemySight.playerInSight)
                break;
            
        }
        if (alertMeter < alertMax)
            StartCoroutine(StopAlerting());
        else
        {
            alertMeter = 1;
            alerting = false;
        }

    }

    IEnumerator StopAlerting()
    {
        alerting = false;
        yield return new WaitForSeconds(2);
        if (!enemySight.playerInSight && alertMeter < alertMax) { 
        while (alertMeter > 0)
            {
                alertMeter -= Time.deltaTime;
                yield return 0;
                if (enemySight.playerInSight)
                    break;
            }
        if  (alertMeter < 0)
            {
                alertMeter = 0;
            }
        }
           
    }
    
    
    void Shooting()
    {
        Debug.Log("Shooting");
        //may change this to spherical raycast
        nav.isStopped = true;
        //Reset vision timer if in sight
        chaseTimer = 0;
    }
    void Chasing()
    {
        Debug.Log("Chasing!");
        nav.isStopped = false;    
        nav.SetDestination(player.position);
        nav.speed = runSpeed;
        
        //if they chase too long, they lose track of you after 10 seconds and move to your last position
        //may need to increase sight range beyond shooting range
        chaseTimer += Time.deltaTime;

        lastPlayerSighting.position = player.position;
        //enemySight.personalLastSighting = lastPlayerSighting.position;
    }

    void StopChasing()
    {
        Debug.Log("Losing");
        nav.isStopped = false;
            nav.SetDestination(enemySight.personalLastSighting);
        nav.speed = runSpeed;
        if (nav.remainingDistance < .5f)
        {
            endChaseTimer += Time.deltaTime;
            if (endChaseTimer > endChaseWaitTime)
            {
                lastPlayerSighting.position = lastPlayerSighting.resetPosition;
                enemySight.personalLastSighting = lastPlayerSighting.resetPosition;
                endChaseTimer = 0;
                chaseTimer = 0;
            }
        }
        else
        {
            endChaseTimer = 0;
        }
    }
    void Patrolling()
    {
        Debug.Log("Patrolling");
        nav.isStopped = false;
        nav.speed = patrolSpeed;
        if (nav.destination == lastPlayerSighting.resetPosition || nav.remainingDistance < 1)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                if (wayPointIndex == patrolWaypoints.Length - 1)
                {
                    wayPointIndex = 0;
                }
                else
                    wayPointIndex++;

                patrolTimer = 0;
            }
        }
        else 
            patrolTimer = 0;

        nav.SetDestination(patrolWaypoints[wayPointIndex].position);
    }


    public void BrightLight()
    {
        enemySight.fieldOfViewAngle = 90;
    }
    public void DimLight()
    {
        enemySight.fieldOfViewAngle = 70;
    }
    public void AmbientLight()
    {
        enemySight.fieldOfViewAngle = 50;
    }


}
