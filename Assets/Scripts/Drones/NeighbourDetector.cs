using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighbourDetector : MonoBehaviour
{
    public List<GameObject> neighbours;
    public List<GameObject> obstacles;
    public List<GameObject> dangerousObstacles;

    public List<GameObject> detectedObjects = new List<GameObject>();
    private SphereCollider sphereCollider;
    private GameObject ObstacleCopiesParent;

    public Action OnChangeNeighbours;
    public Action OnChangeObstacles;
    public Action OnChangeDangerousObstacles;


    private void Start()
    {
        
    }

    public void InitCollider(float da)
    {
        ObstacleCopiesParent = new GameObject("ObstacleCopiesParent");
        neighbours = new List<GameObject>();
        obstacles = new List<GameObject>();
        dangerousObstacles = new List<GameObject>();
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.center = Vector3.zero;
        sphereCollider.radius = da;
    }

    

    public void InspectTerritory(Vector3 flockCenter, float innerBound, float outerBound, float obstacleMaxVelocity)
    {

        foreach (GameObject obj in detectedObjects)
        {
            if ((obj.transform.position - flockCenter).magnitude >= outerBound)
            {
                neighbours.Remove(obj);
                OnChangeNeighbours?.Invoke();
            }

        }
        

        foreach (GameObject obj in detectedObjects)
        {
            bool objProcessFinished = false; 
            foreach(GameObject obstacle in obstacles)
            {
                //if (obj.name == "Obstacle(Clone)" && obstacle.name == "Obstacle(Clone)" && (obj.transform.position - obstacle.transform.position).magnitude >= obstacleMaxVelocity * Time.deltaTime)
                //{
                //    Debug.Log("Actual Velocity: " + (obj.transform.position - obstacle.transform.position).magnitude + " - Expected Max: " + obstacleMaxVelocity * Time.deltaTime);
                //}
                if ((obj.transform.position - obstacle.transform.position).magnitude < obstacleMaxVelocity * Time.deltaTime)
                {
                    UpdateCopiedPosition(obstacle, obj);
                    if((obj.transform.position - flockCenter).magnitude < innerBound)
                    {
                        if(!dangerousObstacles.Contains(obstacle))
                        {
                            dangerousObstacles.Add(obstacle);
                            OnChangeNeighbours?.Invoke();
                        }
                    }
                    if ((obj.transform.position - flockCenter).magnitude > innerBound)
                    {
                        dangerousObstacles.Remove(obstacle);
                        OnChangeNeighbours?.Invoke();
                    }
                    //if (obj.name == "Obstacle(Clone)" && obstacle.name == "Obstacle(Clone)" && (obj.transform.position - flockCenter).magnitude >= outerBound)
                    //{
                    //    Debug.Log("Actual Dist: " + (obj.transform.position - flockCenter).magnitude + " - Expected Max: " +outerBound);
                    //}
                    if ((obj.transform.position - flockCenter).magnitude >= outerBound)
                    {
                        obstacles.Remove(obstacle);
                        Destroy(obstacle);
                        OnChangeNeighbours?.Invoke();
                    }
                    objProcessFinished = true;
                    break;
                }
            }

            if(objProcessFinished)
            {
                continue;
            }

            if ((obj.transform.position - flockCenter).magnitude > innerBound && (obj.transform.position - flockCenter).magnitude < outerBound)
            {
                obstacles.Add(CopyPosition(obj));
                OnChangeNeighbours?.Invoke();
            }
            else if ((obj.transform.position - flockCenter).magnitude <= innerBound)
            {
                if (!neighbours.Contains(obj))
                {
                    neighbours.Add(obj);
                    OnChangeNeighbours?.Invoke();
                }
            }
        }
    }

    private GameObject CopyPosition(GameObject go)
    {
        GameObject newGo = new GameObject(go.name);
        newGo.transform.position = go.transform.position;
        newGo.transform.parent = ObstacleCopiesParent.transform;
        return newGo;
    }

    private void UpdateCopiedPosition(GameObject oldPos, GameObject newPos)
    {
        oldPos.transform.position = newPos.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if((other.gameObject.tag == "Drone" || other.gameObject.tag == "Obstacle") && other.gameObject != this.gameObject.transform.parent.gameObject)
        {
            detectedObjects.Add(other.gameObject);
        }
        OnChangeNeighbours?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Drone" || other.gameObject.tag == "Obstacle")
        {
            detectedObjects.Remove(other.gameObject);
        }
        OnChangeNeighbours?.Invoke();
    }
}
