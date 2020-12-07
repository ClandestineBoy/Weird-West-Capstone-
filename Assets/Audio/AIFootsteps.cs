using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFootsteps : MonoBehaviour
{
    public AudioClip[] AIClips;

    private AudioSource thisSource;

    private EnemyAI _enemyAi;

    private AINav _aiNav;
    // Start is called before the first frame update
    void Start()
    {
        _enemyAi = GetComponent<EnemyAI>();
        _aiNav = GetComponent<AINav>();
        thisSource = GetComponent<AudioSource>();
    }

    void PlaySound()
    {
        thisSource.volume = Random.Range(.7f, 1f);
        thisSource.pitch = Random.Range(.8f, 1.1f);
         thisSource.clip = AIClips[0];
         thisSource.PlayOneShot(AIClips[0]);

    }



    // Update is called once per frame
    void Update()
    {
        if (!thisSource.isPlaying && !_aiNav.agent.isStopped)
        {
          //  PlaySound();
        }

        if ((!thisSource.isPlaying && _aiNav.animator.GetBool("isWalking")))
        {
            PlaySound();
        }
    }
}
