using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FallReset : MonoBehaviour
{
    Vector3 resetPoint;
    public float fallLimit;
    void Start()
    {
        resetPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y <= fallLimit)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
