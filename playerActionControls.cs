using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerActionControls : MonoBehaviour
{
    public poseDetectionSocket ps;
    Vector3 avgleft1;
    Vector3 avgright1;
    Vector3 avgLeftLeg1;
    Vector3 avgRightLeg1;
    Vector3 avgHead1;
    Vector3 avgBody1;



    //float holdDur = 3f;
    //float timer1;
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        //    avgleft1 = gameObject.GetComponent<poseDetectionSocket>().avgleft;
        //    avgright1 = gameObject.GetComponent<poseDetectionSocket>().avgright;
        //    avgLeftLeg1 = gameObject.GetComponent<poseDetectionSocket>().avgLeftLeg;
        //    avgRightLeg1 = gameObject.GetComponent<poseDetectionSocket>().avgRightLeg;
        //    avgHead1 = gameObject.GetComponent<poseDetectionSocket>().avgHead;
        //    avgBody1 = gameObject.GetComponent<poseDetectionSocket>().avgBody;

        //    print(avgleft1 + "");

        //    if (avgleft1.y > avgHead1.y && avgright1.y > avgHead1.y)
        //    {
        //        timer1 += Time.deltaTime;

        //        print("timer is " + timer1);

        //        if (timer1 > holdDur)
        //        {
        //            //perform your action
        //            print("hands up  for 3x seconds");
        //            timer1 = 0;
        //        }
        //    }
        //else if (avgleft1.y > avgHead1.y && avgright1.y > avgHead1.y && avgright1.x > avgleft1.x) {

        //    timer1 += Time.deltaTime;

        //    print("hands up cross " + timer1);

        //    if (timer1 > holdDur)
        //    {
        //        //perform your action
        //        print("hands up  for 3x seconds");
        //        timer1 = 0;
        //    }
        //}
        //else if (avgleft1.x > avgBody1.x && avgright1.x > avgBody1.x)
        //{

        //    timer1 += Time.deltaTime;

        //    print("pointing right " + timer1);

        //    if (timer1 > holdDur)
        //    {
        //        //perform your action
        //        print("hands up  for 3x seconds");
        //        timer1 = 0;
        //    }
        //}
        //else if (avgleft1.x < avgBody1.x && avgright1.x < avgBody1.x)
        //{

        //    timer1 += Time.deltaTime;

        //    print("pointing left " + timer1);

        //    if (timer1 > holdDur)
        //    {
        //        //perform your action
        //        print("hands up  for 3x seconds");
        //        timer1 = 0;
        //    }
        //}
        //    else
        //    {
        //        timer1 = 0;
        //    }
        //}
    }
}
