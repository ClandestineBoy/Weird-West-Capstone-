using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    public AnimationCurve animCurve;
    bool slashing;
    MeshRenderer mesh;
    BoxCollider box;
    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<BoxCollider>();
        mesh = GetComponent<MeshRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !slashing)
        {
            StartCoroutine(Slash());

        }
    }

    IEnumerator Slash()
    {
        box.enabled = true;
        mesh.enabled = true;
        slashing = true;
        float t = 0;
        Vector3 StartPos = transform.localPosition;
        Vector3 EndPos = transform.localPosition +new Vector3(.8f,0,0);
        while (t < 1)
        {
            transform.localPosition = Vector3.LerpUnclamped(StartPos, EndPos, animCurve.Evaluate(t));
            t += Time.deltaTime * 6;
            yield return 0;
        }
        transform.localPosition = StartPos;
        slashing = false;
        box.enabled = false;
        mesh.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            other.gameObject.GetComponent<AINav>().RagDoll();

            foreach (Rigidbody rb in other.GetComponent<AINav>().rbs)
            {
                rb.useGravity = true;
                //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                rb.AddForce(transform.right * 5, ForceMode.Impulse);
               
            }
        }
    }
}
