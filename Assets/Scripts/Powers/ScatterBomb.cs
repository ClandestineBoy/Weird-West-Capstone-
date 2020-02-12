using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScatterBomb : MonoBehaviour
{
    int layerMask = 1 << 11;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        
    }


    private void OnCollisionEnter(Collision collision)
    {
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 15, layerMask);
        Debug.Log(hitColliders.Length);
        foreach (var NPC in hitColliders)
        {

            float variable = Random.Range(-5, 5);
            Vector3 dir = (NPC.transform.position - new Vector3(transform.position.x+variable,
                NPC.transform.position.y, transform.position.z +variable)).normalized;
            
            NPC.GetComponent<AINav>().Reposition(NPC.transform.position + (dir*15f));
        }
    }
}
