using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Mist
{
   public float mistDuration;
   public float mistSpeed;
   public float mistCost;
    float originalSpeed;
    int layerMask1 = 1 >> 9;
    int layerMask2 = 1 >> 10;
   public bool isMist = false;




   public IEnumerator BecomeMist()
    {
        isMist = true;
        Physics.IgnoreLayerCollision(layerMask1, layerMask2, true);
        originalSpeed = Player_Controller.instance.speed;
        Player_Controller.instance.speed = mistSpeed;
        yield return new WaitForSeconds(mistDuration);
        BecomeHuman();
    }
    public void BecomeHuman()
    {
        isMist = false;
        Debug.Log("das");
        Physics.IgnoreLayerCollision(layerMask1,layerMask2, false);
        Player_Controller.instance.speed = originalSpeed;
        
    }
}
