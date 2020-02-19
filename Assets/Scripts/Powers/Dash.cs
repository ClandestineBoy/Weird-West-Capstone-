using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public AnimationCurve animCurve;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(DashMove());
        }
    }
    IEnumerator DashMove()
    {
        
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + Player_Controller.instance.moveDirection * 1.25f;
        float t = 0;
        while (t < 1)
        {
            transform.position = Vector3.LerpUnclamped(startPos, endPos, animCurve.Evaluate(t));
                t += Time.deltaTime * 6;
                yield return 0;
        }
        
    }
}
