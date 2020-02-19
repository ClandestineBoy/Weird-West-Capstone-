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
    private Animator animator;
    bool fallen;

    //Ragdoll
    [HideInInspector]
    public  Rigidbody[] rbs;
    [HideInInspector]
    public CapsuleCollider[] ccs;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
        agent.SetDestination(home.position);
        reactionTime = Random.Range(0, 1f);
        runAwaySpeed = Random.Range(7, 8);

        //Ragdoll
        ccs = GetComponentsInChildren<CapsuleCollider>();
        rbs = GetComponentsInChildren<Rigidbody>();
        foreach (CapsuleCollider cc in ccs)
        {
            cc.enabled = false;
        }

        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
        }

        GetComponentInChildren<BoxCollider>().enabled = false;
        GetComponentInChildren<SphereCollider>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = true;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fallen) {
            agent.SetDestination(transform.position);
            agent.speed = 0;
        }
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

    public void RagDoll()
    {
        foreach (CapsuleCollider cc in ccs)
        {
            cc.enabled = true;
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
