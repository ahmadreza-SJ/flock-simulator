using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class DroneInfoMonitor : MonoSingleton<DroneInfoMonitor>
{
    public ValueField QPValueField;
    public ValueField CollideValueField;
    public GameObject CenterMarkerPrefab;
    public GameObject InnerTerritoryVisualizerPrefab;
    public GameObject OuterTerritoryVisualizerPrefab;
    public GameObject Obstacle;


    /// <summary>
    /// Its value is 2* RealCollidesCount cause in each collide both drones add to this var
    /// </summary>
    private int DetectedDroneCollides;
    private int DetectedObstacleCollides;

    public int DetectedCollides => (DetectedObstacleCollides + DetectedDroneCollides / 2);

    private GameObject CenterMarker;
    private GameObject InnerTerritoryVisualizer;
    private GameObject OuterTerritoryVisualizer;

    public void ReportCollide()
    {
        DetectedDroneCollides += 1;
    }

    public void ReportObstacleCollide()
    {
        DetectedObstacleCollides += 1;
        CollideValueField.SetValue(DetectedCollides);
    }

    private DroneController monitoringDrone;

    public DroneController MonitoringDrone => monitoringDrone;

    // Start is called before the first frame update
    void Start()
    {
        QPValueField.gameObject.SetActive(false);
        DetectedDroneCollides = 0;
        DetectedObstacleCollides = 0;
    }

    // Update is called once per frame
    void Update()
    {
        QPValueField.SetValue(monitoringDrone?.QPValue);
        
    }

    public void ShowDetailes(DroneController drone)
    {
        //monitoringDrone?.HideNeighboursNet();
        QPValueField.gameObject.SetActive(true);
        drone.ShowNeighboursNet();
        monitoringDrone = drone;
        StartCoroutine(UpdateFlockCenter());
    }

    private IEnumerator UpdateFlockCenter()
    {

        while(true)
        {
            SetFlockCenter();
            SetTerritoryVisualizer();
            yield return new WaitForEndOfFrame();
        }
    }

    private void SetFlockCenter()
    {

        if (CenterMarker == null)
        {
            CenterMarker = Instantiate(CenterMarkerPrefab, this.transform);
        }

        CenterMarker.transform.position = monitoringDrone.flockCenter;
    }

    private void SetTerritoryVisualizer()
    {
        if(InnerTerritoryVisualizer == null)
        {
            InnerTerritoryVisualizer = Instantiate(InnerTerritoryVisualizerPrefab, this.transform);
        }

        if (OuterTerritoryVisualizer == null)
        {
            OuterTerritoryVisualizer = Instantiate(OuterTerritoryVisualizerPrefab, this.transform);
        }

        InnerTerritoryVisualizer.transform.position = MonitoringDrone.flockCenter;

        OuterTerritoryVisualizer.transform.position = MonitoringDrone.flockCenter;
        InnerTerritoryVisualizer.transform.localScale = MonitoringDrone.InnerTerritoryRadius * Vector3.one * 2;
        OuterTerritoryVisualizer.transform.localScale = MonitoringDrone.OuterTerritoryRadius * Vector3.one * 2;
    }

    public void updateTerritoryRadius()
    {
        if (MonitoringDrone != null)
        {
            InnerTerritoryVisualizer.transform.localScale = MonitoringDrone.InnerTerritoryRadius * Vector3.one * 2;
            OuterTerritoryVisualizer.transform.localScale = MonitoringDrone.OuterTerritoryRadius * Vector3.one * 2;
        }
    }
}
