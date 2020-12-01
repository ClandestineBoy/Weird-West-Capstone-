﻿using System.Collections;
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
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ShadowHandStartPos = ShadowHand.transform.localPosition;
        shadowParent = ShadowHand.transform.parent;
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
            }
            else if (PlayerManager.instance.equippedPower == 1)
            {
                PlayerManager.instance.leftHand.SetBool("swapIdle", true);
                PlayerManager.instance.leftHand.SetBool("teleIdle", false);
                PlayerManager.instance.leftHand.SetBool("mistIdle", false);
                PlayerManager.instance.leftHand.SetBool("grappleIdle", false);
            }
            else if (PlayerManager.instance.equippedPower == 2)
            {
                PlayerManager.instance.leftHand.SetBool("teleIdle", false);
                PlayerManager.instance.leftHand.SetBool("mistIdle", false);
                PlayerManager.instance.leftHand.SetBool("swapIdle", false);
                PlayerManager.instance.leftHand.SetBool("grappleIdle", true);
                ShadowHand.SetActive(true);
            }
            else if (PlayerManager.instance.equippedPower == 3)
            {
                PlayerManager.instance.leftHand.SetBool("teleIdle", true);
                PlayerManager.instance.leftHand.SetBool("mistIdle", false);
                PlayerManager.instance.leftHand.SetBool("swapIdle", false);
                PlayerManager.instance.leftHand.SetBool("grappleIdle", false);
            }
        }
    }
}
