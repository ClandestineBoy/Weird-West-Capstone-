using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //everything except npc root capsule collider
    int layerMask = 1 << 0 | 1<< 1 | 1<<2 | 1<<3 | 1<<4 | 1<<5 | 1<<6 | 1<<7 | 1<< 8 | 1<<9  | 1<<12| 1<<13 | 1<<14 | 1<<16 | 1<<17 | 1<<18;
    public GameObject bullet;
    public Transform shootPoint;
    bool reloaded;
    // Start is called before the first frame update
    void Start()
    {
        reloaded = true;
    }

    // Update is called once per frame
    void Update()
    {

       
    }
    public void Shoot()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, layerMask) && reloaded)
        {
            StartCoroutine(BulletVisual(hit.point));
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
                   
                }
                else if (hit.transform.root.gameObject.GetComponent<EnemyAI>().attackType == 3)
                {
                    hit.transform.root.gameObject.GetComponent<EnemyAI>().enemyHealth -= 25;
                }
            }

        }
    }

    IEnumerator BulletVisual(Vector3 hitPoint)
    {
        reloaded = false;
        GameObject newBullet = Instantiate(bullet, shootPoint.transform.position, Quaternion.identity);
        float t = 0;
        while (t < 1)
        {
            newBullet.transform.position = Vector3.LerpUnclamped(shootPoint.position, hitPoint, PlayerManager.instance.linearCurve.Evaluate(t));
            t += Time.deltaTime * 10;
            yield return 0;

        }
        Destroy(newBullet);
        yield return new WaitForSeconds(2);
        reloaded = true;
    }

}

