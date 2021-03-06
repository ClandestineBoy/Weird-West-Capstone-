﻿using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
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
    public bool patrolling;
    public float alertMeter = 0;
    private float alertMax = 1;
    private int wayPointIndex;
    public bool alerting;
    public bool armed;

    public AudioClip[] AIaudio;
    public AudioSource AIaudioSource;
    public static float lightMod = 1;
    public float distMod;
    public bool objectHeard;
    public bool bodySeen;
    public Vector3 distractedPos;

    public Image detection;

    //UI Follow Round Screen Variables
    // No scaling is done for distance yet
    public GameObject IndicatorAnchor;
    public GameObject DetectionEye;
    public GameObject DetectedEye;
    private Canvas CanvasIndicators;
    private GameObject text;
    private Text myText;
    //private Sprite eye;
    //private Sprite eyeSpotted;

    public static bool inCombat;
    //ENemies who chase timer have not exceeded limit
    public static List<GameObject> enemiesInCombat = new List<GameObject>();

    public AnimationCurve linearCurve;

    //CombatStuff
    bool foundAttackPoint;
    bool currentlyAttacking;
    bool inAttackPattern;
    bool inAir;
    private Vector3 attackPoint;
    public Transform shootPoint;


    Vector3 rightAtRedirect;
    Vector3 leftAtRedirect;
    // 0 = shooting, 1 = melee, 2 = sniper, 3 = anti-geist
    public int attackType;
    public float enemyHealth = 100;
    public GameObject bullet;
    int layerMask = 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3 | 1 << 4 | 1 << 6 | 1 << 7 | 1 << 8 | 1 << 9 | 1 << 10 | 1 << 12 | 1 << 13 | 1 << 14 | 1 << 16 | 1 << 17 | 1 << 18;
    int AIMask = 1 << 11;

    public GameObject gun;
    public GameObject stowedGun;
    public GameObject melee;
    public GameObject stowedMelee;
    private int frames = 0;

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
        DetectionEye.transform.SetParent(CanvasIndicators.transform);
        DetectedEye.transform.SetParent(CanvasIndicators.transform);

        myText = text.AddComponent<Text>();
        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        myText.font = ArialFont;
        myText.material = ArialFont.material;
        myText.text = "";
        myText.alignment = TextAnchor.MiddleCenter;
        myText.fontSize = 20;
        myText.color = Color.red;
        myText.transform.localPosition = new Vector3(0, 0, 0);
        DetectionEye.transform.localPosition = new Vector3(0, 0, 0);
        DetectedEye.transform.localPosition = new Vector3(0, 0, 0);
        DetectedEye.transform.rotation = new Quaternion(0, 0, 0, 0);
        DetectionEye.transform.rotation = new Quaternion(0, 0, 0, 0);
        DetectedEye.transform.localScale = DetectedEye.transform.localScale * 100;
        DetectionEye.transform.localScale = DetectionEye.transform.localScale * 100;

        aINav.animator = GetComponent<Animator>();
        gun.SetActive(false);
        melee.SetActive(false);
        stowedGun.SetActive(false);
        stowedMelee.SetActive(false);
        if (attackType != 1) {
            aINav.animator.SetBool("gun", true);
            stowedGun.SetActive(true);
             }
        else { 
            aINav.animator.SetBool("melee", true);
            stowedMelee.SetActive(true);
             }

        //eye = (Sprite)Resources.Load("Art/UI/eye");
        //eyeSpotted = (Sprite)Resources.Load("Art/UI/eyeSpotted");
    }

    // Update is called once per frame
    void Update()
    {
        frames++;
        if (alertMeter >= alertMax && !aINav.ragDolled)
        {
            DetectedEye.SetActive(true);
            DetectionEye.SetActive(false);
            //myText.text = "Detected";
        }
        else if ((alertMeter > 0 || enemySight.playerInSight) && !aINav.ragDolled)
        {
            DetectionEye.SetActive(true);
            DetectedEye.SetActive(false);
           // myText.text = "Detecting";
        } else
        {
            DetectionEye.SetActive(false);
            DetectedEye.SetActive(false);
           // myText.text = "";
        }
        //ALERT Imagine Move Round bounds of screen
        Vector3 screenPos = Camera.main.WorldToScreenPoint(IndicatorAnchor.transform.position);
        //Distance to Scale UI
        //float distance = screenPos.z;

        Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

        //How far in the UI goes (.1f = 10% in)
        float margin = 0.1f;
        //Vector3 screenBounds = screenCenter * (1 - margin);

        // Screen bounds for individual sides
        float screenBoundsRight = screenCenter.x * (1 - margin);
        float screenBoundsLeft = -screenCenter.x * (1 - margin);
        float screenBoundsTop = screenCenter.y * (1 - margin);
        float screenBoundsBottom = -screenCenter.y * (1 - margin);

        screenPos -= screenCenter;

        if (screenPos.z < 0)
        {
            // Only invert x & y.  z still negative if behind to force off screen conditional when x & y are *in* bounds
            screenPos = new Vector3(-screenPos.x, -screenPos.y, screenPos.z);
        }

        float angle = Mathf.Atan2(screenPos.y, screenPos.x);
        angle -= 90 * Mathf.Deg2Rad;

        // Debug.Log(string.Format("Angle: {0}", angle * Mathf.Rad2Deg));

        if (screenPos.z < 0 || screenPos.x > screenBoundsRight || screenPos.x < screenBoundsLeft || screenPos.y > screenBoundsTop || screenPos.y < screenBoundsBottom)
        {
            // Off screen

            // Force vector to be larger than screen so we will hit bounds when behind (z is negative), but x & y not exceeding bounds
            screenPos *= 1000;

            float cos = Mathf.Cos(angle);
            float sin = -Mathf.Sin(angle);

            //y = mx+b where b = 0
            float m = cos / sin;

            if (screenPos.y < screenBoundsBottom)
                screenPos = new Vector3(screenBoundsBottom / m, screenBoundsBottom, 0);
            if (screenPos.y > screenBoundsTop)
                screenPos = new Vector3(screenBoundsTop / m, screenBoundsTop, 0);

            if (screenPos.x > screenBoundsRight)
                screenPos = new Vector3(screenBoundsRight, screenBoundsRight * m, 0);
            if (screenPos.x < screenBoundsLeft)
                screenPos = new Vector3(screenBoundsLeft, screenBoundsLeft * m, 0);
        }

        screenPos += screenCenter;
        myText.transform.position = screenPos;
        DetectedEye.transform.position = screenPos;
        DetectionEye.transform.position = screenPos;



        //Detection and Behavior
        detection.fillAmount = alertMeter;
        if (!aINav.ragDolled)
        {
            if (alertMeter >= alertMax && !armed)
            {
                aINav.animator.SetBool("startled", true);
                nav.isStopped =  true;

            }
            else if ((!enemySight.playerInSight && !enemySight.hearingPlayer) && (objectHeard || bodySeen) && alertMeter < alertMax)
            {
                Distracted();
               
            }
            else if ((enemySight.hearingPlayer || enemySight.playerInSight) && alertMeter < alertMax)
            {
                if (!alerting)
                    StartCoroutine(Alerting());
            }
            else if ((enemySight.playerInSight || inAttackPattern) && attackType != 1)
            {
               // Debug.Log("Shoot!");
                AttackPattern();
            }
            else if (chaseTimer > chaseWaitTime && alertMeter >= alertMax && enemiesInCombat.Count == 0)
            {
                //Debug.Log("StopChasing");
                StopChasing();
            }
            else if (enemySight.personalLastSighting != lastPlayerSighting.resetPosition && alertMeter >= alertMax)
            {
                //Debug.Log("Chasing");
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
        nav.SetDestination(distractedPos);
        nav.speed = patrolSpeed;

        if (nav.remainingDistance < 2)
        {
            StartCoroutine(Inspect());
        }
    }

    IEnumerator Inspect()
    {
        //run looking around animation
       // aINav.animator.SetBool("startled", true);
        yield return new WaitForSeconds(10);
        objectHeard = false;
        bodySeen = false;
    }


    IEnumerator Alerting()
    {
        float listenMod = 1.5f;
        if (enemySight.hearingPlayer)
        {
            listenMod = 3.5f;
        } else
        {
            listenMod = 1.5f;
        }
        alerting = true;
        while (alertMeter < 1)
        {
            distMod = 10 / enemySight.dist;
            alertMeter += (Time.deltaTime * lightMod * distMod)/listenMod;
            yield return 0;
            if (!enemySight.playerInSight && !enemySight.hearingPlayer)
                break;
          //  AIaudioSource.clip = AIaudio[2];
          //  AIaudioSource.PlayOneShot(AIaudio[2]);
        }

        if (alertMeter < alertMax)
        {
           // AIaudioSource.PlayOneShot(AIaudio[3]);
            StartCoroutine(StopAlerting());
        }
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
        float t = 0;
        yield return new WaitForSeconds(2);
        if (!enemySight.playerInSight && !enemySight.hearingPlayer && alertMeter < alertMax)
        {
            while (alertMeter > 0)
            {
                if (enemySight.playerInSight || enemySight.hearingPlayer)
                    break;
                alertMeter -= Time.deltaTime * lightMod;
                yield return 0;
                // if (enemySight.playerInSight)
                //   break;
            }
            if (alertMeter < 0)
            {
                alertMeter = 0;
            }
        }

    }


    void AttackPattern()
    {

        if (attackType != 2)
        {
           // Debug.Log(attackType);
            //Debug.Log("runnintoAttack");
            nav.isStopped = false;
            if (!foundAttackPoint)
            {
                inAttackPattern = true;
                NavMeshHit hitNM;
                RaycastHit hitRC;
                Vector3 RandomPoint;
                float distToPlayer = Vector3.Distance(player.position, transform.position);
                if (distToPlayer < 10)
                {

                    RandomPoint = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-5, 5), transform.position.z + Random.Range(-5, 5));
                    // RandomPoint =  new Vector3(Random.Range(-5, 5),Random.Range(-5, 5), Random.Range(-5, 5));
                    // RandomPoint -= transform.position +(transform.position - player.position);
                    //RandomPoint += player.position - transform.position;

                    Vector3 dir = (transform.position - player.transform.position);
                    dir.Normalize();
                    RandomPoint += (dir * 5);
                    RandomPoint = new Vector3(RandomPoint.x, transform.position.y, RandomPoint.z);
                    Debug.DrawLine(transform.position, RandomPoint, Color.green);
                }
                else
                {
                    RandomPoint = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-5, 5), transform.position.z + Random.Range(-5, 5));
                }

                //Check for if RandomPoint gets point on navMesh, calculate RayCast from point on navMesh + AI height, check if RayCast hit Player.
                if (NavMesh.SamplePosition(RandomPoint, out hitNM, 25, NavMesh.AllAreas))
                {
                    // Debug.Log("NavPointFound");
                    if (Physics.Raycast(hitNM.position + Vector3.up, (player.position - (hitNM.position + Vector3.up)).normalized, out hitRC, 25, layerMask))
                    {
                        // Debug.DrawLine(hitNM.position+Vector3.up, enemySight.player.transform.position, Color.blue);
                        //Debug.Log("FoundPlayer");
                        if (hitRC.transform.gameObject == player || hitRC.transform.root == player)
                        {

                            // Debug.Log("AttackPointFound");
                            attackPoint = hitNM.position;
                            foundAttackPoint = true;

                        }
                    }
                }
            }



            if (nav.destination != attackPoint)
            {
               

                rightAtRedirect = Vector3.left;
                leftAtRedirect = Vector3.right;

                nav.SetDestination(attackPoint);
               
            }
        }

        //Animation states and look rotation when running to attack point
        if (frames % 3 == 0 && !currentlyAttacking && attackType != 1 && attackType != 2)
        {
            nav.updateRotation = false;
            Vector3 directionToPlayer = (new Vector3 (player.position.x, 0, player.position.z) - (new Vector3(transform.position.x, 0 , transform.position.z))).normalized;
            Vector3 directionToNextPoint = (new Vector3(attackPoint.x, 0, attackPoint.z) - (new Vector3(transform.position.x, 0, transform.position.z))).normalized;
            float Rangle = Vector3.Angle(transform.right, directionToNextPoint);
            float Langle = Vector3.Angle(-transform.right, directionToNextPoint);
            //Debug.Log(Rangle + " Rangle");
            //Debug.Log(Langle + " Langle");
            float angle = Vector3.Angle(directionToPlayer, directionToNextPoint);
           //walking back = 180, toward = 0, always positive
           //Debug.Log(angle + " Angle");
            // Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            //transform.rotation = new Quaternion(transform.rotation.x, lookRotation.y,transform.rotation.z,transform.rotation.w);

            if (Rangle < 45)
            {
                Debug.Log("RunningRight");
                aINav.animator.SetBool("combatWalkBack", false);
                aINav.animator.SetBool("combatWalkForward", false);
                aINav.animator.SetBool("combatWalkRight", true);
                aINav.animator.SetBool("combatWalkLeft", false);
                aINav.animator.SetBool("combatKneel", false);
                aINav.animator.SetBool("combatRunAfter", false);
                aINav.animator.SetBool("combatAim", false);
            } else if (Langle < 45)
            {
                aINav.animator.SetBool("combatWalkBack", false);
                aINav.animator.SetBool("combatWalkForward", false);
                aINav.animator.SetBool("combatWalkRight", false);
                aINav.animator.SetBool("combatWalkLeft", true);
                aINav.animator.SetBool("combatKneel", false);
                aINav.animator.SetBool("combatRunAfter", false);
                aINav.animator.SetBool("combatAim", false);
                Debug.Log("RunningLeft");
            } else if (angle > 135)
            {
                Debug.Log("RunningBack");
                
                aINav.animator.SetBool("combatWalkBack", true);
                aINav.animator.SetBool("combatWalkForward", false);
                aINav.animator.SetBool("combatWalkRight", false);
                aINav.animator.SetBool("combatWalkLeft", false);
                aINav.animator.SetBool("combatKneel", false);
                aINav.animator.SetBool("combatRunAfter", false);
                aINav.animator.SetBool("combatAim", false);
            } else
            {
                Debug.Log("RunningFWD");
                aINav.animator.SetBool("combatWalkBack", false);
                aINav.animator.SetBool("combatWalkForward", true);
                aINav.animator.SetBool("combatWalkRight", false);
                aINav.animator.SetBool("combatWalkLeft", false);
                aINav.animator.SetBool("combatKneel", false);
                aINav.animator.SetBool("combatRunAfter", false);
                aINav.animator.SetBool("combatAim", false);
            }

          

        }

        if ((nav.remainingDistance < .35f || attackType == 2) && !currentlyAttacking)
            {
                //Debug.Log("InPlace");
                StartCoroutine(Attack());
            }

        
        //  Debug.Log("Shooting");
        //may change this to spherical raycast
        //  nav.isStopped = true;

        //Reset vision timer if in sight
        chaseTimer = 0;
    }
    IEnumerator Attack()
    {
        //Debug.Log("Attack!");
        if (attackType == 1)
            yield return 0;
        currentlyAttacking = true;


        if (attackType != 1)
        {
            aINav.animator.SetBool("combatWalkBack", false);
            aINav.animator.SetBool("combatWalkForward", false);
            aINav.animator.SetBool("combatWalkRight", false);
            aINav.animator.SetBool("combatWalkLeft", false);
            aINav.animator.SetBool("combatKneel", false);
            aINav.animator.SetBool("combatRunAfter", false);
            aINav.animator.SetBool("combatAim", true);
            //Debug.Log("AT1");
            nav.isStopped = true;
            float t = 0;
            while (t < 2)
            {
                //Debug.Log("Loop!");
                Vector3 direction = (player.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
                t += Time.deltaTime;
                yield return 0;
            }
            Shoot();
            AIaudioSource.clip = AIaudio[0];
            AIaudioSource.Play();
            inAttackPattern = false;
            foundAttackPoint = false;
            currentlyAttacking = false;
            nav.isStopped = false;
            nav.updateRotation = true;
        }
        else if (attackType == 1)
        {
           
            aINav.animator.SetBool("runningMelee", true);
        }


    }
    void Shoot()
    {
        RaycastHit hitRC;
        float accValue = (Vector3.Distance(player.position, transform.position) / 15) + (player.GetComponent<CharacterController>().velocity.magnitude/15);
       //Debug.Log(accValue);
        Vector3 inaccuratePos = new Vector3(player.position.x + Random.Range(-accValue, accValue), player.position.y + Random.Range(-accValue, accValue), player.position.z + Random.Range(-accValue, accValue));
        if (Physics.Raycast(shootPoint.position, (inaccuratePos - shootPoint.position).normalized, out hitRC, Mathf.Infinity, layerMask) && hitRC.transform.gameObject == player.gameObject)
        {
            PlayerManager.instance.GetHurt(25f);
            StartCoroutine(PlayerManager.instance.HurtEffect());
        }
        else if (Physics.Raycast(shootPoint.position, (inaccuratePos - shootPoint.position).normalized, out hitRC, Mathf.Infinity, layerMask) && hitRC.transform.gameObject.layer == 14)
        {
            if (hitRC.transform.root.gameObject.GetComponent<EnemyAI>().attackType != 3 || hitRC.transform.root.gameObject.GetComponent<EnemyAI>().enemyHealth <= 25)
            {
                hitRC.transform.root.gameObject.GetComponent<AINav>().RagDoll();
                AIaudioSource.PlayOneShot(AIaudio[4]);
                foreach (Rigidbody rb in hitRC.transform.root.gameObject.GetComponent<AINav>().rbs)
                {
                    rb.useGravity = true;
                }
                hitRC.transform.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 75, ForceMode.Impulse);
            } else
            {
                hitRC.transform.root.gameObject.GetComponent<EnemyAI>().enemyHealth -= 25;
            }
          
        }
        Vector3 hitpoint = hitRC.point;
        StartCoroutine(BulletVisual(hitpoint));

    }
    IEnumerator BulletVisual(Vector3 hitPoint)
    {
       GameObject newBullet = Instantiate(bullet, shootPoint.transform.position, Quaternion.identity);
        float t = 0;
        while (t < 1)
        {
            newBullet.transform.position = Vector3.LerpUnclamped(shootPoint.position, hitPoint, linearCurve.Evaluate(t));
            t += Time.deltaTime * 10;
            yield return 0;
           
        }
        Destroy(newBullet);
    }
    public void EndRunningMelee()
    {
       // Debug.Log("We Out");
        currentlyAttacking = false;
        nav.isStopped = false;
        inAir = false;
        aINav.animator.SetBool("runningMelee", false);
        melee.GetComponent<BoxCollider>().enabled = false;
    }
    public void NavUpdate()
    {
        Vector3 oldPos = player.position;
        nav.SetDestination(oldPos);
        inAir = true;
    }
    public void NavStop()
    {
        nav.isStopped = true;
        melee.GetComponent<BoxCollider>().enabled = true;
    }

    void Chasing()
    {
       // Debug.Log("Chasing");
        if (!inAir && attackType != 2)
        {
            nav.SetDestination(player.position);
            if (attackType != 1)
            {
                aINav.animator.SetBool("combatWalkBack", false);
                aINav.animator.SetBool("combatWalkForward", false);
                aINav.animator.SetBool("combatWalkRight", false);
                aINav.animator.SetBool("combatWalkLeft", false);
                aINav.animator.SetBool("combatKneel", false);
                aINav.animator.SetBool("combatAim", false);
                aINav.animator.SetBool("combatRunAfter", true);
            }
        }
        if (!currentlyAttacking)
        {
            //Debug.Log("ChasingMe");
            if (attackType != 2)
            {
                nav.isStopped = false;
                nav.SetDestination(player.position);
                nav.speed = runSpeed;
            }

            //if they chase too long, they lose track of you after 10 seconds and move to your last position
            //may need to increase sight range beyond shooting range
            chaseTimer += Time.deltaTime;

            lastPlayerSighting.position = player.position;
            //enemySight.personalLastSighting = lastPlayerSighting.position;

            if (chaseTimer > chaseWaitTime && enemiesInCombat.Contains(gameObject))
            {
                enemiesInCombat.Remove(gameObject);
            }
            else if (enemySight.playerInSight && !enemiesInCombat.Contains(gameObject))
            {
                enemiesInCombat.Add(gameObject);
            }

        }
        //Melee Code
        if (attackType == 1)
        {
            nav.speed = runSpeed + 1f;
            if (enemySight.playerInSight)
            {
                chaseTimer = 0;
            }
            if (nav.remainingDistance < 7.5f && !currentlyAttacking)
            {
                //Debug.Log("ChasingAttack");
                StartCoroutine(Attack());
            }       
        }
    }

    void StopChasing()
    {

        //Debug.Log("Losing");
        if (attackType != 2){
            nav.isStopped = false;
            nav.SetDestination(enemySight.personalLastSighting);
            nav.speed = runSpeed;
        }
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
        if (attackType != 2)
        {
            enemySight.sightRadius = 10;
        }
       // enemySight.col.radius = 10;
    }
    void Patrolling()
    {
        // Debug.Log("Patrolling");
        patrolling = true;
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
        //Debug.Log("hooray");
        inCombat = true;
        armed = true;
       
        Collider[] heardYou = Physics.OverlapSphere(transform.position, 15, AIMask);
        foreach(Collider c in heardYou)
        {
            c.gameObject.GetComponent<EnemyAI>().alertMeter = 1;

            if (attackType == 2)
            {

            }
            else
            {
                c.GetComponent<EnemySight>().sightRadius = 25;
                //enemySight.col.radius = 25;
            }
        }
        enemiesInCombat.Add(gameObject);
        EnemySight.fieldOfViewAngle = 160;
        
    }
    void OnTriggerEnter(Collider other)
    {

        if ((other.gameObject.layer == 13 || other.gameObject.layer == 14) && other.gameObject.GetComponent<Rigidbody>().velocity.magnitude >10)
        {
            aINav.RagDoll();
            foreach (Rigidbody rb in aINav.rbs)
            {
                rb.useGravity = true;
            }
        }
    }
    public void EquipWeapon()
    {
        stowedGun.SetActive(false);
        stowedMelee.SetActive(false);
        if (attackType != 1)
        {
            gun.SetActive(true);
        } else
        {
            melee.SetActive(true);
        }
    }

}
