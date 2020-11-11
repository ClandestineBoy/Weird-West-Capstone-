using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobber : PlayerController
{ 
    private float timer = 0.0f; 
    float bobbingSpeed = 0.38f; 
    float bobbingAmount = .1f; 
    float midpoint = 0.0f;
    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        if (status == Status.moving && movement.grounded)
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
                cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, midpoint + translateChange,
                    cam.transform.localPosition.z);
            }
            else
            {
                cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, midpoint, cam.transform.localPosition.z);
            }
        }
        else
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(0, 0, 0), Time.deltaTime);
        }
        
    }
}
