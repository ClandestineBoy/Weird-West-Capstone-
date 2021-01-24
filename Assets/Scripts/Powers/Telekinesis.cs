using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    public Transform liftPoint;
    public static GameObject liftedObject;
    bool liftingObject;
    bool isNPC;
    float objectVelocity = 18;
    public float manaCost;
    public GameObject tkEffect, tkEffect2;
    int layerMask = 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3 | 1 << 4 | 1 << 6 | 1 << 7 | 1 << 8 | 1 << 9 | 1 << 12 | 1 << 13 | 1 << 14 | 1 << 11 | 1<<16 | 1<<17 | 1<<18;
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
            tkEffect.SetActive(true); 
        } else
        {
            tkEffect.SetActive(false);
        }



    }
    public void DoTelekinesis()
    {
        if (PlayerController.instance.status == Status.crouching)
        {
            if ((PlayerManager.instance.currentHealth > manaCost * 2 || PlayerManager.instance.currentMana >0) && !liftingObject)
            {
                StealthPickUp();
            }
            else if (liftingObject)
            {
                StealthDropObject();
                PlayerManager.instance.leftHand.SetBool("teleAct", false);
            }
        } else
        {
            if ((PlayerManager.instance.currentHealth > manaCost * 2 || PlayerManager.instance.currentMana >0) && !liftingObject)
            {
                PickUp();
            }
            else if (liftingObject)
            {
                DropObject();
                PlayerManager.instance.leftHand.SetBool("teleAct", false);
            }

        }


    }


    void PickUp()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            if (hit.transform.gameObject.layer == 13)
            {
                liftPoint.position = transform.position + transform.forward * 3.5f;
                liftedObject = hit.transform.gameObject;
                liftedObject.GetComponent<Rigidbody>().useGravity = false;
                liftingObject = true;

            }
            else if ((hit.transform.gameObject.layer == 11 || hit.transform.gameObject.layer == 14) && hit.transform.root.gameObject.GetComponent<EnemyAI>().attackType != 3)
            {
                isNPC = true;
                liftPoint.position = transform.position + transform.forward * 3.5f;
                if (hit.transform.gameObject.GetComponent<AINav>() != null)
                    liftedObject = hit.transform.gameObject;
                else
                {
                    Transform tempParent = hit.transform.root;
                    hit.transform.root.DetachChildren();
                 tempParent.position = hit.transform.position;
                    hit.transform.root.parent = tempParent;
                    liftedObject = hit.transform.root.gameObject;
                }

                //Ragdoll


                if (liftedObject.GetComponent<AINav>() != null)
                {
                    liftedObject.GetComponent<AINav>().RagDoll();
                    liftedObject.GetComponent<AINav>().liftedBody = true;
                }
                
            

                liftingObject = true;
            } else if ((hit.transform.gameObject.layer == 11 || hit.transform.gameObject.layer == 14) && hit.transform.root.gameObject.GetComponent<EnemyAI>().attackType == 3)
            {
                PlayerController.instance.GetKnockedBack(hit.transform.root.transform);
            }
            if (liftingObject)
            {
                PlayerManager.instance.SpendMana(manaCost);
                PlayerManager.instance.leftHand.SetBool("teleAct", true);
            }        
        }
    }
    void DropObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!isNPC)
        {
            //Vector3 velocity = (liftPoint.position - liftedObject.transform.position) / Time.deltaTime;
            //liftedObject.GetComponent<Rigidbody>().AddForce(velocity / objectVelocity, ForceMode.Impulse);
            liftedObject.GetComponent<Rigidbody>().useGravity = true;
            liftedObject.GetComponent<Rigidbody>().AddForce(ray.direction * 50, ForceMode.Impulse);
        }
        else
        {

            foreach (Rigidbody rb in liftedObject.GetComponent<AINav>().rbs)
            {

                rb.useGravity = true;

                rb.AddForce(ray.direction * 50, ForceMode.Impulse);
                isNPC = false;
            }
            liftedObject.transform.root.GetComponent<AINav>().liftedBody = false;
        }
        liftedObject = null;
        liftingObject = false;
        
    }





    void StealthPickUp()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            if (hit.transform.gameObject.layer == 13)
            {
                liftPoint.position = hit.point;
                liftedObject = hit.transform.gameObject;
                liftedObject.GetComponent<Rigidbody>().useGravity = false;
                liftingObject = true;

            }
            else if ((hit.transform.gameObject.layer == 11 || hit.transform.gameObject.layer == 14) && hit.transform.root.gameObject.GetComponent<EnemyAI>().attackType != 3)
            {
                isNPC = true;
                liftPoint.position = hit.point;
                if (hit.transform.gameObject.GetComponent<AINav>() != null)
                    liftedObject = hit.transform.gameObject;
                else
                {
                    Transform tempParent = hit.transform.root;
                    hit.transform.root.DetachChildren();
                    tempParent.position = hit.transform.position;
                    hit.transform.root.parent = tempParent;
                    liftedObject = hit.transform.root.gameObject;
                }

                //Ragdoll
                if (liftedObject.GetComponent<AINav>() != null)
                {
                    liftedObject.GetComponent<AINav>().RagDoll();
                    liftedObject.GetComponent<AINav>().liftedBody = true;
                }
                liftingObject = true;
            }
            else if ((hit.transform.gameObject.layer == 11 || hit.transform.gameObject.layer == 14) && hit.transform.root.gameObject.GetComponent<EnemyAI>().attackType == 3)
            {
                PlayerController.instance.GetKnockedBack(hit.transform.root.transform);
            }
            if (liftingObject)
            {
                PlayerManager.instance.SpendMana(manaCost);
                PlayerManager.instance.leftHand.SetBool("teleAct", true);
            }
        }
    }

    void StealthDropObject()
    {
        Vector3 velocity = (liftPoint.position - liftedObject.transform.position) / Time.deltaTime;
        if (!isNPC)
        {
            liftedObject.GetComponent<Rigidbody>().AddForce(velocity / objectVelocity, ForceMode.Impulse);
            liftedObject.GetComponent<Rigidbody>().useGravity = true;
            liftedObject.GetComponent<Collisions>().enabled = true;
        }
            else
        {

            foreach (Rigidbody rb in liftedObject.GetComponent<AINav>().rbs)
            {

                rb.useGravity = true;

                rb.AddForce(velocity / objectVelocity, ForceMode.Impulse);
                isNPC = false;
            }
            liftedObject.transform.root.GetComponent<AINav>().liftedBody = false;
        }
        liftedObject = null;
        liftingObject = false;
        
    }
}
    

