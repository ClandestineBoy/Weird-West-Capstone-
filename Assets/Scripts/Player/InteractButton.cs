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
    // Start is called before the first frame update
    void Start()
    {
        
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
          //  EnactPressed();
        }

        heldButtonTimer = 0;
    }

    void EnactHeld()
    {

    }

    void EnactPressed()
    {
        Debug.Log("Enacted");
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 3, genericLayerMask))
        {
            Debug.Log(hit.transform.gameObject.name);
            if (hit.transform.gameObject.layer == 14 && hit.transform.root.GetComponent<AINav>().ragDolled && !hit.transform.root.GetComponent<AINav>().liftedBody)
            {
                Debug.Log(hit.transform.gameObject.name + "I Made it");
                hit.transform.gameObject.AddComponent<SpringJoint>();
                hit.transform.gameObject.GetComponent<SpringJoint>().connectedBody = rbC;
                hit.transform.gameObject.GetComponent<SpringJoint>().anchor = hit.point;

            }
        }
    }

}
