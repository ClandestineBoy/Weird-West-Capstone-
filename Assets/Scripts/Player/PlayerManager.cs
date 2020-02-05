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
    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            
            SpendMana(10);
        }*/
        if (Input.GetKeyDown(KeyCode.P))
        {
            EctoStim(25);
        }
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
}
