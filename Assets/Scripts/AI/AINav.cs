using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINav : MonoBehaviour
{
    public Transform home;
   public  NavMeshAgent agent;
    float reactionTime;
    float runAwaySpeed;
    bool running;
    public Animator animator;
    bool fallen;
    public Transform spawn;

    public bool ragDolled;
    public bool liftedBody;


    Vector3 runPos;
    //Ragdoll
    [HideInInspector]
    public  Rigidbody[] rbs;
    [HideInInspector]
    public CapsuleCollider[] ccs;
    [HideInInspector]
    public SphereCollider[] scs;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
      //  agent.SetDestination(home.position);
        reactionTime = Random.Range(.25f, 1.25f);
        runAwaySpeed = Random.Range(7, 8);

        //Ragdoll
        ccs = GetComponentsInChildren<CapsuleCollider>();
        rbs = GetComponentsInChildren<Rigidbody>();
        foreach (CapsuleCollider cc in ccs)
        {
            cc.isTrigger = true;
        }

        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
        }
        foreach (SphereCollider sc in scs)
        {
            sc.isTrigger = true;
        }

        GetComponentInChildren<BoxCollider>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = true;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!ragDolled)
        {
            if (agent.isStopped)
            {
                animator.SetBool("isWalking", false);
            }
            else
            {
                animator.SetBool("isWalking", true);
            }
        }
       /* if (fallen) {
            agent.SetDestination(transform.position);
            agent.speed = 0;
        }
        if (running && transform.position == runPos)
        {
            agent.SetDestination(home.position);    
        }*/
    }
   public void Reposition(Vector3 pos)
    {
        agent.SetDestination(transform.position);
        agent.speed = 0;
        agent.radius = .5f;
        running = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isFreakingOut", true);
        if (!fallen)
        StartCoroutine(RunAway(pos));
    }
    IEnumerator RunAway(Vector3 pos)
    {
        yield return new WaitForSeconds(reactionTime);
        animator.SetBool("isRunning", true);
        animator.SetBool("isFreakingOut", false);
        animator.SetBool("isWalking", false);
        agent.speed = runAwaySpeed;
        agent.SetDestination(pos);
        runPos = pos;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (running)
        {
            agent.SetDestination(transform.position);
            agent.speed = 0;

            Vector3 dir = (collision.gameObject.transform.position - new Vector3(transform.position.x,
                 collision.gameObject.transform.position.y, transform.position.z)).normalized;
            float angle = Mathf.Abs(Vector3.Angle(transform.forward, dir));
            if (angle <270 && angle > 90)
            {
                animator.SetBool("fallingForward", true);
                fallen = true;
                
            } else
            {
                fallen = true;
                animator.SetBool("fallingBackward", true);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.transform == home)
        //{
          //  transform.position = spawn.position;
        //}
    }

    public void RagDoll()
    {
        if (EnemyAI.enemiesInCombat.Contains(gameObject))
        {
            EnemyAI.enemiesInCombat.Remove(gameObject);
        }
        ragDolled = true;
        foreach (CapsuleCollider cc in ccs)
        {
            cc.isTrigger = false;
        }
        foreach (SphereCollider sc in scs)
        {
            sc.isTrigger = false;
        }
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            
        }

        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Animator>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponentInChildren<BoxCollider>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = false;
    }
}
