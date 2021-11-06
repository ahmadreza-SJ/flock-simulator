using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Generator : MonoBehaviour
{
    public GameObject DronePrefab;
    public CinemachineVirtualCamera VCam;

    public int Count;
    public float Offset;

    private List<GameObject> drones;

    public List<GameObject> Drones => drones;

    // Start is called before the first frame update
    void Start()
    {
        drones = new List<GameObject>();
        int rowCount = (int)Mathf.Sqrt(Count);
        for(int i = 0; i < Count; i++)
        {
            GameObject go = Instantiate(DronePrefab, new Vector3((i / rowCount) * Offset, (i % rowCount) * Offset), DronePrefab.transform.rotation);
            drones.Add(go);
            go.transform.parent = this.gameObject.transform;
        }
        SetVCam(drones[0]);
    }

    public void SetVCam(GameObject drone)
    {
        VCam.Follow = drone.transform;
    }
    
}
