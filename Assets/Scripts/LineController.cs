using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private GameObject Drone1;
    private GameObject Drone2;

    private LineRenderer lineRenderer;

    public void Init(GameObject drone1, GameObject drone2)
    {
        Drone1 = drone1;
        Drone2 = drone2;
        lineRenderer = GetComponent<LineRenderer>();

        Vector3[] pos = { Drone1.transform.position, Drone2.transform.position };
        lineRenderer.SetPositions(pos);
        this.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] pos = { Drone1.transform.position, Drone2.transform.position };
        lineRenderer.SetPositions(pos);
    }
}
