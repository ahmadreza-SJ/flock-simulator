using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class NetworkVisualizer : MonoSingleton<NetworkVisualizer>
{
    public GameObject LinePrefab;
    public Generator Generator;

    private List<GameObject> lines;

    private void Start()
    {

        lines = new List<GameObject>();
    }

    public void WholeNet()
    {
        RemoveLines();
        List<GameObject> drones = Generator.Drones;
        for(int i = 0; i < drones.Count; i++)
        {
            for(int j = i + 1; j < drones.Count; j++)
            {
                CreateLine(drones[i], drones[j]);
            }
        }
    }

    public void NeighboursNet(DroneController drone)
    {
        RemoveLines();
        List<GameObject> drones = drone.GetNeighbours();
        for (int i = 0; i < drones.Count; i++)
        {
            CreateLine(drone.gameObject, drones[i]);
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

    private void CreateLine(GameObject drone1, GameObject drone2)
    {
        GameObject line = Instantiate(LinePrefab, this.gameObject.transform);
        line.GetComponent<LineController>().Init(drone1, drone2);
        lines.Add(line);
    }
}
