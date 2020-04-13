using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PowerSwap : MonoBehaviour
{
    public static PowerSwap instance;

    public GameObject SwapUI;
    public bool swapping;

    public Vector2 moveInput;
    public TMP_Text[] options;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(2))
        {
            Cursor.lockState = CursorLockMode.None;
            swapping = true;
            SwapUI.SetActive(true);

            moveInput.x = Input.mousePosition.x - (Screen.width / 2f);
            moveInput.y = Input.mousePosition.y - (Screen.height / 2f);
            moveInput.Normalize();

            if (moveInput != Vector2.zero) {
                float angle = Mathf.Atan2(moveInput.y, -moveInput.x) / Mathf.PI;
                angle *= 180;
                angle += 90f;
                if (angle < 0)
                {
                    angle += 360;
                }


                for (int i = 0 ; i < options.Length; i++)
                {
                    if (angle > i * (360 / options.Length) && angle < (i + 1) * (360/ options.Length))
                    {
                        PlayerManager.instance.equippedPower = i;         
                          
                    }
                }
            }

            


        } else if (Input.GetMouseButtonUp(2))
        {
            SwapUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            swapping = false;
        }
    }
}
