using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollisionDetectionScript : MonoBehaviour
{
    poseDetectionSocket myPose; // player model script
    DynamicTest dt; //obstacle creation script
    public GameObject Score; // game score
    Points myPoints;
    public TextMeshProUGUI DeathText;
    public GameObject BoundsText;
   
    // Start is called before the first frame update
    void Start()
    {
        myPose = GameObject.Find("Main Controller").GetComponent<poseDetectionSocket>(); //set player script
        dt = GameObject.Find("ObstacleGeneration").GetComponent<DynamicTest>(); //set obstacle generation script
        myPoints = Score.GetComponent<Points>(); //set score
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacle")
        { //Ends the game as the user collided with an obstacle
            print("you collided with the obstacle");
            dt.gameMode = Modes.Waiting;
            myPose.setDeath(true);
            DeathText.text= "You Died, Restart Game by raising both your hands \n SCORE WAS : "+myPoints.score;
        }

        if (other.tag == "Bounds")
        { //Prompts the user to stay within the game bounds
            print("Stay within bounds");
            BoundsText.SetActive(true);
            Invoke("StopDisplayBound", 3f); //stop prompt after a few seconds
        }
    }


    void StopDisplayBound()
    { //Stop the bounds prompt
        BoundsText.SetActive(false);
    }

}
