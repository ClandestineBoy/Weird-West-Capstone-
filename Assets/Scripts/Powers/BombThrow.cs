using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombThrow : MonoBehaviour
{
    bool drawing;
    public GameObject invisBomb;
    public GameObject scatterBomb;
    // Start is called before the first frame update
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if (!drawing)
            {
              // StartCoroutine(Draw());
            }
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            GameObject newBomb = Instantiate(scatterBomb, transform.position, Quaternion.identity);
            newBomb.GetComponent<Rigidbody>().AddForce((transform.forward + Vector3.up) * 10, ForceMode.Impulse);
        }
    }
    IEnumerator Draw()
    {
       GameObject newBomb = Instantiate(invisBomb, transform.position, Quaternion.identity);
        newBomb.GetComponent<Rigidbody>().AddForce((transform.forward + Vector3.up) * 10, ForceMode.Impulse);
        drawing = true;
        yield return new WaitForSeconds(1);
        drawing = false;
        Destroy(newBomb);
    }
}
