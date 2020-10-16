using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //everything except npc root capsule collider
    int layerMask = 1 << 0 | 1<< 1 | 1<<2 | 1<<3 | 1<<4 | 1<<5 | 1<<6 | 1<<7 | 1<< 8 | 1<<9  | 1<<12| 1<<13 | 1<<14 | 1<<16 | 1<<17 | 1<<18;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

       
    }
    public void Shoot()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {

            //Debug.Log(hit.transform.name);
            if (hit.transform.gameObject.layer == 14)
            {
                if (hit.transform.root.gameObject.GetComponent<EnemyAI>().attackType != 3 || hit.transform.root.gameObject.GetComponent<EnemyAI>().enemyHealth <= 25)
                {
                    hit.transform.root.gameObject.GetComponent<AINav>().RagDoll();

                    foreach (Rigidbody rb in hit.transform.root.gameObject.GetComponent<AINav>().rbs)
                    {
                        rb.useGravity = true;
                    }
                    hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 75, ForceMode.Impulse);
                    Debug.Log("boom2");
                }
                else if (hit.transform.root.gameObject.GetComponent<EnemyAI>().attackType == 3)
                {
                    hit.transform.root.gameObject.GetComponent<EnemyAI>().enemyHealth -= 25;
                }
            }

        }
    }
}

