using UnityEngine;
public class DynamicTest : MonoBehaviour
{
    private Obstacle[] obstacles = new Obstacle[5];

    // Instantiates prefabs in a circle formation
    public float DEFAULTCREATIONDELAY = 4;
    public float DEFAULTOBSTACLESPEED = 2;

    public GameObject prefab;
    private double newBlockTime;
    private float newBlockCreationDelay;
    //private int curState;
    public float dist = 20;
    public Modes gameMode = Modes.Waiting;
    private int[] standardGame;
    private int curObstacle;
    public float obstacleSpeed;
    private float relMaxSpeed;
    private float relNewBlock;
    public float offsety;
    //Points myPoints;

    void Start()
    {
        Vector3 scale = prefab.transform.localScale;
        offsety = scale.y / 2;
        Vector3 offset = transform.position + new Vector3(0, scale.y/2, dist);
        obstacles[0] = new ObstacleLeft(offset, scale);
        obstacles[1] = new ObstacleVLeft(offset, scale);
        obstacles[2] = new ObstacleRight(offset, scale);
        obstacles[3] = new ObstacleVRight(offset, scale);
        //obstacles[4] = new ObstacleDown(offset, scale);
        obstacles[4] = new ObstacleUp(offset, scale);

        newBlockTime = 0;

        standardGame = new int[]
        {
            1, 2, 3, 4, 0,
            3, 4, 0, 3, 4, 1, 3,
            3, 3, 1, 1, 2, 0, 4, 3, 1, 4
        };
        curObstacle = 0;

        //myPoints = GameObject.Find("Score").GetComponent<Points>();

        obstacleSpeed = DEFAULTOBSTACLESPEED;
        newBlockCreationDelay = DEFAULTCREATIONDELAY;
        relMaxSpeed = obstacleSpeed * 0.5f;
        relNewBlock = 0.1F;
        
        //Vector3 pos = transform.position + new Vector3(x, y, z);

        //For the move left obstacle

        //Quaternion rot = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        newBlockTime -= Time.deltaTime;
        if (newBlockTime <= 0)
        {
            newBlockTime = newBlockCreationDelay;
            Vector3[] curPos;
            if (gameMode == Modes.Standard)
            {
                if (curObstacle >= standardGame.Length)
                {
                    curObstacle++;
                    if (curObstacle >= standardGame.Length + 4)
                    {
                        //trigger game end here
                        print("Game Done");
                    }
                    return;
                }
                curPos = standardMode();
            }
            else if (gameMode == Modes.Endless)
            {
                curPos = endlessMode();
            }
            else if (gameMode == Modes.Waiting)
            {
                //print("here");
                return;
            }
            else
            {
                print("error");
                return;
            }

            for (int i = 0; i < curPos.Length; i++)
            {
                Instantiate(prefab, curPos[i], Quaternion.identity);
            }

        }
    }

    private Vector3[] standardMode()
    {
        SpeedUp();
        SpeedUp();
        return obstacles[standardGame[curObstacle++]].positions;
    }

    private Vector3[] endlessMode()
    {
        SpeedUp();
        int rInt = (int)Random.Range(0, obstacles.Length - 0.001f);
        return obstacles[rInt].positions;
    }

    private void SpeedUp()
    {
        //Determine max speed, and don't go past it
        //Lower speedups as speed gets really fast
        if (newBlockCreationDelay > 2) newBlockCreationDelay -= relNewBlock;
        if (obstacleSpeed < 40f) obstacleSpeed += relMaxSpeed;
        //print(newBlockCreationDelay);
        //print(obstacleSpeed);
    }

    public void ResetSpeeds()
    {
        obstacleSpeed = DEFAULTOBSTACLESPEED;
        newBlockCreationDelay = DEFAULTCREATIONDELAY;
    }
}

public enum Modes
{
    Endless,
    Standard,
    Waiting
}

abstract class Obstacle
{
    public Vector3[] positions;
}

class ObstacleRight : Obstacle
{
    public ObstacleRight(Vector3 offset, Vector3 scale)
    {
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(1 * scale.x, 2 * scale.y, 0);
        positions[1] = offset + new Vector3(1 * scale.x, 1 * scale.y, 0);
        positions[2] = offset + new Vector3(1 * scale.x, 0, 0);
    }
}

class ObstacleVRight : Obstacle
{
    public ObstacleVRight(Vector3 offset, Vector3 scale)
    {
        positions = new Vector3[6];
        positions[0] = offset + new Vector3(0, 2 * scale.y, 0);
        positions[1] = offset + new Vector3(0, 1 * scale.y, 0);
        positions[2] = offset + new Vector3(0, 0, 0);
        positions[3] = offset + new Vector3(1 * scale.x, 2 * scale.y, 0);
        positions[4] = offset + new Vector3(1 * scale.x, 1 * scale.y, 0);
        positions[5] = offset + new Vector3(1 * scale.x, 0, 0);
    }
}

class ObstacleLeft : Obstacle
{
    public ObstacleLeft(Vector3 offset, Vector3 scale)
    {
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(-1 * scale.x, 2 * scale.y, 0);
        positions[1] = offset + new Vector3(-1 * scale.x, 1 * scale.y, 0);
        positions[2] = offset + new Vector3(-1 * scale.x, 0, 0);
    }
}

class ObstacleVLeft : Obstacle
{
    public ObstacleVLeft(Vector3 offset, Vector3 scale)
    {
        positions = new Vector3[6];
        positions[0] = offset + new Vector3(0, 2 * scale.y, 0);
        positions[1] = offset + new Vector3(0, 1 * scale.y, 0);
        positions[2] = offset + new Vector3(0, 0, 0);
        positions[3] = offset + new Vector3(-1 * scale.x, 2 * scale.y, 0);
        positions[4] = offset + new Vector3(-1 * scale.x, 1 * scale.y, 0);
        positions[5] = offset + new Vector3(-1 * scale.x, 0, 0);
    }
}

class ObstacleDown : Obstacle
{
    public ObstacleDown(Vector3 offset, Vector3 scale)
    {
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(-1 * scale.x, 0, 0);
        positions[1] = offset + new Vector3(0, 0, 0);
        positions[2] = offset + new Vector3(1 * scale.x, 0, 0);
    }
}

class ObstacleUp : Obstacle
{
    public ObstacleUp(Vector3 offset, Vector3 scale)
    {
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(-1 * scale.x, 2 * scale.y, 0);
        positions[1] = offset + new Vector3(0, 2 * scale.y, 0);
        positions[2] = offset + new Vector3(1 * scale.x, 2 * scale.y, 0);
    }
}