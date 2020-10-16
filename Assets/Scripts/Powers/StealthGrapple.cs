using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthGrapple : MonoBehaviour
{
    public Transform tether;
    public AnimationCurve animCurve;

    int layerMask = 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3 | 1 << 4 | 1 << 5 | 1 << 6 | 1 << 7 | 1 << 8 | 1 << 9 | 1 << 10 | 1 << 12 | 1 << 13 | 1 << 14;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

       
    }
   public  void GrappleCheck()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            
            if (hit.transform.gameObject.layer == 13)
            {
                tether.position = hit.point;
                hit.transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
                StartCoroutine(PullObject(hit.transform.gameObject));
            } else if (hit.transform.gameObject.layer == 14 && hit.transform.root.gameObject.GetComponent<EnemyAI>().attackType != 3)
            {              
                tether.position = hit.point;
                StartCoroutine(PullPerson(hit.transform.gameObject));
            } else if (hit.transform.gameObject.layer == 14 && hit.transform.root.gameObject.GetComponent<EnemyAI>().attackType == 3)
            {
                PlayerController.instance.GetKnockedBack(hit.transform.root.transform);
            }

        }
    }
    IEnumerator PullObject (GameObject hitObject)
    {
        tether.parent = hitObject.transform;
        Vector3 StartPos = hitObject.transform.position;
        Vector3 EndPos = transform.position + transform.forward;
        float t = 0;
        while (t < 1)
        {
            hitObject.transform.position = Vector3.LerpUnclamped(StartPos, EndPos, animCurve.Evaluate(t));
            t += Time.deltaTime;
            yield return 0;
        }
        tether.parent = null;
        Debug.Log("In Hand: " + hitObject.name);
        Destroy(hitObject);
    }

    IEnumerator PullPerson (GameObject hitObject)
    {
       
        hitObject.transform.root.GetComponent<AINav>().RagDoll();
        foreach (Rigidbody rb in hitObject.transform.root.gameObject.GetComponent<AINav>().rbs)
        {
            rb.useGravity = true;
        }
        hitObject.transform.root.parent = tether;
        Vector3 StartPos = tether.position;
        Vector3 EndPos = transform.position + transform.forward;
        float t = 0;
        while (t < 1)
        {
            tether.position = Vector3.LerpUnclamped(StartPos, EndPos, animCurve.Evaluate(t));
            t += Time.deltaTime;
            yield return 0;
        }
        GameObject ai = tether.GetChild(0).gameObject;
        ai.transform.parent = null;
        Destroy(ai);
    }
}
