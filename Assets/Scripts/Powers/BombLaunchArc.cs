using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombLaunchArc : MonoBehaviour
{
    LineRenderer lr;

    public float velocity;
    public float angle;
    public int resolution = 100;
    public float maxArcTime = 4.0f;
    public Transform hand;

    float g; //force of gravity on y
    float radianAngle;
    private void Awake()
    {
            lr = GetComponent<LineRenderer>();

            Color start = Color.cyan;
            Color end = Color.red;

            start.a = 0.0f;
            end.a = 1.0f;

            lr.startColor = start;
            lr.endColor = end;

            lr.startWidth = 0.10f;
            lr.endWidth = .10f;

            g = Mathf.Abs(Physics.gravity.y*2);
        
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            RenderArc();
        } else
        {
            lr.enabled = false;
        }


    }

    //populate 
    void RenderArc()
    {
        lr.enabled = true;
        angle = CalculateThrowAngle(Player_Controller.instance.currentY);
        Vector3[] arc = CalculateArcArray();

        lr.SetVertexCount(arc.Length);
        lr.SetPositions(arc);
    }

    Vector3[] CalculateArcArray()
    {
        List<Vector3> arcArray = new List<Vector3>();
        radianAngle = Mathf.Deg2Rad * angle;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i * maxArcTime / (float)resolution;
            Vector3 point = CalculateArcPoint(t);
            // Trajectory arc is drawn for maxArcTime seconds of flight time
            // You can look to see if point is out of bounds here (below floor, in wall, etc) and break if no more calculations needed
            // Accuracy of check is dependent on resolution and maxArxTime settings
            // e.g. This stops calculation after going through bottom floot which is about y = -26
            // if (point.y < -30)
            //     break;

            arcArray.Add(point);
        }
        return arcArray.ToArray();
    }

    //calculate height and distance of each vertex
    Vector3 CalculateArcPoint(float t)
    {
        float x = velocity * Mathf.Cos(radianAngle) * t;
        float y = velocity * Mathf.Sin(radianAngle) * t - (g * t * t / 2.0f);

        Vector3 point = new Vector3(hand.position.x, hand.position.y + y, hand.position.z) + Player_Controller.instance.gameObject.transform.forward * x;
        //        point = new Vector3(point.x, y + hand.position.y, point.z);
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

  /*  Vector3 getPointFromTime(float t)
    {
        int lowerBound = (int)Mathf.Floor(t / timePerPoint);
        int upperBount = lowerBound + 1;

        if (lowerBound < 0 || upperBount > arc.Length)
        {
            Debug.Log("Error: Calculating arc beyond bounds");
            return new Vector3(0, 0, 0);
        }

        return Vector3.Lerp(arc[lowerBound], arc[upperBount], t - lowerBound * timePerPoint);
    }*/
}
