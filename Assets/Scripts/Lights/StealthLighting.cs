using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthLighting : MonoBehaviour
{
    GameObject player;
    bool entered = false;

    SphereCollider col;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<SphereCollider>();
        player = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        //Only make changes to FOV when not in combat
        if (!EnemyAI.inCombat)
        {
            if (entered)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, player.transform.position, out hit, Mathf.Infinity))
                {
                    float dist = Vector3.Distance(transform.position, player.transform.position);
                    if (dist < col.radius / 2)
                    {
                        EnemySight.fieldOfViewAngle = 90;
                        EnemyAI.lightMod = 1;
                    }
                    else
                    {
                        EnemySight.fieldOfViewAngle = 70;
                        EnemyAI.lightMod = .5f;
                    }
                }
                else
                {
                    EnemySight.fieldOfViewAngle = 50;
                    EnemyAI.lightMod = .25f;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
            entered = true;
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            EnemySight.fieldOfViewAngle = 50;
            EnemyAI.lightMod = .25f;
            entered = false;
        }
    }
}
