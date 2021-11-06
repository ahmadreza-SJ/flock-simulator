using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighbourDetector : MonoBehaviour
{
    public List<GameObject> neighbours;

    private SphereCollider sphereCollider;

    public Action OnChangeNeighbours;


    

    public void InitCollider(float da)
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.center = Vector3.zero;
        sphereCollider.radius = da;
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Drone" && other.gameObject != this.gameObject.transform.parent.gameObject)
        {
            neighbours.Add(other.gameObject);
        }
        OnChangeNeighbours?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Drone")
        {
            neighbours.Remove(other.gameObject);
        }
        OnChangeNeighbours?.Invoke();
    }
}
