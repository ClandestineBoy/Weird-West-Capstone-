using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class rope : MonoBehaviour
{
    public Transform bob;
    public Transform target1;
    private Transform curTarget;
    LineRenderer lr;
    private bool isTarget1 = true;
    public bool hasParent;

    // Use this for initialization
    void Start()
    {

        lr = GetComponent<LineRenderer>();
        if (hasParent)
        {
            lr.SetPosition(1, transform.InverseTransformPoint(bob.position));
        }
        else
        {
            lr.SetPosition(1, bob.position);
        }

        curTarget = target1;

    }

    // Update is called once per frame
    void Update()
    {

        if (SwingController.instance.state == SwingController.State.Swinging)
        {
            lr.enabled = true;
        }
        else
            lr.enabled = false;

        lr = GetComponent<LineRenderer>();
        if (hasParent)
        {
            lr.SetPosition(1, transform.InverseTransformPoint(bob.position));
        }
        else
        {
            lr.SetPosition(1, bob.position);
        }

        if (hasParent)
        {
            lr.SetPosition(0, transform.InverseTransformPoint(curTarget.position));
        }
        else
        {
            lr.SetPosition(0, curTarget.position);
        }

    }

}
