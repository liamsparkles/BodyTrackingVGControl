using UnityEngine;
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public class poseDetectionSocket : MonoBehaviour
{
    public const int NUMLANDMARKS = 33;
    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    Vector4[] receivedPositions = new Vector4[NUMLANDMARKS];
    bool running;
    private int count = 0;

    private void Update()
    {
        //Landmark positions 
        //Left hand 16 18 20 22
        //Right hand 15 17 19 21
        Vector4[] lefthandpositions = new Vector4[] {receivedPositions[16], receivedPositions[18], receivedPositions[20], receivedPositions[22]};
        Vector4[] righthandpositions = new Vector4[] {receivedPositions[15], receivedPositions[17], receivedPositions[19], receivedPositions[21]};

        Vector3 avgleft = averageVector4(lefthandpositions);
        Vector3 avgright = averageVector4(righthandpositions);
        //print("Left is");
        print(avgleft);
        //print("right is");
        //print(avgright);
        transform.position = avgleft;
    }

    private void Start()
    {
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }

    void GetInfo()
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        //client = listener.AcceptTcpClient();

        running = true;
        while (running)
        {
            client = listener.AcceptTcpClient();
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
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes("Hey I got your message Python! Do You see this massage?" + count++); //Converting string to byte data
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

    Vector3 averageVector4(Vector4[] myVectors)
    {
        print(myVectors);
        Vector3 avgVec = new Vector3(0, 0, 0);
        for (int i = 0; i < myVectors.Length; i++)
        {
            avgVec.x += myVectors[i].x;
            avgVec.y += myVectors[i].y;
            avgVec.z += myVectors[i].z;
        }
        //avgVec.x /= myVectors.Length;
        //avgVec.y /= myVectors.Length;
        //avgVec.z /= myVectors.Length;
        avgVec.x *= -1;
        avgVec.y *= -1;
        avgVec.z *= -1;
        return avgVec;
    }
}
