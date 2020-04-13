using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    public Transform liftPoint;
    private GameObject liftedObject;
    bool liftingObject;
    bool isNPC;
    float objectVelocity = 18;
    public float manaCost;
    int layerMask = 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3 | 1 << 4 | 1 << 5 | 1 << 6 | 1 << 7 | 1 << 8 | 1 << 9 | 1 << 10 | 1 << 12 | 1 << 13 | 1 << 14 | 1<<11;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (liftingObject)
        {
            liftedObject.transform.position = Vector3.Lerp(liftedObject.transform.position, liftPoint.position, Time.deltaTime * 2);        
        }



    }
    public void DoTelekinesis()
    {
        if (PlayerManager.instance.currentHealth > manaCost * 2 && !liftingObject)
        {
            PickUp();
        }
        else {
            DropObject();
        }

    }
    void PickUp()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.transform.gameObject.layer == 13)
            {
                liftPoint.position = hit.transform.position;
                liftedObject = hit.transform.gameObject;
                liftedObject.GetComponent<Rigidbody>().useGravity = false;
                liftingObject = true;

            }
            else if (hit.transform.gameObject.layer == 11 || hit.transform.gameObject.layer == 14)
            {
                isNPC = true;
                liftPoint.position = transform.position + transform.forward * 5;
                if (hit.transform.gameObject.GetComponent<AINav>() != null)
                    liftedObject = hit.transform.gameObject;
                else
                    liftedObject = hit.transform.root.gameObject;

                //Ragdoll
                if (liftedObject.GetComponent<AINav>() != null && !liftedObject.GetComponent<AINav>().ragDolled)
                    liftedObject.GetComponent<AINav>().RagDoll();
                liftingObject = true;
            }
        }
    }
        void DropObject()
        {
         if (!isNPC)
        {
            Vector3 velocity = (liftPoint.position - liftedObject.transform.position) / Time.deltaTime;
            liftedObject.GetComponent<Rigidbody>().AddForce(velocity / objectVelocity, ForceMode.Impulse);
            liftedObject.GetComponent<Rigidbody>().useGravity = true;
        }
        else
        {

            foreach (Rigidbody rb in liftedObject.GetComponent<AINav>().rbs)
            {

                rb.useGravity = true;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                rb.AddForce(ray.direction * 50, ForceMode.Impulse);
                isNPC = false;
            }
        }
        liftedObject = null;
        liftingObject = false;
    }
}
