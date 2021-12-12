using UnityEngine;
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;

// Controlls communication with the python socket and controls the player model
public class poseDetectionSocket : MonoBehaviour
{
    // PLayer model body part objects
    public GameObject LHand;
    public GameObject RHand;
    public GameObject LLeg;
    public GameObject RLeg;
    public GameObject Head;
    public GameObject Body;

    // Player model body part vectors
    public Vector3 avgleft;
    public Vector3 avgright;
    public Vector3 avgLeftLeg;
    public Vector3 avgRightLeg;
    public Vector3 avgHead;
    public Vector3 avgBody;

    public const int NUMLANDMARKS = 33; // Number of landmarks

    //Communication Variables
    Thread mThread; // thread to manage communciation with python
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25003;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    Vector4[] receivedPositions = new Vector4[NUMLANDMARKS]; // received values from python
    bool running; // communication is runnign (always true)

    // Calibration variables
    public float xoffset; // calibration offset x
    public float yoffset; // calibration offset y
    public bool calibrate = false; //for recalibrating the player 
    public bool CalibratedAlready=false; // for controlling the gui
    float timer1; // for timing action held time
    public float holdDur = 2f; // how long to hold an action

    // UI display elements
    public GameObject Canvas;
    public GameObject Score;
    public GameObject DeathText;
    public TextMeshProUGUI CalibrateText;

    Points myPoints; // points script
    public PlayerStatus pStatus; //player status
    DynamicTest dt; //obstacle creation script

    private void Update()
    {

        //Landmark positions 
        Vector4[] lefthandpositions = new Vector4[] { receivedPositions[16], receivedPositions[18], receivedPositions[20] };
        Vector4[] righthandpositions = new Vector4[] { receivedPositions[15], receivedPositions[17], receivedPositions[19] };
        Vector4[] leftLegposition = new Vector4[] { receivedPositions[28], receivedPositions[32], receivedPositions[30] };
        Vector4[] rightLegposition = new Vector4[] { receivedPositions[27], receivedPositions[29], receivedPositions[31] };
        Vector4[] headposition = new Vector4[] { receivedPositions[1], receivedPositions[2], receivedPositions[3], receivedPositions[4], receivedPositions[5], receivedPositions[6], receivedPositions[9], receivedPositions[10], receivedPositions[0] };
        Vector4[] body = new Vector4[] { receivedPositions[24], receivedPositions[23] };

	// Average the positiosn for each body part
        Vector4 avgleft = averageVector4(lefthandpositions);
        Vector4 avgright = averageVector4(righthandpositions);
        Vector4 avgLeftLeg = averageVector4(leftLegposition);
        Vector4 avgRightLeg = averageVector4(rightLegposition);
        Vector4 avgHead = averageVector4(headposition);
        Vector4 avgBody = averageVector4(body);

	// Z value is not relevant (2D game), ignore it
        avgleft.z = 0;
        avgright.z = 0;
        avgLeftLeg.z = 0;
        avgRightLeg.z = 0;
        avgHead.z = 0;
        avgBody.z = 0;

	// Transform each of the player model objects to their respective positions
        LHand.transform.position = new Vector3(avgleft.x + xoffset, avgleft.y + yoffset, avgleft.z);
        RHand.transform.position = new Vector3(avgright.x + xoffset, avgright.y + yoffset, avgleft.z);
        RLeg.transform.position = new Vector3(avgLeftLeg.x + xoffset, avgLeftLeg.y + yoffset, avgLeftLeg.z);
        LLeg.transform.position = new Vector3(avgRightLeg.x + xoffset, avgRightLeg.y + yoffset, avgRightLeg.z);
        Head.transform.position = new Vector3(avgHead.x + xoffset, avgHead.y + yoffset, avgHead.z);
        Body.transform.position = new Vector3(avgBody.x + xoffset, avgBody.y + yoffset, avgBody.z);


        if (avgleft.y > avgHead.y && avgright.y > avgHead.y && avgright.x < avgleft.x)
        {  // Script to restart the game
            timer1 += Time.deltaTime;
            if (timer1 > holdDur & (pStatus == PlayerStatus.Playing | pStatus == PlayerStatus.Dead))
            { // Restarts the game
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
        { // Hands crossed above your head action
            timer1 += Time.deltaTime;
            if (timer1 > holdDur)
            {
                //calibrate the player
                calibrate = true;
                print("calibrated");
                timer1 = 0;
            }
        }
        else if (avgleft.x > avgBody.x && avgright.x > avgBody.x)
        { // Hands pointing to the right direction
            timer1 += Time.deltaTime;
            if (timer1 > holdDur & pStatus == PlayerStatus.Menu & CalibratedAlready)
            { // If held for some seconds, start endless mode
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
        { // Hands pointing in the left direction
            timer1 += Time.deltaTime;
            if (timer1 > holdDur & pStatus == PlayerStatus.Menu)
            { // Select standard mode
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
        { // set to false after calibrated (so you can do it again in a few seconds)
            calibrate = false;
            timer1 = 0;
        }

        //Calibration step
        if (calibrate==true)
        { // Calibrate the player
          Calibration(avgLeftLeg, avgRightLeg, avgBody);
        }

        if (CalibratedAlready)
        { // Once calibrated remove prompt to calibrate
            CalibrateText.enabled = false;
        }

        if (pStatus == PlayerStatus.Dead)
        { // Set player death to dead, show death screen
            DeathText.SetActive(true);
            Canvas.SetActive(false);
            Score.SetActive(false);
        }
    }

    private void Start()
    {
        myPoints = Score.GetComponent<Points>(); // Find points script
        dt = GameObject.Find("ObstacleGeneration").GetComponent<DynamicTest>(); // Find the obstacle generation script
        pStatus = PlayerStatus.Menu; // Get the player status

        ThreadStart ts = new ThreadStart(GetInfo); //Start thread for communication with python (open source)
        mThread = new Thread(ts);
        mThread.Start();
    }

    void Calibration(Vector4 leftleg, Vector4 rightleg, Vector4 body)
    { // Calibration steps, offsets the player model position to centre of the screen
            yoffset = -(leftleg.y + rightleg.y) / 2;
            xoffset = -body.x;
            CalibratedAlready = true;
    }

    void GetInfo()
    {
	// Setup and open socket to accept clients (taken from Open Source)
	// https://github.com/CanYouCatchMe01/CSharp-and-Python-continuous-communication
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

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
	// Open up a network steram  (taken from Open Source)
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
	// Converts the communicated string to a vector (modified from Open source)
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
        float x = 0, y = 0, z = 0, v = 0;
        for (int i = 0; i < numLandmarkPositions; i++)
        { // For every landmark position, add to the array as a vector4
            string curLandmark = posArray[i];
            if (curLandmark.StartsWith("(") && curLandmark.EndsWith(")"))
            { //Split up the current landmark
                curLandmark = curLandmark.Substring(1, curLandmark.Length - 2);
            }
            else
            { // Should never be here, communication or formatting error
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
            { // Should never be here, communication error or formatting error
                print(e);
            }
        }
    }

    public void setDeath(bool myDeath)
    { // Set player to dead
        pStatus = PlayerStatus.Dead;
    }

    public bool getDeath()
    { // Check if player is dead
        return pStatus == PlayerStatus.Dead;
    }

    Vector4 averageVector4(Vector4[] myVectors)
    {
	//Averages the vector values in myVectors
        Vector4 avgVec = new Vector4(0, 0, 0, 0);
        for (int i = 0; i < myVectors.Length; i++)
        { // Add vector values for each vector
            avgVec.x += myVectors[i].x;
            avgVec.y += myVectors[i].y;
            avgVec.z += myVectors[i].z;
            avgVec.w += myVectors[i].w;
        }
	// Divide by the amount of vectors
        avgVec.x /= myVectors.Length;
        avgVec.y /= myVectors.Length;
        avgVec.z /= myVectors.Length;
        avgVec.w /= myVectors.Length;
	// Flip xy axis because camera causes mirroring
        avgVec.x *= -1;
        avgVec.y *= -1;
        return avgVec;
    }

    Vector3 averageVector4_2(Vector4[] myVectors)
    { // Average points using weighted points based on visibility value (unused)
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
{ //Holds the player state
    Dead,
    Playing,
    Menu
}
