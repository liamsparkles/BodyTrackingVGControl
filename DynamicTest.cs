using UnityEngine;
public class DynamicTest : MonoBehaviour
{
    private Obstacle[] obstacles = new Obstacle[6];

    // Instantiates prefabs in a circle formation
    public float DEFAULTCREATIONDELAY = 4;
    public float DEFAULTOBSTACLESPEED = 2;

    private double newBlockTime;
    public float newBlockCreationDelay;
    public GameObject prefabGeneral;
    public GameObject prefabJump;
    //private int curState;
    public float dist = 20;
    public Modes gameMode = Modes.Waiting;
    private int[] standardGame;
    private int curObstacle;
    public float obstacleSpeed;
    private float relMaxSpeed;
    private float relNewBlock;
    public float offsety;

    void Start()
    {
        Vector3 offset = transform.position + new Vector3(0, 0, dist);
        obstacles[0] = new ObstacleLeft(offset, prefabGeneral);
        obstacles[1] = new ObstacleVLeft(offset, prefabGeneral);
        obstacles[2] = new ObstacleRight(offset, prefabGeneral);
        obstacles[3] = new ObstacleVRight(offset, prefabGeneral);
        obstacles[4] = new ObstacleDown(offset, prefabJump);
        obstacles[5] = new ObstacleUp(offset, prefabGeneral);

        newBlockTime = 0;

        standardGame = new int[] 
        { 
            1, 2, 3, 4, 5, 0,
            3, 4, 5, 0, 3, 5, 5, 4, 1, 3,
            3, 3, 1, 1, 2, 0, 5, 4, 3, 5, 1, 4
        };
        curObstacle = 0;

	obstacleSpeed = DEFAULTOBSTACLESPEED;
	newBLockCreationDelay = DEFAULTCREATIONDELAY;
        relMaxSpeed = obstacleSpeed * 0.1f;
        relNewBlock = 0.025f;
        //For the move left obstacle
    }

    void Update()
    {
        newBlockTime -= Time.deltaTime;
        if (newBlockTime <= 0)
        {
	    newBlockTime = newBlockCreationDelay;
            Obstacle myObst;
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
                myObst = standardMode();
            }
            else if (gameMode == Modes.Endless)
            { 
                myObst = endlessMode();
            }
            else if (gameMode == Modes.Waiting)
	    {
		return;
	    }
	    else
            {
                print("error");
		return;
            }

            for (int i = 0; i < myObst.positions.Length; i++)
            {
                Instantiate(myObst.prefab, myObst.positions[i], Quaternion.identity); 
            }
            newBlockTime = newBlockCreationDelay;
        }
    }

    private Obstacle standardMode()
    {
        SpeedUp();
        SpeedUp();
        return obstacles[standardGame[curObstacle++]];
    } 

    private Obstacle endlessMode()
    {
        SpeedUp();
        int rInt = (int)Random.Range(0, obstacles.Length - 0.001f);
        return obstacles[rInt];
    }

    private void SpeedUp()
    {
        //Determine max speed, and don't go past it
        //Lower speedups as speed gets really fast
        if (newBlockCreationDelay > 2) newBlockCreationDelay -= relNewBlock;
        if (obstacleSpeed < 40f) obstacleSpeed += relMaxSpeed;
        print(newBlockCreationDelay);
        print(obstacleSpeed);
    }

    public void ResetSpeeds()
    {
	    obstacleSpeed = DEFAULTOBSTACLESPEED;
	    newBlockCreationDelay = DEFAULTCREATIONDELAY;
	    curObstacle = 0;
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
    public GameObject prefab;
    public Vector3[] positions;
}

class ObstacleRight : Obstacle
{
    public ObstacleRight(Vector3 offset, GameObject myprefab)
    {
        prefab = myprefab;
        Vector3 scale = prefab.transform.localScale;
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(1*scale.x, 2*scale.y, 0);
        positions[1] = offset + new Vector3(1*scale.x, 1*scale.y, 0);
        positions[2] = offset + new Vector3(1*scale.x, 0, 0);
    }
}

class ObstacleVRight : Obstacle
{
    public ObstacleVRight(Vector3 offset, GameObject myprefab)
    {
        prefab = myprefab;
        Vector3 scale = prefab.transform.localScale;
        positions = new Vector3[6];
        positions[0] = offset + new Vector3(0, 2*scale.y, 0);
        positions[1] = offset + new Vector3(0, 1*scale.y, 0);
        positions[2] = offset + new Vector3(0, 0, 0);
        positions[3] = offset + new Vector3(1*scale.x, 2*scale.y, 0);
        positions[4] = offset + new Vector3(1*scale.x, 1*scale.y, 0);
        positions[5] = offset + new Vector3(1*scale.x, 0, 0);
    }
}

class ObstacleLeft : Obstacle
{
    public ObstacleLeft(Vector3 offset, GameObject myprefab)
    {
        prefab = myprefab;
        Vector3 scale = prefab.transform.localScale;
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(-1*scale.x, 2*scale.y, 0);
        positions[1] = offset + new Vector3(-1*scale.x, 1*scale.y, 0);
        positions[2] = offset + new Vector3(-1*scale.x, 0, 0);
    }
}

class ObstacleVLeft : Obstacle
{
    public ObstacleVLeft(Vector3 offset, GameObject myprefab)
    {
        prefab = myprefab;
        Vector3 scale = prefab.transform.localScale;
        positions = new Vector3[6];
        positions[0] = offset + new Vector3(0, 2*scale.y, 0);
        positions[1] = offset + new Vector3(0, 1*scale.y, 0);
        positions[2] = offset + new Vector3(0, 0, 0);
        positions[3] = offset + new Vector3(-1*scale.x, 2*scale.y, 0);
        positions[4] = offset + new Vector3(-1*scale.x, 1*scale.y, 0);
        positions[5] = offset + new Vector3(-1*scale.x, 0, 0);
    }
}

class ObstacleDown : Obstacle
{
    public ObstacleDown(Vector3 offset, GameObject myprefab)
    {
        prefab = myprefab;
        Vector3 scale = prefab.transform.localScale;
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(-1*scale.x, 0, 0);
        positions[1] = offset + new Vector3(0, 0, 0);
        positions[2] = offset + new Vector3(1*scale.x, 0, 0);
    }
}

class ObstacleUp : Obstacle
{
    public ObstacleUp(Vector3 offset, GameObject myprefab)
    {
        prefab = myprefab;
        Vector3 scale = prefab.transform.localScale;
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(-1*scale.x, 2*scale.y, 0);
        positions[1] = offset + new Vector3(0, 2*scale.y, 0);
        positions[2] = offset + new Vector3(1*scale.x, 2*scale.y, 0);
    }
}
