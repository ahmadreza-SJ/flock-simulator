using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public float Da;
    public float Dc;
    public float Kc;
    public float Kp;
    public float ColliderBarrierDiff;
    public float MaxAcceleration;
    public float MaxVelocity;
    public float MinVelocity;
    public float InitialInnerTerritoryRadius;
    public float InitialOuterTerritoryRadius;
    public float InnerTerritoryRadiusOffset;
    public float OuterTerritoryWidth;
    public float OuterTerritoryRadius => outerTerritoryRadus;
    public float InnerTerritoryRadius => innerTerritoryRadus;
    public int NumberOfGuesses;
    public float ObstacleMaxVelocity;

    public float QPValue;
    public Vector3 flockCenter = Vector3.zero;

    private NeighbourDetector neighbourDetector;
    private Rigidbody droneRigidbody;
    private float innerTerritoryRadus;
    private float outerTerritoryRadus;
    //private Vector3 currentVelocity;



    private void Start()
    {
        droneRigidbody.velocity = 1 * GetRandomDirection();
        StartCoroutine(setBoundsRadius());
    }

    // Update is called once per frame
    void Update()
    {
        neighbourDetector.InspectTerritory(flockCenter, InnerTerritoryRadius, OuterTerritoryRadius, ObstacleMaxVelocity);
        Vector3 v = CalculateVelocity();
        droneRigidbody.velocity = v;
    }


    public List<GameObject> GetNeighbours()
    {
        return neighbourDetector.neighbours;
    }

    public List<GameObject> GetObstacles()
    {
        return neighbourDetector.obstacles;
    }

    public List<GameObject> GetDangerousObstacles()
    {
        return neighbourDetector.dangerousObstacles;
    }


    private Vector3 GetRandomDirection()
    {
        Vector3 dir = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
        return dir / dir.magnitude;
    }

    private IEnumerator setBoundsRadius()
    {
        innerTerritoryRadus = InitialInnerTerritoryRadius;
        outerTerritoryRadus = InitialOuterTerritoryRadius;
        yield return new WaitForSeconds(1);
        DroneInfoMonitor.Instance.updateTerritoryRadius();

        //while (true)
        //{
        //    List<GameObject> neighbours = GetNeighbours();
        //    float maxOffset = 0;
        //    foreach (GameObject neighbour in neighbours)
        //    {
        //        maxOffset = Mathf.Max(maxOffset, (neighbour.transform.position - flockCenter).magnitude);
        //    }
        //    maxOffset = Mathf.Max(maxOffset, (this.transform.position - flockCenter).magnitude);
        //    innerTerritoryRadus = maxOffset + InnerTerritoryRadiusOffset;
        //    outerTerritoryRadus = innerTerritoryRadus + OuterTerritoryWidth;
        //    DroneInfoMonitor.Instance.updateTerritoryRadius();
        //    yield return new WaitForEndOfFrame();
        //}
    }


    private List<Vector3> GetMoveGuesses()
    {
        List<Vector3> choices = new List<Vector3>();
        Vector3 rbCurrentVelocity = this.gameObject.GetComponent<Rigidbody>().velocity;
        for (int i = 0; i < NumberOfGuesses; i++)
        {
            Vector3 v = rbCurrentVelocity + (GetRandomDirection() * Random.Range(0, MaxAcceleration));
            choices.Add(v);
        }
        return choices;
    }

    private Vector3 CalculateVelocity()
    {
        List<GameObject> neighbours = GetNeighbours();
        Vector3 rbCurrentVelocity = this.gameObject.GetComponent<Rigidbody>().velocity;
        Vector3 finalChoose = this.gameObject.GetComponent<Rigidbody>().velocity;
        List<Vector3> velocityGuesses = GetMoveGuesses();
        foreach (Vector3 velocity in velocityGuesses)
        {
            if (rbCurrentVelocity.magnitude > MinVelocity && velocity.magnitude < MinVelocity)
            {
                continue;
            }


            if (velocity.magnitude > MaxVelocity)
            {
                continue;
            }

            float newQP = GetQPValue(neighbours, finalChoose);
            //if (DroneInfoMonitor.Instance.MonitoringDrone != null && DroneInfoMonitor.Instance.MonitoringDrone == this)
            //{
            //    Debug.Log(rbCurrentVelocity.magnitude);
            //}
            if (GetQPValue(neighbours, velocity) < newQP)
            {
                finalChoose = velocity;
                QPValue = newQP;
            }
        }
        return finalChoose;
    }

    private float CollideBarrierFunction(List<GameObject> neighbours, Vector3 v, float kc, float dc, float colliderBarrierDiff)
    {
        float maxValue = float.MinValue;
        foreach (GameObject neighbour in neighbours)
        {
            float distance = GetRelativePosition(neighbour).magnitude;
            float heuristic = Vector3.Dot(v, GetRelativePosition(neighbour)) - kc * (distance * distance - dc * dc);
            if (heuristic > 0)
            {
                maxValue = Mathf.Max(maxValue, colliderBarrierDiff * heuristic);
            }
        }
        return Mathf.Max(0f, maxValue);
    }

    private float GetQPValue(List<GameObject> neighbours, Vector3 v)
    {
        //Debug.Log(GetSigma(neighbours, v));
        return Vector3.Dot(v, v) + Mathf.Pow(GetSigma(neighbours, v), 2) + CollideBarrierFunction(neighbours, v, Kc, Dc, ColliderBarrierDiff)
            + CollideBarrierFunction(GetDangerousObstacles(), v, 5, 10, ColliderBarrierDiff);
    }

    private Vector3 GetRelativePosition(GameObject other)
    {
        return (other.transform.position - this.gameObject.transform.position);
    }

    private float GetSigma(List<GameObject> neighbours, Vector3 v)
    {
        Vector3 relPosSum = Vector3.zero;
        float relMoveSum = 0;
        foreach (GameObject neighbour in neighbours)
        {
            Vector3 pij = GetRelativePosition(neighbour);
            relPosSum += pij;
            relMoveSum += Vector3.Dot(pij, v);
        }
        flockCenter = (relPosSum / (neighbours.Count)) + gameObject.transform.position;
        return Mathf.Max(0, Kp * Mathf.Pow(relPosSum.magnitude, 2) - relMoveSum);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        neighbourDetector = GetComponentInChildren<NeighbourDetector>();
        neighbourDetector.InitCollider(Da);
        droneRigidbody = this.gameObject.GetComponent<Rigidbody>();
    }

    public void ShowNeighboursNet()
    {
        neighbourDetector.OnChangeNeighbours += InitNeighboursNetVisualizer;
        neighbourDetector.OnChangeObstacles += InitNeighboursNetVisualizer;
        neighbourDetector.OnChangeDangerousObstacles += InitNeighboursNetVisualizer;
        InitNeighboursNetVisualizer();

    }

    public void HideNeighboursNet()
    {
        neighbourDetector.OnChangeNeighbours -= InitNeighboursNetVisualizer;
        neighbourDetector.OnChangeObstacles -= InitNeighboursNetVisualizer;
        neighbourDetector.OnChangeDangerousObstacles -= InitNeighboursNetVisualizer;
    }

    private void InitNeighboursNetVisualizer()
    {
        NetworkVisualizer.Instance.NeighboursNet(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Drone")
        {
            DroneInfoMonitor.Instance.ReportCollide();
            Debug.Log("Collided");
        }

        if (other.gameObject.tag == "Obstacle")
        {
            DroneInfoMonitor.Instance.ReportObstacleCollide();
            Debug.Log("Obstacle Collided");
        }
    }
}
