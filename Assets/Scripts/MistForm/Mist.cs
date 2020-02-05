using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Mist : MonoBehaviour
{
   public float mistDuration;
   public float mistSpeed;
   public float mistCost;
    float originalSpeed;
   public bool isMist = false;

   public IEnumerator BecomeMist()
    {
        isMist = true;
        Physics.IgnoreLayerCollision(9, 10, true);
        originalSpeed = Player_Controller.instance.speed;
        Player_Controller.instance.speed = mistSpeed;
        PlayerManager.instance.SpendMana(mistCost);
        GetComponentInChildren<CameraShader>().enabled = true;
        yield return new WaitForSeconds(mistDuration);
        BecomeHuman();
    }
    public void BecomeHuman()
    {
        isMist = false;
        Physics.IgnoreLayerCollision(9,10, false);
        GetComponentInChildren<CameraShader>().enabled = false;
        Player_Controller.instance.speed = originalSpeed;
        
    }
}
