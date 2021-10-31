using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System;
using System.Globalization;

int NUMLANDMARKS = 33;

public class poseDetectionSocket : MonoBehaviour
{
    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25002;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    Vector4[] receivedPositions = new Vector4[NUMLANDMARKS];
    bool running;
    private void Update()
    {
        transform.position = receivedPos; //assigning receivedPos in SendAndReceiveData()
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
            receivedPos = StringToVector4Array(dataReceived); //<-- assigning receivedPos value from Python
            print("received pos data, and moved the Cube!");

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
	    print("Something went wrong")	
	}
        // split the items
        string[] posArray = sVector.Split('|');
        int numlandmarkPositions = posArray.Length;
	Debug.assert(numlandmarkPositions == NUMLANDMARKS);
        float x=0,y=0,z=0,v=0;
        for(int i=0;i<numPositions;i++){
            print(posArray[i]);
            string[] positions = posArray[i].Split(',');
	    if (positions.StartsWith("(") && positions.EndsWith(")"))
	    {
		positions = positions.Substring(1, sVector.Length - 2);
	    }
	    else
	    {
		print("Something went wrong")	
	    }
	
            try{
                x = float.Parse(positions[0], CultureInfo.InvariantCulture);
                y = float.Parse(positions[1], CultureInfo.InvariantCulture);
                z = float.Parse(positions[2], CultureInfo.InvariantCulture);
		v = float.Parse(positions[3], CultureInfo.InvariantCulture);
		receivedPositions[i] = new Vector4(x,y,z,v); // Store the data
            }catch(Exception e){
                print(e);
            }
        }
    }
}
