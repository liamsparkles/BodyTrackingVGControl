using UnityEngine;
public class dynamicTest : MonoBehaviour
{
    private Obstacle[] obstacles = new Obstacle[6];

    // Instantiates prefabs in a circle formation
    public GameObject prefab;
    private double newBlockTime;
    public float newBlockCreationDelay = 5;
    //private int curState;
    public float dist = 0;

    void Start()
    {
        Vector3 offset = transform.position + new Vector3(0, 0, dist);
        obstacles[0] = new ObstacleLeft(offset);
        obstacles[1] = new ObstacleVLeft(offset);
        obstacles[2] = new ObstacleRight(offset);
        obstacles[3] = new ObstacleVRight(offset);
        obstacles[4] = new ObstacleDown(offset);
        obstacles[5] = new ObstacleUp(offset);

        newBlockTime = 0;
        //Vector3 pos = transform.position + new Vector3(x, y, z);

        //For the move left obstacle

        //Quaternion rot = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        Vector3[] curPos;
        newBlockTime -= Time.deltaTime;
        int rInt = (int)Random.Range(0, obstacles.Length - 0.001f);
        if (newBlockTime <= 0)
        {
            curPos = obstacles[rInt].positions;

            for (int i = 0; i < curPos.Length; i++)
            {
                Instantiate(prefab, curPos[i], Quaternion.identity); 
            }
            newBlockTime = newBlockCreationDelay;
        }
    }
}

abstract class Obstacle 
{
    public Vector3[] positions;
}

class ObstacleRight : Obstacle
{
    public ObstacleRight(Vector3 offset)
    {
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(1, 2, 0);
        positions[1] = offset + new Vector3(1, 1, 0);
        positions[2] = offset + new Vector3(1, 0, 0);
    }
}

class ObstacleVRight : Obstacle
{
    public ObstacleVRight(Vector3 offset)
    {
        positions = new Vector3[6];
        positions[0] = offset + new Vector3(0, 2, 0);
        positions[1] = offset + new Vector3(0, 1, 0);
        positions[2] = offset + new Vector3(0, 0, 0);
        positions[3] = offset + new Vector3(1, 2, 0);
        positions[4] = offset + new Vector3(1, 1, 0);
        positions[5] = offset + new Vector3(1, 0, 0);
    }
}

class ObstacleLeft : Obstacle
{
    public ObstacleLeft(Vector3 offset)
    {
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(-1, 2, 0);
        positions[1] = offset + new Vector3(-1, 1, 0);
        positions[2] = offset + new Vector3(-1, 0, 0);
    }
}

class ObstacleVLeft : Obstacle
{
    public ObstacleVLeft(Vector3 offset)
    {
        positions = new Vector3[6];
        positions[0] = offset + new Vector3(0, 2, 0);
        positions[1] = offset + new Vector3(0, 1, 0);
        positions[2] = offset + new Vector3(0, 0, 0);
        positions[3] = offset + new Vector3(-1, 2, 0);
        positions[4] = offset + new Vector3(-1, 1, 0);
        positions[5] = offset + new Vector3(-1, 0, 0);
    }
}

class ObstacleDown : Obstacle
{
    public ObstacleDown(Vector3 offset)
    {
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(-1, 0, 0);
        positions[1] = offset + new Vector3(0, 0, 0);
        positions[2] = offset + new Vector3(1, 0, 0);
    }
}

class ObstacleUp : Obstacle
{
    public ObstacleUp(Vector3 offset)
    {
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(-1, 2, 0);
        positions[1] = offset + new Vector3(0, 2, 0);
        positions[2] = offset + new Vector3(1, 2, 0);
    }
}
