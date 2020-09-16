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
   public bool armed;
    bool currentlyShooting;

    public static float lightMod = 1;
    public float distMod;
    public bool objectHeard;
    public Vector3 heardPos;

    public Image detection;

    //UI Follow Round Screen Variables
    // No scaling is done for distance yet
    public GameObject IndicatorAnchor;
    private Canvas CanvasIndicators;
    private GameObject text;
    private Text myText;

    public static bool inCombat;


    private void Awake()
    {
        enemySight = GetComponent<EnemySight>();
        nav = GetComponent<NavMeshAgent>();
        aINav = GetComponent<AINav>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastPlayerSighting = GameObject.FindGameObjectWithTag("GameController").GetComponent<LastPlayerSighting>();



    }
    private void Start()
    {
        CanvasIndicators = GameObject.Find("Indicators").GetComponent<Canvas>();

        //Change to ALERT Image
        text = new GameObject("myText");
        text.transform.SetParent(CanvasIndicators.transform);

        myText = text.AddComponent<Text>();
        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        myText.font = ArialFont;
        myText.material = ArialFont.material;
        myText.text = "AI";
        myText.alignment = TextAnchor.MiddleCenter;
        myText.fontSize = 20;
        myText.color = Color.red;
        myText.transform.localPosition = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //ALERT Imagine Move Round bounds of screen
        Vector3 screenPos = Camera.main.WorldToScreenPoint(IndicatorAnchor.transform.position);
        float distance = screenPos.z;

        Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

        //How far in the UI goes (.1f = 10% in)
        float margin = 0.1f;
        Vector3 screenBounds = screenCenter * (1 - margin);

        if (screenPos.z < 0)
            screenPos *= -1;

        screenPos -= screenCenter;

        if (screenPos.z < 0 || screenPos.x > screenBounds.x || screenPos.x < -screenBounds.x || screenPos.y > screenBounds.y || screenPos.y < -screenBounds.y)
        {
            // Off screen
            float angle = Mathf.Atan2(screenPos.y, screenPos.x);
            angle -= 90 * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = -Mathf.Sin(angle);

            //y = mx+b where b = 0
            float m = cos / sin;

            if (screenPos.y > screenBounds.y)
                screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
            if (screenPos.y < -screenBounds.y)
                screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);

            if (screenPos.x > screenBounds.x)
                screenPos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
            if (screenPos.x < -screenBounds.x)
                screenPos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
        }

        screenPos += screenCenter;
        myText.transform.position = screenPos;


        //Detection and Behavior
        detection.fillAmount = alertMeter;
        if (!aINav.ragDolled)
        {
            if (alertMeter >= alertMax && !armed)
            {
                aINav.animator.SetBool("startled", true);

            }
            else if (!enemySight.playerInSight && objectHeard && alertMeter < alertMax)
            {
                Distracted();
            }
            else if (enemySight.playerInSight && alertMeter < alertMax)
            {
                if (!alerting)
                    StartCoroutine(Alerting());
            }
            else if (enemySight.playerInSight || currentlyShooting)
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


    public void StartCombatMode()
    {
        Debug.Log("hooray");
        inCombat = true;
        armed = true;
        EnemySight.fieldOfViewAngle = 160;
        enemySight.col.radius = 25;
    }

}
