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
            else if (hit.transform.gameObject.layer == 11)
            {
                isNPC = true;
                liftPoint.position = transform.position + transform.forward * 5;
                liftedObject = hit.transform.gameObject;

                //Ragdoll
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
