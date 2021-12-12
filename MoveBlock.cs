using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : MonoBehaviour
{
    public float deleteDepth = -5.0f;
    Points myPoints;
    DynamicTest myObstacles;
    poseDetectionSocket myPose;
    // Start is called before the first frame update
    void Start()
    {
        myPoints = GameObject.Find("Score").GetComponent<Points>();
        myObstacles = GameObject.Find("ObstacleGeneration").GetComponent<DynamicTest>();
        myPose = GameObject.Find("Main Controller").GetComponent<poseDetectionSocket>();
    }

    // Update is called once per frame
    void Update()
    {
        print(myPose.ToString());
        if (myPose.getDeath())
        {
            Destroy(gameObject);
        }

        float speed = myObstacles.obstacleSpeed;
        transform.position -= new Vector3(0, 0, speed * Time.deltaTime);

        if (transform.position.z <= deleteDepth)
        {
            myPoints.AddPoints();
            Destroy(gameObject);
        }
    }
}