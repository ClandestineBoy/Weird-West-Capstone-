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
    public int healthTicks = 3;
    public int manaTicks = 3;

    [Header("PlayerAudio")] 
    public AudioClip[] playerClips;

   
    public AudioClip SlideFxx;

    public AudioSource thisSource;
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
    public Image tick1, tick2, tick3, tick4, tick5, tick6;
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
    public GameObject bloodEffect;
    bool bloodied;

    int myScene;
    int frames;

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
        frames++;
        if (Input.GetKeyDown(KeyCode.K)){
            SceneManager.LoadScene(myScene);
        }

        /*
        hp.fillAmount = currentHealth / maxHealth;
        mp.fillAmount = currentMana / maxMana;

        if (currentHealth <= 25)
        {
            tick1.fillAmount = currentHealth /25;
        } else if (currentHealth <= 50)
        {
            tick2.fillAmount = (currentHealth - 25) /25;
        } else if (currentHealth <= 75)
        {
            tick3.fillAmount = (currentHealth - 50) /25;
        } else if (currentHealth <= 100)
        {
            tick4.fillAmount = (currentHealth - 75) /25;
        } else if (currentHealth <= 125)
        {
            tick5.fillAmount = (currentHealth - 100) /25;
        }

        if (currentMana <= 25)
        {
            tick6.fillAmount = (currentMana) /25;
        } else if (currentMana <= 50)
        {
            tick5.fillAmount = (currentMana - 25) /25;
        } else if (currentMana <= 75)
        {
            tick4.fillAmount = (currentMana - 50) /25;
        } else if (currentMana <= 100)
        {
            tick3.fillAmount = (currentMana - 75) /25;
        } else if (currentMana <= 125)
        {
            tick2.fillAmount = (currentMana - 100) /25;
        }
        */
        if (frames % 3 == 0)
        {
            List<Image> healthTickList = new List<Image>();
            List<Image> manaTickList = new List<Image>();

            if (healthTicks >= 1) healthTickList.Add(tick1);
            if (healthTicks >= 2) healthTickList.Add(tick2);
            if (healthTicks >= 3) healthTickList.Add(tick3);
            if (healthTicks >= 4) healthTickList.Add(tick4);
            if (healthTicks >= 5) healthTickList.Add(tick5);

            if (manaTicks >= 1) manaTickList.Add(tick6);
            if (manaTicks >= 2) manaTickList.Add(tick5);
            if (manaTicks >= 3) manaTickList.Add(tick4);
            if (manaTicks >= 4) manaTickList.Add(tick3);
            if (manaTicks >= 5) manaTickList.Add(tick2);

            for (int i = 0; i < healthTickList.Count; i++)
            {
                healthTickList[i].fillAmount = currentHealth >= (i + 1) * 25 ? 1.0f : Mathf.Max(currentHealth - 25 * i, 0) / 25.0f;
                // Also set bar color here
                healthTickList[i].color = Color.red;
            }

            for (int i = 0; i < manaTickList.Count; i++)
            {
                manaTickList[i].fillAmount = currentMana >= (i + 1) * 25 ? 1.0f : Mathf.Max(currentMana - 25 * i, 0) / 25.0f;
                // Also set bar color here
                manaTickList[i].color = Color.blue;
            }

        }


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
                     thisSource.outputAudioMixerGroup = Stealthmaster;
                    break;
                case 1:
                    swap.DoSwap();
                    thisSource.outputAudioMixerGroup = player;
                    break;
                case 2:
                    if (PlayerController.instance.status == Status.crouching)
                    {
                        stealthGrapple.GrappleCheck();   
                    } else if(PlayerController.instance.status != Status.grappling)
                     {
                         PlayerController.instance.SetUpGrapple();
                         thisSource.clip = playerClips[3];
                         thisSource.PlayOneShot(playerClips[3]);
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
        if (currentHealth <= 0)
        {
            thisSource.PlayOneShot(playerClips[4]);
            SceneManager.LoadScene(myScene);
        }
        //round up
        //manaTicks = (int)Mathf.Ceil(currentMana / 25);
        //healthTicks = (int)Mathf.Ceil(currentHealth / 25);

    }
    public void GetHurt(float damageValue)
    {
        currentHealth -= damageValue;
        thisSource.PlayOneShot(playerClips[4]);
        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(myScene);
        }
        //healthTicks = (int)Mathf.Ceil(currentHealth / 25);
    }


    public void EctoStim(float ectoAmount)
    {
        currentMana += ectoAmount;
        if (currentMana > maxMana)
        {
            //Ticks
            manaTicks = manaTicks + 1;
            healthTicks = healthTicks - 1;
            maxMana = manaTicks * 25;
            currentMana = manaTicks * 25;
            maxHealth = healthTicks * 25;
            //float diff = currentMana - maxMana;
            //maxHealth -= diff;
            if (maxHealth < currentHealth)
            {
                currentHealth = maxHealth;
            }
            //maxMana = currentMana;           
        } //if not a multiple of 25, increase mana to next tick value
        else if (currentMana % 25 != 0)
        {
            currentMana = Mathf.Ceil(currentMana/25) *25;
        }
    }

    public void HealthStim(float healthAmount)
    {
        currentHealth += healthAmount;
        if (currentHealth > maxHealth)
        {
            //Ticks
            manaTicks = manaTicks - 1;
            healthTicks = healthTicks + 1;
            maxHealth = healthTicks * 25;
            currentHealth = healthTicks * 25;
            maxMana = manaTicks * 25;
            //float diff = currentHealth - maxHealth;
            //maxMana -= diff;
            if (maxMana < currentMana)
            {
                currentMana = maxMana;
            }
            //maxHealth = currentHealth;
        }
        else if (currentHealth % 25 != 0)
        {
            currentHealth = Mathf.Ceil(currentHealth/25) * 25;
        }
    }
    public IEnumerator HurtEffect()
    {
        bloodied = true;
        bloodEffect.SetActive(true);
        bloodEffect.transform.localEulerAngles += new Vector3(0,0,180);
        Color temp = bloodEffect.GetComponent<Image>().color;
        temp.a = .75f;
        yield return 0;
        bloodied = false;
        float t = 0;
        while (t < 1)
        {
           temp.a = Mathf.LerpUnclamped(.75f, 0, linearCurve.Evaluate(t));
            bloodEffect.GetComponent<Image>().color = temp;
            t += Time.deltaTime;
            yield return 0;
            if (bloodied)
            {
                break;
            }
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
            inVignette = false;
            vignette.active = false;
            yield return 0;
        }
        inVignette = true;

        
     
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
