using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Rigidbody rigidbody;

    public float Speed;

    private Vector3 last = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log((this.transform.position - last).magnitude);
        last = this.transform.position;
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    rigidbody.AddForce(Speed * new Vector3(0, 1, 1));
        //    rigidbody.useGravity = true;
        //}
    }
}
