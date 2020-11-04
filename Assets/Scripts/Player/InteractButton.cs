using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractButton : MonoBehaviour
{

    float heldButtonTimer;
    bool buttonDown;
    bool held;
    bool pressed;
    int genericLayerMask = 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3 | 1 << 4 | 1 << 5 | 1 << 6 | 1 << 7 | 1 << 8 | 1 << 12 | 1 << 13 | 1 << 14 | 1 << 11 | 1 << 16 | 1 << 17 | 1 << 18;
    public Rigidbody rbC;

    public CharacterJoint cJoint;
    public float spring = 50.0f;
    public float damper = 5.0f;
    public float drag = 10.0f;
    public float angularDrag = 5.0f;
    public float distance = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
       // springJoint.spring = spring;
       // springJoint.damper = damper;
       // springJoint.maxDistance = distance; 
    }

    // Update is called once per frame
    void Update()
    {
        //For UI
      /*  RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 3, genericLayerMask))
        {
             if (hit.transform.gameObject.layer == 14 && hit.transform.root.GetComponent<AINav>().ragDolled && !hit.transform.root.GetComponent<AINav>().liftedBody)
            {

            }
        }*/


        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(HeldButtonCheck());
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            buttonDown = false;
        }
    }

    IEnumerator HeldButtonCheck()
    {
        buttonDown = true;
        while (buttonDown)
        {
            heldButtonTimer += Time.deltaTime;
            yield return 0;
        }

        //May need to change time values
        if (heldButtonTimer > .5f)
        {
            Debug.Log("Held");
            EnactHeld();
        } else if (heldButtonTimer > .2f)
        {
            Debug.Log("Middled");
           //UI
        } else
        {
            Debug.Log("Pressed");
            EnactPressed();
        }

        heldButtonTimer = 0;
    }

    void EnactHeld()
    {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 4, genericLayerMask))
        {
           
            if (hit.transform.gameObject.layer == 14 && hit.transform.root.GetComponent<AINav>().ragDolled && !hit.transform.root.GetComponent<AINav>().liftedBody)
            {
                Debug.Log(hit.transform.gameObject.name);
                Destroy(hit.transform.root.gameObject);
            }
        }
    }

    void EnactPressed()
    {
        
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 4, genericLayerMask))
        {
            //Debug.Log(hit.transform.gameObject.name);
            if (cJoint.connectedBody != null)
            {
                cJoint.connectedBody = null;
            }
            else if (hit.transform.gameObject.layer == 14 && hit.transform.root.GetComponent<AINav>().ragDolled && !hit.transform.root.GetComponent<AINav>().liftedBody)
            {
               // Debug.Log(hit.transform.gameObject.name + "I Made it");
              //  hit.transform.gameObject.AddComponent<SpringJoint>();
                cJoint.connectedBody = hit.transform.gameObject.GetComponent<Rigidbody>();
                //hit.transform.gameObject.GetComponent<SpringJoint>().anchor = hit.point;

            }
        }
    }

}
