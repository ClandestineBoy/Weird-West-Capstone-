using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Swap : MonoBehaviour
{
    bool crouchTime = false;
    int layerMaskGates = 1 << 10;
    public float manaCost;
    CharacterController cc;
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame

    void Update()
    {
        if(crouchTime)
        SlowTime();
    }

    public void DoSwap()
    {
        if (PlayerManager.instance.crouching)
        {
            crouchTime = true;
        }
        else if (PlayerManager.instance.currentHealth > manaCost * 2)
        {
            SwapAction();
        }
    }

    void SlowTime()
    {
        Time.timeScale = .4f;
        PlayerManager.instance.SpendMana(1);
        if (Input.GetMouseButtonUp(1))
        {
            Time.timeScale = 1;
            crouchTime = false;
            SwapAction();
        }
    }

    void SwapAction()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.transform.gameObject.layer == 11)
            {
                PlayerManager.instance.SpendMana(manaCost);
                cc.enabled = false;
                hit.transform.gameObject.GetComponent<NavMeshAgent>().enabled = false;
                Vector3 newPos = hit.transform.gameObject.transform.position + Vector3.up;
                Vector3 oldPos = transform.position;
                transform.position = newPos;
                hit.transform.gameObject.transform.position = oldPos;
                cc.enabled = true;
                hit.transform.gameObject.GetComponent<NavMeshAgent>().enabled = true;
            }

        }
    }
}
