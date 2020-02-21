using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScatterBomb : MonoBehaviour
{
    int layerMask = 1 << 11;
    float g;

    private bool go;

    float elapsedTime = 0;

    Vector3 _initialPos;
    Vector3 _forward;
    float _radianAngle;
    float _velocity;

    private void Awake()
    {
        go = false;
        g = Mathf.Abs(Physics.gravity.y*2);
        elapsedTime = 0;
        this.GetComponent<SphereCollider>().enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void setParams(Vector3 initialPos, Vector3 forward, float viewAngle, float velocity)
    {
        _initialPos = initialPos;
        _forward = forward;
        _radianAngle = Mathf.Deg2Rad * CalculateThrowAngle(viewAngle);
        _velocity = velocity;

        transform.position = CalculateArcPoint(0);
    }

    public void Go()
    {
        go = true;
        this.GetComponent<SphereCollider>().enabled = true;
    }

    public void Stop()
    {
        go = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (go == true)
        {
            elapsedTime += Time.deltaTime;
            transform.position = CalculateArcPoint(elapsedTime);
        }
    }


    Vector3 CalculateArcPoint(float t)
    {
        float x = _velocity * Mathf.Cos(_radianAngle) * t;
        float y = _velocity * Mathf.Sin(_radianAngle) * t - (g * t * t / 2.0f);

        Vector3 point = new Vector3(_initialPos.x, _initialPos.y + y, _initialPos.z) + _forward * x;

        return point;
    }

    float CalculateThrowAngle(float viewAngle)
    {
        float min = -90;
        float max = 90;

        float offset = 45;

        float throwAngle;
        if (viewAngle >= 0)
            throwAngle = ((max - offset) * viewAngle / max) + offset;
        else
            throwAngle = ((min - offset) * viewAngle / min) + offset;

        return throwAngle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 15, layerMask);
        Debug.Log(hitColliders.Length);
        foreach (var NPC in hitColliders)
        {

            float variable = Random.Range(-5, 5);
            Vector3 dir = (NPC.transform.position - new Vector3(transform.position.x + variable,
                NPC.transform.position.y, transform.position.z + variable)).normalized;
            if (NPC.GetComponent<AINav>() != null)
            {
                NPC.GetComponent<AINav>().Reposition(NPC.transform.position + (dir * 15f));
            }
        }

       Destroy(gameObject);
    }
}
