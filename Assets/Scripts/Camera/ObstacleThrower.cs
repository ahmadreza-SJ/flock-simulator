using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleThrower : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ObstaclePrefab;
    public float Speed;
    void Start()
    {
        //StartCoroutine(autoThrower(5));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            throwObstacle();
        }
    }

    private void throwObstacle()
    {
        GameObject obstacle = Instantiate(ObstaclePrefab);
        obstacle.transform.localPosition = this.gameObject.transform.position;
        obstacle.GetComponent<Rigidbody>().AddForce(Speed * this.transform.forward);
    }

    private IEnumerator autoThrower(int interval)
    {
        while(true)
        {
            yield return new WaitForSeconds(interval);
            throwObstacle();
        }
    }
}
