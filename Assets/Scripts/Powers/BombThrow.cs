using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombThrow : MonoBehaviour
{
    public GameObject scatterBomb;
    public float bombVelocity = 10;

    public Transform hand;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {

        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            GameObject newBomb = Instantiate(scatterBomb, transform.position, Quaternion.identity);
            newBomb.GetComponent<ScatterBomb>().setParams(
                hand.position,          // Assumes script is on hand, can set to hand public var if you like, may want offset added to look like frrom in hand rather than finger
                Player_Controller.instance.transform.forward,
                Player_Controller.instance.currentY,
                bombVelocity
            );

            newBomb.GetComponent<ScatterBomb>().Go();
        }

    }
}
