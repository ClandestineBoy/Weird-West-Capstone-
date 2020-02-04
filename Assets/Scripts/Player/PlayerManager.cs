using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float totalEnergy = 100;
    public float maxHealth;
    public float maxMana;
    public float currentHealth;
    public float currentMana;

    public float manaCost;
    public float ectoAmount;

   
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            manaCost = 10;
            SpendMana();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            ectoAmount = 25;
            EctoStim();
        }
    }

    public void SpendMana()
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

    public void EctoStim()
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
