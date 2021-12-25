using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraController : MonoBehaviour
{
    public float MoveSpeed;
    public float RotateSpeed;
    public Vector3 turn = Vector3.zero;

    private CinemachineVirtualCamera vCam;
    // Start is called before the first frame update
    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            this.gameObject.transform.position += this.gameObject.transform.forward * MoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.gameObject.transform.position -= this.gameObject.transform.right * MoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.gameObject.transform.position -= this.gameObject.transform.forward * MoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.gameObject.transform.position += this.gameObject.transform.right * MoveSpeed * Time.deltaTime;
        }
        //this.gameObject.transform.rotation.Set(this.gameObject.transform.rotation.x + Input.GetAxis("Mouse X") * RotateSpeed * Time.deltaTime,
        //    this.gameObject.transform.rotation.y + Input.GetAxis("Mouse Y") * RotateSpeed * Time.deltaTime,
        //    this.gameObject.transform.rotation.z, this.gameObject.transform.rotation.w);


        turn.x += Input.GetAxis("Mouse X") * RotateSpeed;
        turn.y += Input.GetAxis("Mouse Y") * RotateSpeed;
        this.gameObject.transform.rotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }
}
