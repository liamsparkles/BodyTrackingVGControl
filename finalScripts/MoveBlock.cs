using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : MonoBehaviour
{
    public float deleteDepth = -5.0f;  //Distance to delete the objects
    Points myPoints; // points script, for modifying the score
    DynamicTest myObstacles; // obstacle creation script, for getting speed
    poseDetectionSocket myPose; // playaer script, for checking player game over
    // Start is called before the first frame update
    void Start()
    {
	//Set the scripts to their proper instance
        myPoints = GameObject.Find("Score").GetComponent<Points>();
        myObstacles = GameObject.Find("ObstacleGeneration").GetComponent<DynamicTest>();
        myPose = GameObject.Find("Main Controller").GetComponent<poseDetectionSocket>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myPose.getDeath())
        { //Delete objects if player dies
            Destroy(gameObject);
        }

	// Get obstacle speed
        float speed = myObstacles.obstacleSpeed;
        transform.position -= new Vector3(0, 0, speed * Time.deltaTime); //transform obstacle

        if (transform.position.z <= deleteDepth)
        { //When object passes player, delete it
            myPoints.AddPoints();
            Destroy(gameObject);
        }
    }
}
