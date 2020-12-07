using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDetectionplayer : MonoBehaviour
{
    public AudioClip[] AIClips;
    private AudioSource thisSource;
    private EnemyAI _enemyAi;

     void PlaySound()
    {
        thisSource.clip = AIClips[0];
        thisSource.PlayOneShot(AIClips[0]);

    }

    void Update()
    {
        if (!thisSource.isPlaying && _enemyAi.alerting)
        {
            PlaySound();
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        _enemyAi = GetComponent<EnemyAI>();
        thisSource = GetComponent<AudioSource>();
        
    }

}
