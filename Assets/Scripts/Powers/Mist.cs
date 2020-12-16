using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Mist : MonoBehaviour
{
   public float mistDuration;
   public float mistSpeed;
   public float manaCost;
    float originalWalkSpeed;
    float originalCrouchSpeed;
   public bool isMist = false;
    public GameObject camHolder;

  
    
    public void DoMist()
    {
        
        if (!isMist && (PlayerManager.instance.currentHealth > manaCost *2 || PlayerManager.instance.currentMana >= 0))
        {
            StartCoroutine(BecomeMist());
        }
    }
    public IEnumerator BecomeMist()
    {
        PlayerManager.instance.leftHand.SetBool("mistAct", true);
        isMist = true;
        PlayerController.instance.canSprint = false;
        Physics.IgnoreLayerCollision(9, 10, true);
        originalWalkSpeed = PlayerMovement.instance.walkSpeed;
        originalCrouchSpeed = PlayerMovement.instance.crouchSpeed;
        
        GetComponentInChildren<CameraShader>().enabled = true;
        if (!PlayerManager.instance.crouching)
        {
            PlayerMovement.instance.walkSpeed = mistSpeed;
            PlayerManager.instance.SpendMana(manaCost);
            yield return new WaitForSeconds(mistDuration);
        }
        else
        {
            PlayerMovement.instance.crouchSpeed = mistSpeed/4;
            
            camHolder.transform.position = new Vector3(camHolder.transform.position.x, camHolder.transform.position.y - .85f, camHolder.transform.position.z);
            PlayerMovement.instance.controller.height = PlayerController.instance.halfheight / 10;
            float prevLight = EnemyAI.lightMod;
            EnemyAI.lightMod = 0;
          while (Input.GetMouseButton(1) && PlayerManager.instance.currentHealth > 10)
            {
                PlayerManager.instance.SpendMana(1);
                yield return new WaitForSeconds(.25f);
            }
            EnemyAI.lightMod = prevLight;
            
            
            PlayerMovement.instance.controller.height = PlayerController.instance.halfheight;
            camHolder.transform.position = new Vector3(camHolder.transform.position.x, camHolder.transform.position.y + .85f, camHolder.transform.position.z);
        }
        
        BecomeHuman();

    }
    public void BecomeHuman()
    {
        PlayerManager.instance.leftHand.SetBool("mistAct", false);
        isMist = false;
        PlayerController.instance.canSprint = true;
        Physics.IgnoreLayerCollision(9,10, false);
        GetComponentInChildren<CameraShader>().enabled = false;
        PlayerMovement.instance.walkSpeed = originalWalkSpeed;
        PlayerMovement.instance.crouchSpeed = originalCrouchSpeed;
    }
}
