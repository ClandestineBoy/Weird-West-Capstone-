using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Swap : MonoBehaviour
{
    bool crouchTime = false;
    int layerMask = 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3 | 1 << 4 | 1 << 5 | 1 << 6 | 1 << 7 | 1 << 8 | 1 << 9 | 1<<11 | 1 << 12 | 1 << 13 | 1 << 14 | 1 << 16 | 1 << 17 | 1 << 18;
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
            SwapAction();
        }
        else if (PlayerManager.instance.currentHealth > manaCost * 2)
        {
            SwapAction();
            crouchTime = true;
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
        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            if (hit.transform.gameObject.layer == 11)
            {
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
