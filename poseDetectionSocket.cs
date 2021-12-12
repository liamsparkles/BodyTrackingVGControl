using UnityEngine;
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;

public class poseDetectionSocket : MonoBehaviour
{
    public GameObject LHand;
    public GameObject RHand;
    public GameObject LLeg;
    public GameObject RLeg;
    public GameObject Head;
    public GameObject Body;



    public const int NUMLANDMARKS = 33;
    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25003;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    Vector4[] receivedPositions = new Vector4[NUMLANDMARKS];
    bool running;
    //float timeCounter;

    //float holdDur = 1.5f;
    //float timer1;
    public Vector3 avgleft;
    public Vector3 avgright;
    public Vector3 avgLeftLeg;
    public Vector3 avgRightLeg;
    public Vector3 avgHead;
    public Vector3 avgBody;
    public float xoffset;
    public float yoffset;
    public bool calibrate = false;
    //private float calibrationTime = 10;
    public bool CalibratedAlready=false;

    public GameObject Canvas;
    public GameObject Score;
    public GameObject DeathText;

    public TextMeshProUGUI CalibrateText;

    Points myPoints;

    public PlayerStatus pStatus;

    DynamicTest dt;
    public float holdDur = 2f;
    float timer1;

    private void Update()
    {

        //Landmark positions 
        //Left hand 16 18 20 22
        //Right hand 15 17 19 21
        Vector4[] lefthandpositions = new Vector4[] { receivedPositions[16], receivedPositions[18], receivedPositions[20] };
        Vector4[] righthandpositions = new Vector4[] { receivedPositions[15], receivedPositions[17], receivedPositions[19] };
        Vector4[] leftLegposition = new Vector4[] { receivedPositions[28], receivedPositions[32], receivedPositions[30] };
        Vector4[] rightLegposition = new Vector4[] { receivedPositions[27], receivedPositions[29], receivedPositions[31] };
        Vector4[] headposition = new Vector4[] { receivedPositions[1], receivedPositions[2], receivedPositions[3], receivedPositions[4], receivedPositions[5], receivedPositions[6], receivedPositions[9], receivedPositions[10], receivedPositions[0] };
        Vector4[] body = new Vector4[] { receivedPositions[24], receivedPositions[23] };


        Vector4 avgleft = averageVector4(lefthandpositions);
        Vector4 avgright = averageVector4(righthandpositions);
        Vector4 avgLeftLeg = averageVector4(leftLegposition);
        Vector4 avgRightLeg = averageVector4(rightLegposition);
        Vector4 avgHead = averageVector4(headposition);
        Vector4 avgBody = averageVector4(body);

        avgleft.z = 0;
        avgright.z = 0;
        avgLeftLeg.z = 0;
        avgRightLeg.z = 0;
        avgHead.z = 0;
        avgBody.z = 0;

        LHand.transform.position = new Vector3(avgleft.x + xoffset, avgleft.y + yoffset, avgleft.z);
        RHand.transform.position = new Vector3(avgright.x + xoffset, avgright.y + yoffset, avgleft.z);
        RLeg.transform.position = new Vector3(avgLeftLeg.x + xoffset, avgLeftLeg.y + yoffset, avgLeftLeg.z);
        LLeg.transform.position = new Vector3(avgRightLeg.x + xoffset, avgRightLeg.y + yoffset, avgRightLeg.z);
        Head.transform.position = new Vector3(avgHead.x + xoffset, avgHead.y + yoffset, avgHead.z);
        Body.transform.position = new Vector3(avgBody.x + xoffset, avgBody.y + yoffset, avgBody.z);

        if (avgLeftLeg.w == 0 | avgRightLeg.w == 0)
        {
           // print("no feet in frame");
        }
        else if (avgLeftLeg.w < 0.7 | avgRightLeg.w < 0.7)
        {
            //print("Move back k12");
        }
        else
        {
           // print("okay distance k12");
        }

        //script to call pause action
        if (avgleft.y > avgHead.y && avgright.y > avgHead.y && avgright.x < avgleft.x)
        {
            timer1 += Time.deltaTime;

            //print("timer is " + timer1);

            if (timer1 > holdDur & (pStatus == PlayerStatus.Playing | pStatus == PlayerStatus.Dead))
            {
                myPoints.ResetScore();
                dt.ResetSpeeds();
                pStatus = PlayerStatus.Menu;
                dt.gameMode = Modes.Waiting;
                Canvas.SetActive(true);
                Score.SetActive(false);
                DeathText.SetActive(false);
                //perform your action
                print("handsup show menu");
                timer1 = 0;
            }
        }
        else if (avgleft.y > avgHead.y && avgright.y > avgHead.y && avgright.x > avgleft.x)
        {

            timer1 += Time.deltaTime;

            //print("hands up cross " + timer1);

            if (timer1 > holdDur)
            {
                //perform your action
                calibrate = true;
                print("calibrated");
                timer1 = 0;
                
            }
        }
        else if (avgleft.x > avgBody.x && avgright.x > avgBody.x)
        {

            timer1 += Time.deltaTime;

            //print("pointing right " + timer1);

            if (timer1 > holdDur & pStatus == PlayerStatus.Menu)
            {
                dt.gameMode = Modes.Endless;
                pStatus = PlayerStatus.Playing;
                
                Canvas.SetActive(false);
                Score.SetActive(true);
                //perform your action
                print("hands right for 3x seconds");
                timer1 = 0;
            }
        }
        else if (avgleft.x < avgBody.x && avgright.x < avgBody.x)
        {

            timer1 += Time.deltaTime;

            //print("pointing left " + timer1);

            if (timer1 > holdDur & pStatus == PlayerStatus.Menu)
            {
                dt.gameMode = Modes.Standard;
                pStatus = PlayerStatus.Playing;
                Canvas.SetActive(false);
                Score.SetActive(true);
                //perform your action
                print("hands left for 3x seconds");
                timer1 = 0;
            }
        }
        else
        {

            calibrate = false;
            timer1 = 0;
        }

        //Calibration step
        if (calibrate==true)
        {
          Calibration(avgLeftLeg, avgRightLeg, avgBody);
        }

        if (CalibratedAlready)
        {
            CalibrateText.enabled = false;
        }

        if (pStatus == PlayerStatus.Dead)
        {
            DeathText.SetActive(true);
            Canvas.SetActive(false);
            Score.SetActive(false);
        }
    }

    private void Start()
    {
        myPoints = Score.GetComponent<Points>();
        dt = GameObject.Find("ObstacleGeneration").GetComponent<DynamicTest>();
        pStatus = PlayerStatus.Menu;

        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }

    void Calibration(Vector4 leftleg, Vector4 rightleg, Vector4 body)
    {
        
            //print("Calibrating");            
            yoffset = -(leftleg.y + rightleg.y) / 2;
            xoffset = -body.x;
            CalibratedAlready = true;
        
    }

    void GetInfo()
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        //client = listener.AcceptTcpClient();
        client = listener.AcceptTcpClient();

        running = true;
        while (running)
        {
            SendAndReceiveData();
        }
        listener.Stop();

    }
    void SendAndReceiveData()
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];

        //---receiving Data from the Host----
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize); //Getting data in Bytes from Python
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead); //Converting byte data to string

        if (dataReceived != null)
        {
            //---Using received data---
            StringToVector4Array(dataReceived); //<-- assigning receivedPos value from Python
            //---Sending Data to Host----
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes("Hey I got your message Python! Do You see this massage?"); //Converting string to byte data
            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length); //Sending the data in Bytes to Python
        }
    }
    void StringToVector4Array(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("[") && sVector.EndsWith("]"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }
        else
        {
            print("Something went wrong");
        }
        // split the items
        string[] posArray = sVector.Split('|');
        int numLandmarkPositions = posArray.Length;
        //Debug.assert(numlandmarkPositions == NUMLANDMARKS);
        float x = 0, y = 0, z = 0, v = 0;
        for (int i = 0; i < numLandmarkPositions; i++)
        {
            string curLandmark = posArray[i];
            if (curLandmark.StartsWith("(") && curLandmark.EndsWith(")"))
            {
                curLandmark = curLandmark.Substring(1, curLandmark.Length - 2);
            }
            else
            {
                print("Something went wrong");
            }
            string[] positions = curLandmark.Split(',');
            try
            {
                x = float.Parse(positions[0], CultureInfo.InvariantCulture);
                y = float.Parse(positions[1], CultureInfo.InvariantCulture);
                z = float.Parse(positions[2], CultureInfo.InvariantCulture);
                v = float.Parse(positions[3], CultureInfo.InvariantCulture);
                receivedPositions[i] = new Vector4(x, y, z, v); // Store the data
            }
            catch (Exception e)
            {
                print(e);
            }
        }
    }

    public void setDeath(bool myDeath)
    {
        pStatus = PlayerStatus.Dead;
    }

    public bool getDeath()
    {
        return pStatus == PlayerStatus.Dead;
    }

    Vector4 averageVector4(Vector4[] myVectors)
    {
        //print(myVectors);
        Vector4 avgVec = new Vector4(0, 0, 0, 0);
        for (int i = 0; i < myVectors.Length; i++)
        {
            avgVec.x += myVectors[i].x;
            avgVec.y += myVectors[i].y;
            avgVec.z += myVectors[i].z;
            avgVec.w += myVectors[i].w;
        }
        avgVec.x /= myVectors.Length;
        avgVec.y /= myVectors.Length;
        avgVec.z /= myVectors.Length;
        avgVec.w /= myVectors.Length;
        avgVec.x *= -1;
        avgVec.y *= -1;
        //avgVec.z *= -1;
        return avgVec;
    }

    Vector3 averageVector4_2(Vector4[] myVectors)
    {
        print(myVectors);
        Vector3 avgVec = new Vector3(0, 0, 0);
        float weight;
        float myWeights = 0;
        for (int i = 0; i < myVectors.Length; i++)
        {
            weight = myVectors[i].w;
            myWeights += weight;
            avgVec.x += myVectors[i].x*weight;
            avgVec.y += myVectors[i].y*weight;
            avgVec.z += myVectors[i].z*weight;
        }
        //avgVec.x /= myVectors.Length;
        //avgVec.y /= myVectors.Length;
        //avgVec.z /= myVectors.Length;
        avgVec.x /= myWeights;
        avgVec.y /= myWeights;
        avgVec.z /= myWeights;
        avgVec.x *= -1;
        avgVec.y *= -1;
        avgVec.z *= -1;
        return avgVec;
    }

    
}

public enum PlayerStatus
{
    Dead,
    Playing,
    Menu
}