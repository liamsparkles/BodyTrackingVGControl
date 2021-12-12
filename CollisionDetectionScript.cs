using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollisionDetectionScript : MonoBehaviour
{
    poseDetectionSocket myPose;
    DynamicTest dt;
    public GameObject Score;
    Points myPoints;
    public TextMeshProUGUI DeathText;
    public GameObject BoundsText;
   
    // Start is called before the first frame update
    void Start()
    {
        myPose = GameObject.Find("Main Controller").GetComponent<poseDetectionSocket>();
        dt = GameObject.Find("ObstacleGeneration").GetComponent<DynamicTest>();
        myPoints = Score.GetComponent<Points>();
    }

    // Update is called once per frame
    void Update()
    {
      

        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacle")
        {
            print("you collided w the obstacle loser kk");
            dt.gameMode = Modes.Waiting;
            myPose.setDeath(true);
            DeathText.text= "You Died, Restart Game by raising both your hands \n SCORE WAS : "+myPoints.score;
        }

        if (other.tag == "Bounds")
        {
            
            print("Stay within bounds");
            BoundsText.SetActive(true);
            Invoke("StopDisplayBound", 3f);
        
        }



    }


    void StopDisplayBound()
    {
        BoundsText.SetActive(false);
    }

}
