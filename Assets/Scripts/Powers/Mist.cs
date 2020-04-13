using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Mist : MonoBehaviour
{
   public float mistDuration;
   public float mistSpeed;
   public float manaCost;
    float originalSpeed;
   public bool isMist = false;

  
    
    public void DoMist()
    {
        
        if (!isMist && PlayerManager.instance.currentHealth > manaCost *2)
        {
            StartCoroutine(BecomeMist());
        }
    }
    public IEnumerator BecomeMist()
    {
        isMist = true;
        Physics.IgnoreLayerCollision(9, 10, true);
        originalSpeed = PlayerMovement.instance.walkSpeed;
        PlayerMovement.instance.walkSpeed = mistSpeed;
        PlayerManager.instance.SpendMana(manaCost);
        GetComponentInChildren<CameraShader>().enabled = true;
        yield return new WaitForSeconds(mistDuration);
        BecomeHuman();
    }
    public void BecomeHuman()
    {
        isMist = false;
        Physics.IgnoreLayerCollision(9,10, false);
        GetComponentInChildren<CameraShader>().enabled = false;
        PlayerMovement.instance.walkSpeed = originalSpeed;
        
    }
}
