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
            PlayerMovement.instance.crouchSpeed = mistSpeed;
            PlayerManager.instance.SpendMana(manaCost);
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - .95f, Camera.main.transform.position.z);
            PlayerMovement.instance.controller.height = PlayerController.instance.halfheight / 10;
            yield return new WaitForSeconds(mistDuration);
            PlayerMovement.instance.controller.height = PlayerController.instance.halfheight;
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + .95f, Camera.main.transform.position.z);
        }
        
        BecomeHuman();

    }
    public void BecomeHuman()
    {
        isMist = false;
        PlayerController.instance.canSprint = true;
        Physics.IgnoreLayerCollision(9,10, false);
        GetComponentInChildren<CameraShader>().enabled = false;
        PlayerMovement.instance.walkSpeed = originalWalkSpeed;
        PlayerMovement.instance.crouchSpeed = originalCrouchSpeed;
    }
}
