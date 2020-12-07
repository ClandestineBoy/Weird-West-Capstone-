using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class rope : MonoBehaviour
{
    public Transform bob;
    public Transform target1;
    private Transform curTarget;
    LineRenderer lr;
    private bool isTarget1 = true;
    public bool hasParent;

    // Use this for initialization
    void Start()
    {
        bob = GameObject.Find("ShadowArm").transform;
        StartCoroutine(ShadowArmOFF());
        target1 = GameObject.Find("Player Movement").transform;
        lr = GetComponent<LineRenderer>();
        if (hasParent)
        {
            lr.SetPosition(1, transform.InverseTransformPoint(bob.position));
        }
        else
        {
            lr.SetPosition(1, bob.position);
        }

        curTarget = target1;

     

    }

    // Update is called once per frame
    void Update()
    {
        

        if (PlayerController.instance.status == Status.grappling)
        {
            lr.enabled = true;
        }
        else
            lr.enabled = false;

        lr = GetComponent<LineRenderer>();
        if (hasParent)
        {
            lr.SetPosition(1, transform.InverseTransformPoint(bob.position));
        }
        else
        {
            lr.SetPosition(1, bob.position);
        }

        if (hasParent)
        {
            lr.SetPosition(0, transform.InverseTransformPoint(target1.position));
        }
        else
        {
            lr.SetPosition(0, target1.position);
        }
       

    }
    
    IEnumerator ShadowArmOFF()
    {
        yield return 0;
        GameObject.Find("ShadowArm").gameObject.SetActive(false);
    }


}
