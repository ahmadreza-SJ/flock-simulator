using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVLogger : MonoBehaviour
{

    [SerializeField] private DroneInfoMonitor droneInfoMonitor;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(logData());
    }

    private IEnumerator logData()
    {
        while(true)
        {
            Debug.Log("Time: " + Time.time.ToString() + "   -   Collides: " + droneInfoMonitor.DetectedCollides.ToString());
            yield return new WaitForSeconds(60);
        }
    }
}
