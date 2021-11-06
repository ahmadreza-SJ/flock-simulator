using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class DroneInfoMonitor : MonoSingleton<DroneInfoMonitor>
{
    public ValueField QPValueField;
    public ValueField CollideValueField;


    /// <summary>
    /// Its value is 2* RealCollidesCount cause in each collide both drones add to this var
    /// </summary>
    private int DetectedCollides;

    public void ReportCollide()
    {
        DetectedCollides += 1;
        if(DetectedCollides % 2 == 0)
        {
            CollideValueField.SetValue(GetCollides());
        }
    }

    public int GetCollides()
    {
        return DetectedCollides / 2;
    }

    private DroneController monitoringDrone;

    public DroneController MonitoringDrone => monitoringDrone;

    // Start is called before the first frame update
    void Start()
    {
        QPValueField.gameObject.SetActive(false);
        DetectedCollides = 0;
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
    }
}
