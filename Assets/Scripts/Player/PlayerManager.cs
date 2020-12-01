using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public float totalEnergy = 100;
    public float maxHealth;
    public float maxMana;
    public float currentHealth;
    public float currentMana;

    [Header("PlayerAudio")] 
    public AudioClip[] playerClips;

   
    public AudioClip gunFxTest;

    public AudioSource thisSource;
    public AudioSource footstepSource;
    public AudioMixerGroup Stealthmaster;
    public AudioMixerGroup player;
    
    public int equippedPower;
    public int equippedWeapon;

    public Mist mist;
    private Swap swap;
    private SwingController swingController;
    private Telekinesis telekinesis;
    private StealthGrapple stealthGrapple;
    private Gun gun;
    private Melee melee;

    public GameObject gunHand;
    public GameObject swordHand;
    public Vector3 swordStartRot;

    public bool crouching = false;

    public Image hp;
    public Image mp;
    public enum LightLevel { brightLight, dimLight, ambientLight, noLight };
    public LightLevel lightState = new LightLevel();

   GameObject[] NPCS;

    public PostProcessVolume ppVolume;
    Vignette vignette;
    LensDistortion lensDistortion;
    public AnimationCurve linearCurve;
    public AnimationCurve humpCurve;
    public Animator leftHand;
    public Animator gunHandAnim;
    public Animator swordHandAnim;
    bool inVignette;

    int myScene;

    private void Start()
    {
        swordStartRot = swordHand.transform.localEulerAngles;
        NPCS = GameObject.FindGameObjectsWithTag("NPC");

        stealthGrapple = GetComponent<StealthGrapple>();
        mist = GetComponent<Mist>();
        swap = GetComponent<Swap>();
        swingController = GetComponent<SwingController>();
        telekinesis = GetComponent<Telekinesis>();
        gun = GetComponent<Gun>();
        melee = GetComponentInChildren<Melee>();
        thisSource = this.GetComponent<AudioSource>();
        equippedPower = 3;
        instance = this;
        ppVolume = GameObject.Find("Post-Processing Volume").GetComponent<PostProcessVolume>();
        ppVolume.profile.TryGetSettings(out vignette);
        ppVolume.profile.TryGetSettings(out lensDistortion);
        myScene = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)){
            SceneManager.LoadScene(myScene);
        }

        hp.fillAmount = currentHealth / maxHealth;
        mp.fillAmount = currentMana / maxMana;
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
                    if (PlayerController.instance.status == Status.crouching)
                    {
                        stealthGrapple.GrappleCheck();   
                    } else if(PlayerController.instance.status != Status.grappling)
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
        if (Input.GetMouseButtonDown(0))
        {
            switch (equippedWeapon)
            {
                case 0:
                    gun.Shoot();
                    thisSource.clip = playerClips[0];
                    Debug.Log(thisSource.clip);
                    thisSource.Play();
                    break;
                case 1:
                    if (!melee.slashing)
                    {
                        StartCoroutine(melee.Slash());
                        int x = Random.Range(0, 1);
                        if (x >= .66)
                        {
                            thisSource.clip = playerClips[1];
                        }
                        else if((x >= .33f && x < .66f))
                        {
                            thisSource.clip = playerClips[2];
                        }
                        else if (x >= 0 && x < .33f)

                        {
                            thisSource.clip = playerClips[3];
                        }
                        Debug.Log(thisSource.clip);
                        thisSource.Play();
                    }
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

    /* public void OnTriggerEnter(Collider other)
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
     }   */

    // VFX
     public IEnumerator StealthVignette()
    {

        if (inVignette)
        {
            thisSource.outputAudioMixerGroup = player;
            inVignette = false;
            vignette.active = false;
            yield return 0;
        }
        inVignette = true;
       // thisSource.outputAudioMixerGroup = Stealthmaster;
        
     
        float startValue = vignette.intensity.value;
        float endValue = 0 ;
        if (vignette.intensity.value < .35f)
        {
            endValue = .35f;
        }
        float t = 0;
        while (t < 1)
        {
            vignette.intensity.value = Mathf.LerpUnclamped(startValue, endValue, linearCurve.Evaluate(t));
            t += Time.deltaTime * 10;
            if (!inVignette)
            {
                // vignette.intensity.value = endValue;
                vignette.active = false;
                break;
            }
            yield return 0;
            
        }
 
        vignette.intensity.value = endValue;
        if (inVignette)
        {
            vignette.active = true;
        }
        inVignette = false;

    }

    public IEnumerator SwapDistort()
    {
        leftHand.SetBool("swapAct", true);
        float startValue = lensDistortion.intensity.value;
        float endValue = -100;
        float t = 0;
        while (t < 1)
        {
            lensDistortion.intensity.value = Mathf.LerpUnclamped(startValue, endValue, linearCurve.Evaluate(t));
            t += Time.deltaTime * 10;
            yield return 0;
        }
        leftHand.SetBool("swapAct", false);
        t = 0;
        startValue = lensDistortion.intensity.value;
        endValue = 1;
        while (t < 1)
        {
            lensDistortion.intensity.value = Mathf.LerpUnclamped(startValue, endValue, humpCurve.Evaluate(t));
            t += Time.deltaTime * 10;
            yield return 0;
        }
        lensDistortion.intensity.value = endValue;

    }
}
