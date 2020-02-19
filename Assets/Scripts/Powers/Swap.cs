using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swap : MonoBehaviour
{
    int layerMaskGates = 1 << 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    void Update()
    {
       if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.gameObject.layer == 11)
                {
                    Vector3 newPos = hit.transform.gameObject.transform.position;
                    Vector3 oldPos = transform.position;
                    transform.position = newPos;
                    hit.transform.gameObject.transform.position = oldPos;
                }

            }
        }
    }
}
