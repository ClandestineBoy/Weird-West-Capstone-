using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public float totalEnergy = 100;
    public float maxHealth;
    public float maxMana;
    public float currentHealth;
    public float currentMana;


    public int equippedPower;

    private Mist mist;
    private Swap swap;
    private SwingController swingController;
    private Telekinesis telekinesis;

    public enum LightLevel { brightLight, dimLight, ambientLight, noLight };
    public LightLevel lightState = new LightLevel();

   GameObject[] NPCS;
    private void Start()
    {
        NPCS = GameObject.FindGameObjectsWithTag("NPC");


        mist = GetComponent<Mist>();
        swap = GetComponent<Swap>();
        swingController = GetComponent<SwingController>();
        telekinesis = GetComponent<Telekinesis>();

        equippedPower = 2;
        instance = this;
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            
            SpendMana(10);
        }*/
        if (Input.GetKeyDown(KeyCode.P) && maxHealth > 0)
        {
            EctoStim(25);
        } else if (Input.GetKeyDown(KeyCode.O) && maxMana > 0)
        {
            HealthStim(25);
        }
        if (Input.GetMouseButtonDown(1))
        {
            switch (equippedPower)
            {
                case 0:
                    mist.enabled = true;
                    mist.DoMist();
                    break;
                case 1:
                    swap.DoSwap();
                    break;
                case 2:
                    if(PlayerController.instance.status != Status.grappling)
                     {
                         PlayerController.instance.SetUpGrapple();
                     }
                     else
                     {
                         PlayerController.instance.StopGrapple();
                     }
                    break;
                case 3:
                    telekinesis.DoTelekinesis();
                    break;
                        
            }

        }

        /*if (SwingController.instance.state != SwingController.State.Swinging)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && !mist.isMist)
            {
                StartCoroutine(mist.BecomeMist());
            }
        }*/
    }

    public void SpendMana(float manaCost)
    {
       
            if (currentMana < manaCost)
            {
                float healthCost = (currentMana - manaCost) * 2;
                currentMana = 0;
            currentHealth += healthCost;

            }
            else
            {
                currentMana -= manaCost;
            }  
    }

    public void EctoStim(float ectoAmount)
    {
        currentMana += ectoAmount;
        if (currentMana > maxMana)
        {
            float diff = currentMana - maxMana;
            maxHealth -= diff;
            if (maxHealth< currentHealth)
            {
                currentHealth = maxHealth;
            }
            maxMana = currentMana;
        }
    }

    public void HealthStim(float healthAmount)
    {
        currentHealth += healthAmount;
        if (currentHealth > maxHealth)
        {
            float diff = currentHealth - maxHealth;
            maxMana -= diff;
            if (maxMana < currentMana)
            {
                currentMana = maxMana;
            }
            maxHealth = currentHealth;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DimLight") || other.gameObject.CompareTag("BrightLight"))
        {
            //When entering bright light access npcs from array to activate light changes in enemyAI script
   
            foreach (GameObject n in NPCS)
            {
                if (n.GetComponent<EnemyAI>() != null)
                {
                    if (other.gameObject.CompareTag("BrightLight")) 
                        n.GetComponent<EnemyAI>().BrightLight();

                    else if (other.gameObject.CompareTag("DimLight"))
                        n.GetComponent<EnemyAI>().DimLight();
                }
            }
        } 
    }   
        
}
