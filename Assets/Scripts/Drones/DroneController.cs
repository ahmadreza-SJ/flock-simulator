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
    public int NumberOfGuesses;

    public float QPValue;

    private NeighbourDetector neighbourDetector;
    private Rigidbody droneRigidbody;
    //private Vector3 currentVelocity;
    



    public List<GameObject> GetNeighbours()
    {
        return neighbourDetector.neighbours;
    }


    private Vector3 GetRandomDirection()
    {
        Vector3 dir = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
        return dir / dir.magnitude;
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

    private float CollideBarrierFunction(List<GameObject> neighbours, Vector3 v)
    {
        float maxValue = float.MinValue;
        foreach (GameObject neighbour in neighbours)
        {
            float distance = GetRelativePosition(neighbour).magnitude;
            float heuristic = Vector3.Dot(v, GetRelativePosition(neighbour)) - Kc * (distance * distance - Dc * Dc);
            if (heuristic > 0)
            {
                maxValue = Mathf.Max(maxValue, ColliderBarrierDiff * heuristic);
            }
        }
        return Mathf.Max(0f, maxValue);
    }

    private float GetQPValue(List<GameObject> neighbours, Vector3 v)
    {
        //Debug.Log(GetSigma(neighbours, v));
        return Vector3.Dot(v, v) + Mathf.Pow(GetSigma(neighbours, v), 2) + CollideBarrierFunction(neighbours, v);
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
        InitNeighboursNetVisualizer();

    }

    public void HideNeighboursNet()
    {
        neighbourDetector.OnChangeNeighbours -= InitNeighboursNetVisualizer;
    }

    private void InitNeighboursNetVisualizer()
    {
        NetworkVisualizer.Instance.NeighboursNet(this);
    }

    private void Start()
    {
        droneRigidbody.velocity = 10 * GetRandomDirection();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v = CalculateVelocity();
        droneRigidbody.velocity = v;
    }

    void OnMouseDown()
    {
        // this object was clicked - do something
        DroneInfoMonitor.Instance.ShowDetailes(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Drone")
        {
            DroneInfoMonitor.Instance.ReportCollide();
            Debug.Log("Collided");
        }
    }
}
