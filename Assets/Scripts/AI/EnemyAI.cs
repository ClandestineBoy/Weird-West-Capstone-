using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


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

    public static float lightMod = 1;
    public float distMod;
    public bool objectHeard;
    public Vector3 heardPos;

    public Image detection;
    public GameObject myCanvas;

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
       /* Vector3 line1 = transform.position - player.position;
        float angle = Vector3.SignedAngle( new Vector3(line1.x, 0, line1.z), new Vector3(player.forward.x, 0 , player.forward.z), Vector3.up);
        Debug.Log(angle);
        */
/*
        // Get vector from player to AI
        Vector3 targetDir = transform.position - player.position;

        // Convert vector to polar coordinates (distance & angle) ignoring height
        float dist = Vector3.Distance(new Vector3(targetDir.x, 0, targetDir.z), new Vector3(player.forward.x, 0, player.forward.z));
        float angle = Vector3.SignedAngle(new Vector3(targetDir.x, 0, targetDir.z), new Vector3(player.forward.x, 0, player.forward.z), Vector3.up);

        // Limit angle to FOV
        angle = Mathf.Min(angle, Camera.main.fov / 2);

        // Create vector from player to desired indicator position
        Vector3 player2indicator = new Vector3(dist * Mathf.Cos(angle * Mathf.Deg2Rad), 0, dist * Mathf.Sin(angle * Mathf.Deg2Rad));

        // Valculate vector from AI to desired indicator position (relative vector from parent)
        Vector3 indicator = targetDir - player2indicator;

        // Set indicator height
        indicator.y = 2.0f;
        Debug.Log(indicator);

        // Now use indicator to set position relative to parent
        myCanvas.GetComponent<RectTransform>().localPosition = indicator;
        */



        detection.fillAmount = alertMeter;
        if (!aINav.ragDolled)
        {
            if (!enemySight.playerInSight && objectHeard && alertMeter < alertMax)
            {
                Distracted();
            }
            else if (enemySight.playerInSight && alertMeter < alertMax)
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


    void Distracted()
    {
        //Debug.Log("heard something!");
        //need to add a pause for the ai to look around first after being startled
        nav.isStopped = false;
        nav.SetDestination(heardPos);
        nav.speed = patrolSpeed;

        if (nav.remainingDistance < 2)
        {
            StartCoroutine(Inspect());
        }     
    }

    IEnumerator Inspect()
    {
        //run looking around animation
        yield return new WaitForSeconds(10);
        objectHeard = false;
    }


    IEnumerator Alerting()
    {
        Debug.Log("alerting");
        alerting = true;
        while (alertMeter < 1)
        {
            distMod = 10/enemySight.dist;
            alertMeter += Time.deltaTime*lightMod * distMod;
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
        Debug.Log("stopping");
        alerting = false;
        yield return new WaitForSeconds(2);
        if (!enemySight.playerInSight && alertMeter < alertMax) { 
        while (alertMeter > 0)
            {
                if (enemySight.playerInSight)
                    break;
                alertMeter -= Time.deltaTime*lightMod;
                yield return 0;
               // if (enemySight.playerInSight)
                 //   break;
            }
        if  (alertMeter < 0)
            {
                alertMeter = 0;
            }
        }
           
    }
    
    
    void Shooting()
    {
      //  Debug.Log("Shooting");
        //may change this to spherical raycast
        nav.isStopped = true;
        //Reset vision timer if in sight
        chaseTimer = 0;
    }
    void Chasing()
    {
        //Debug.Log("Chasing!");
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
        //Debug.Log("Losing");
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
            alertMeter = 0;
            endChaseTimer = 0;
        }
    }
    void Patrolling()
    {
       // Debug.Log("Patrolling");
        nav.isStopped = false;
        nav.speed = patrolSpeed;
        if (nav.destination == lastPlayerSighting.resetPosition || nav.remainingDistance < 1)
        {
            nav.isStopped = true;
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


    

}
