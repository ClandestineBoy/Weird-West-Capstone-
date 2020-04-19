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
            Debug.Log("NOISE");
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5 * rb.velocity.magnitude, layerMask);
            foreach (Collider col in hitColliders)
            {
                
               EnemySight enemySight = col.gameObject.GetComponent<EnemySight>();
                 if (enemySight.CalculatePathLength(transform.position) <= enemySight.col.radius)
                {
                    col.gameObject.GetComponent<EnemyAI>().objectHeard = true;
                    col.gameObject.GetComponent<EnemyAI>().heardPos = transform.position;
                }
            }
        }
            
    }
}
