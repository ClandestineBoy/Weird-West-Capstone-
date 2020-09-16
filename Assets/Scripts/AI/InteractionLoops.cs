using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionLoops : MonoBehaviour
{
    public bool talkingGuard;
    public Array VoiceLineOptions;
    int layerMask = 1 << 11;
    //dist = Vector3.Distance(player.transform.position, transform.position);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!EnemyAI.inCombat && GetComponent<EnemySight>().dist<20)
        {
            if (Physics.OverlapSphere(transform.position, 3, layerMask).Length > 1)
            {
                StartCoroutine(Drunkard());
            }
        }
        
    }

    //Will Play twice, one for each AI detecting one another
    IEnumerator Drunkard()
    {
        Debug.Log("What are you a drunkard?");
        yield return 0;
             
    }
}
