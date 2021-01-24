using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsScrip : MonoBehaviour
{
    public AudioClip[] footstepClips;

    private AudioSource thisSource;

    private PlayerController _playerController;
    // Start is called before the first frame update
    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        thisSource = this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerController.movement.grounded && _playerController.status == Status.moving &&
            thisSource.isPlaying == false)
        {
            Step();
            Debug.Log("Walking");
        }
        if (_playerController.status == Status.wallRunning  &&  thisSource.isPlaying == false)
        {
            Step();
            Debug.Log("Wallrunnin");
        }
        
    }

    void Step()
    {
        thisSource.volume = Random.Range(.7f, 1f);
        thisSource.pitch = Random.Range(.8f, 1.1f);
        //thisSource.clip = footstepClips[0];
        //thisSource.PlayOneShot(footstepClips[0]);
    }
}
