using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : MonoBehaviour
{
    public float deleteDepth = -5.0f;
    Points myPoints;
    DynamicTest myObstacles;
    // Start is called before the first frame update
    void Start()
    {
        myPoints = GameObject.Find("Points").GetComponent<Points>();
        myObstacles = GameObject.Find("ObstacleGeneration").GetComponent<DynamicTest>();

    }

    // Update is called once per frame
    void Update()
    {
        float speed = myObstacles.obstacleSpeed;
        transform.position -= new Vector3(0, 0, speed*Time.deltaTime);

        if (transform.position.z <= deleteDepth)
        {
            myPoints.AddPoints();
            Destroy(gameObject);
        }
    }
}
