using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Animations;
public class PowerSwap : MonoBehaviour
{
    public static PowerSwap instance;

    public GameObject SwapUI;
    public bool swapping;
    public AudioSource UISource;
    public AudioClip MenuTic;

    public Vector2 moveInput;
    public TMP_Text[] options;
    public GameObject ShadowHand;
    public Vector3 ShadowHandStartPos;
    public Transform shadowParent;
    public Image Mist, Swap, Swing, Tele, Gun, Melee, Mist2, Swap2, Swing2, Tele2, Gun2, Melee2;
    Color startCol;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ShadowHandStartPos = ShadowHand.transform.localPosition;
        shadowParent = ShadowHand.transform.parent;
        startCol = new Color(.705f, .705f, .705f, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(2))
        {
            Time.timeScale = .2f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            swapping = true;
            SwapUI.SetActive(true);
            ShadowHand.SetActive(false);

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
                //UISource.PlayOneShot(MenuTic);


                for (int i = 0 ; i < options.Length; i++)
                {
                    if (angle > i * (360 / options.Length) && angle < (i + 1) * (360/ options.Length))
                    {
                       Debug.Log(i);
                        if (i == 0)
                        {
                            Mist.color = Color.white;
                            Swap.color = startCol;
                            Swing.color = startCol;
                            Tele.color = startCol;
                            Gun.color = startCol;
                            Melee.color = startCol;
                        } else if (i == 1)
                        {
                            Mist.color = startCol;
                            Swap.color = Color.white;
                            Swing.color = startCol;
                            Tele.color = startCol;
                            Gun.color = startCol;
                            Melee.color = startCol;
                        } else if (i == 2)
                        {
                            Mist.color = startCol;
                            Swap.color = startCol;
                            Swing.color = Color.white;
                            Tele.color = startCol;
                            Gun.color = startCol;
                            Melee.color = startCol;
                        } else if (i == 3)
                        {
                            Mist.color = startCol;
                            Swap.color = startCol;
                            Swing.color = startCol;
                            Tele.color = Color.white;
                            Gun.color = startCol;
                            Melee.color = startCol;
                        } else if (i == 4)
                        {
                            Mist.color = startCol;
                            Swap.color = startCol;
                            Swing.color = startCol;
                            Tele.color = startCol;
                            Gun.color = Color.white;
                            Melee.color = startCol;
                        } else if (i == 5)
                        {
                            Mist.color = startCol;
                            Swap.color = startCol;
                            Swing.color = startCol;
                            Tele.color = startCol;
                            Gun.color = startCol;
                            Melee.color = Color.white;
                        }


                        if (i <= 3)
                            PlayerManager.instance.equippedPower = i;
                        else
                            PlayerManager.instance.equippedWeapon = i - 4;
                          
                    }
                }
            }

            


        } else if (Input.GetMouseButtonUp(2))
        {
            //UISource.PlayOneShot(MenuTic);
            Time.timeScale = 1;
            SwapUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            swapping = false;
            Cursor.visible = false;
            if (PlayerManager.instance.equippedWeapon == 0)
            {
                PlayerManager.instance.gunHand.SetActive(true);
                PlayerManager.instance.swordHand.SetActive(false);
            } else if (PlayerManager.instance.equippedWeapon == 1)
            {
                PlayerManager.instance.gunHand.SetActive(false);
                PlayerManager.instance.swordHand.SetActive(true);
            }

            if (PlayerManager.instance.equippedPower == 0)
            {
                PlayerManager.instance.leftHand.SetBool("mistIdle", true);
                PlayerManager.instance.leftHand.SetBool("teleIdle", false);
                PlayerManager.instance.leftHand.SetBool("swapIdle", false);
                PlayerManager.instance.leftHand.SetBool("grappleIdle", false);
                Mist2.gameObject.SetActive(true);
                Swap2.gameObject.SetActive(false);
                Swing2.gameObject.SetActive(false);
                Tele2.gameObject.SetActive(false);
            }
            else if (PlayerManager.instance.equippedPower == 1)
            {
                PlayerManager.instance.leftHand.SetBool("swapIdle", true);
                PlayerManager.instance.leftHand.SetBool("teleIdle", false);
                PlayerManager.instance.leftHand.SetBool("mistIdle", false);
                PlayerManager.instance.leftHand.SetBool("grappleIdle", false);
                Swap2.gameObject.SetActive(true);
                Mist2.gameObject.SetActive(false);
                Swing2.gameObject.SetActive(false);
                Tele2.gameObject.SetActive(false);
            }
            else if (PlayerManager.instance.equippedPower == 2)
            {
                PlayerManager.instance.leftHand.SetBool("teleIdle", false);
                PlayerManager.instance.leftHand.SetBool("mistIdle", false);
                PlayerManager.instance.leftHand.SetBool("swapIdle", false);
                PlayerManager.instance.leftHand.SetBool("grappleIdle", true);
                ShadowHand.SetActive(true);
                Swap2.gameObject.SetActive(false);
                Swing2.gameObject.SetActive(true);
                Mist2.gameObject.SetActive(false);
                Tele2.gameObject.SetActive(false);
            }
            else if (PlayerManager.instance.equippedPower == 3)
            {
                PlayerManager.instance.leftHand.SetBool("teleIdle", true);
                PlayerManager.instance.leftHand.SetBool("mistIdle", false);
                PlayerManager.instance.leftHand.SetBool("swapIdle", false);
                PlayerManager.instance.leftHand.SetBool("grappleIdle", false);
                Tele2.gameObject.SetActive(true);
                Swap2.gameObject.SetActive(false);
                Swing2.gameObject.SetActive(false);
                Mist2.gameObject.SetActive(false);
            }
        }
    }
}
