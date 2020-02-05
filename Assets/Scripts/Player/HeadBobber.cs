using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobber : MonoBehaviour
{ 
    private float timer = 0.0f; 
    float bobbingSpeed = 0.38f; 
    float bobbingAmount = .1f; 
    float midpoint = 0.0f;

    void Update()
    {
        if (Player_Controller.instance.onGround)
        {
            float waveslice = 0.0f;
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
            {
                timer = 0.0f;
            }
            else
            {
                waveslice = Mathf.Sin(timer);
                timer = timer + bobbingSpeed;
                if (timer > Mathf.PI * 2)
                {
                    timer = timer - (Mathf.PI * 2);
                }
            }

            if (waveslice != 0)
            {
                float translateChange = waveslice * bobbingAmount;
                float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
                totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
                translateChange = totalAxes * translateChange;
                transform.localPosition = new Vector3(transform.localPosition.x, midpoint + translateChange,
                    transform.localPosition.z);
            }
            else
            {
                transform.localPosition = new Vector3(transform.localPosition.x, midpoint, transform.localPosition.z);
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0, 0), Time.deltaTime);
        }
        
    }
}
