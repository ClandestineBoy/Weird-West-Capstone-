using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisions : MonoBehaviour
{
    Rigidbody rb;
    int layerMask = 1 << 11;
    Collisions collisions;
    // Start is called before the first frame update
    void Awake()
    {
        collisions = GetComponent<Collisions>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude < .25f)
        {
            collisions.enabled = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (rb.velocity.magnitude > 1)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 7.5f * rb.velocity.magnitude, layerMask);
            foreach (Collider col in hitColliders)
            {
                
               EnemySight enemySight = col.gameObject.GetComponent<EnemySight>();
                 if (col.gameObject.GetComponent<EnemySight>().CalculatePathLength(transform.position) <= enemySight.col.radius)
                {
                    col.gameObject.GetComponent<EnemyAI>().objectHeard = true;
                    col.gameObject.GetComponent<EnemyAI>().heardPos = transform.position;
                }
            }
        }
            
    }
}
