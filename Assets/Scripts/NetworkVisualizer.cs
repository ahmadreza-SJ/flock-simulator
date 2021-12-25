using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class NetworkVisualizer : MonoSingleton<NetworkVisualizer>
{
    public GameObject NormalLinePrefab;
    public GameObject WarningLinePrefab;
    public GameObject DangerousLinePrefab;
    public Generator Generator;

    private List<GameObject> lines;
    private int droneIndex = 0;

    private void Start()
    {

        lines = new List<GameObject>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            DroneInfoMonitor.Instance.ShowDetailes(Generator.Drones[droneIndex].GetComponent<DroneController>());
            droneIndex = (droneIndex + 1) % Generator.Drones.Count;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DroneInfoMonitor.Instance.ShowDetailes(Generator.Drones[droneIndex].GetComponent<DroneController>());
            droneIndex = droneIndex - 1;
            if(droneIndex < 0)
            {
                droneIndex += Generator.Drones.Count;
            }
        }
    }

    //public void WholeNet()
    //{
    //    RemoveLines();
    //    List<GameObject> drones = Generator.Drones;
    //    for(int i = 0; i < drones.Count; i++)
    //    {
    //        for(int j = i + 1; j < drones.Count; j++)
    //        {
    //            CreateLine(drones[i], drones[j]);
    //        }
    //    }
    //}

    public void NeighboursNet(DroneController drone)
    {
        RemoveLines();
        List<GameObject> drones = drone.GetNeighbours();
        List<GameObject> obstacles = drone.GetObstacles();
        List<GameObject> dangerousObstacles = drone.GetDangerousObstacles();
        for (int i = 0; i < drones.Count; i++)
        {
            CreateLine(drone.gameObject, drones[i], NormalLinePrefab);
        }

        foreach(GameObject ob in obstacles)
        {
            if(dangerousObstacles.Contains(ob))
            {
                CreateLine(drone.gameObject, ob, DangerousLinePrefab);
            }
            else
            {
                CreateLine(drone.gameObject, ob, WarningLinePrefab);
            }
        }
    }

    private void RemoveLines()
    {
        for(int i = 0; i < lines.Count; i++)
        {
            Destroy(lines[i]);
        }
        lines.Clear();
    }

    private void CreateLine(GameObject drone1, GameObject drone2, GameObject linePrefab)
    {
        GameObject line = Instantiate(linePrefab, this.gameObject.transform);
        line.GetComponent<LineController>().Init(drone1, drone2);
        lines.Add(line);
    }
}
