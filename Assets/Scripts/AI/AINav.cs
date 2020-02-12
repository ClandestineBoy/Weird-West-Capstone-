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
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
        agent.SetDestination(home.position);
        reactionTime = Random.Range(0, 1f);
        runAwaySpeed = Random.Range(7, 8);
    }

    // Update is called once per frame
    void Update()
    {

    }
   public void Reposition(Vector3 pos)
    {
        agent.SetDestination(transform.position);
        agent.speed = 0;
        agent.radius = .5f;
        running = true;
        StartCoroutine(RunAway(pos));
    }
    IEnumerator RunAway(Vector3 pos)
    {
        yield return new WaitForSeconds(reactionTime);
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
                
            } else
            {
                animator.SetBool("fallingBackward", true);
            }
        }
    }
}
