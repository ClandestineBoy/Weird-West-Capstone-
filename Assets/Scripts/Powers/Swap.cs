using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swap : MonoBehaviour
{
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
     
    }

    public void DoSwap()
    {
        if (PlayerManager.instance.currentHealth > manaCost * 2)
        {
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
                cc.enabled = false;
                Vector3 newPos = hit.transform.gameObject.transform.position + Vector3.up;
                Vector3 oldPos = transform.position;
                transform.position = newPos;
                hit.transform.gameObject.transform.position = oldPos;
                cc.enabled = true;
            }

        }
    }
}
