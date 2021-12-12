using UnityEngine;
public class DynamicTest : MonoBehaviour
{
    private Obstacle[] obstacles = new Obstacle[6];

    // Instantiates prefabs in a circle formation
    public float DEFAULTCREATIONDELAY = 4; //Default Time beween obstacles being created
    public float DEFAULTOBSTACLESPEED = 2; //Default Obstacle speed

    private double newBlockTime;  //variable for counting for obstacle creating
    public float newBlockCreationDelay; //current time between obstacles beign created
    public GameObject prefabGeneral; //general block
    public GameObject prefabJump; //block for jumps
    public float dist = 20; //distance to create teh obstacles
    public Modes gameMode = Modes.Waiting; //mode to control game modes
    private int[] standardGame; //array to contain the standard game objstacles
    private int curObstacle; //value to contain the curreent obstacle for standard mode
    public float obstacleSpeed; //current speed of obstacles
    private float relMaxSpeed; //amount to increase obstacle speed per step
    private float relNewBlock; //amount to decrease obstacle creation delay per step

    void Start()
    {
	//Creating the 6 different obstacles
        Vector3 offset = transform.position + new Vector3(0, 0, dist);
        obstacles[0] = new ObstacleLeft(offset, prefabGeneral);
        obstacles[1] = new ObstacleVLeft(offset, prefabGeneral);
        obstacles[2] = new ObstacleRight(offset, prefabGeneral);
        obstacles[3] = new ObstacleVRight(offset, prefabGeneral);
        obstacles[4] = new ObstacleDown(offset+new Vector3(0f,0.02f,0f), prefabJump);
        obstacles[5] = new ObstacleUp(offset, prefabGeneral);

        newBlockTime = 0; //Starting block delay

	//Standard game mode index setup
        standardGame = new int[]
        {
            1, 2, 3, 4, 5, 0,
            3, 4, 5, 0, 3, 5, 5, 4, 1, 3,
            3, 3, 1, 1, 2, 0, 5, 4, 3, 5, 1, 4
        };

	//Setup starting values
        curObstacle = 0;
        obstacleSpeed = DEFAULTOBSTACLESPEED;
        newBlockCreationDelay = DEFAULTCREATIONDELAY;
	//Set speed up delta variables
        relMaxSpeed = obstacleSpeed * 0.1f;
        relNewBlock = 0.025f;
    }

    void Update()
    {
	//Reduce the time variable, if 0, reset it and generate an obstacle
        newBlockTime -= Time.deltaTime;
        if (newBlockTime <= 0)
        {
            newBlockTime = newBlockCreationDelay;
            Obstacle myObst;
            if (gameMode == Modes.Standard)
            { //Play standard mode obstacles
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
            { //Generate endless mode obstacles
                myObst = endlessMode();
            }
            else if (gameMode == Modes.Waiting)
            { //Wait for the user to select
                return;
            }
            else
            { //Should never be here,
                print("error");
                return;
            }

            for (int i = 0; i < myObst.positions.Length; i++)
            { //Instantiate the blocks to make up the obstacle
                Instantiate(myObst.prefab, myObst.positions[i], Quaternion.identity);
            }
            newBlockTime = newBlockCreationDelay;  //Reset the delay time
        }
    }

    private Obstacle standardMode()
    { //Standard mode, speed up twice and generate the next obstacle
        SpeedUp();
        SpeedUp();
        return obstacles[standardGame[curObstacle++]];
    }

    private Obstacle endlessMode()
    { //ENdless mdoe, speed up and generate a random obstacle
        SpeedUp();
        int rInt = (int)Random.Range(0, obstacles.Length - 0.001f);
        return obstacles[rInt];
    }

    private void SpeedUp()
    {
        //Determine max speed, and don't go past it
        //Stop speedups once speed gets really fast
        if (newBlockCreationDelay > 2) newBlockCreationDelay -= relNewBlock;
        if (obstacleSpeed < 40f) obstacleSpeed += relMaxSpeed;
        print(newBlockCreationDelay);
        print(obstacleSpeed);
    }

    public void ResetSpeeds()
    { // Restart the game, so reset the speeds and restart standard mode
        obstacleSpeed = DEFAULTOBSTACLESPEED;
        newBlockCreationDelay = DEFAULTCREATIONDELAY;
        curObstacle = 0;
    }
}

public enum Modes
{ // Modes for game select
    Endless,
    Standard,
    Waiting
}

abstract class Obstacle
{ // Obstacle parent class
    public GameObject prefab; //The gameObject to create
    public Vector3[] positions; //the positions to instantiate that object
}

class ObstacleRight : Obstacle
{
    public ObstacleRight(Vector3 offset, GameObject myprefab)
    {
        prefab = myprefab;
        Vector3 scale = prefab.transform.localScale;
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(1 * scale.x, 2 * scale.y, 0);
        positions[1] = offset + new Vector3(1 * scale.x, 1 * scale.y, 0);
        positions[2] = offset + new Vector3(1 * scale.x, 0, 0);
    }
}

class ObstacleVRight : Obstacle
{
    public ObstacleVRight(Vector3 offset, GameObject myprefab)
    {
        prefab = myprefab;
        Vector3 scale = prefab.transform.localScale;
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
    public ObstacleLeft(Vector3 offset, GameObject myprefab)
    {
        prefab = myprefab;
        Vector3 scale = prefab.transform.localScale;
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(-1 * scale.x, 2 * scale.y, 0);
        positions[1] = offset + new Vector3(-1 * scale.x, 1 * scale.y, 0);
        positions[2] = offset + new Vector3(-1 * scale.x, 0, 0);
    }
}

class ObstacleVLeft : Obstacle
{
    public ObstacleVLeft(Vector3 offset, GameObject myprefab)
    {
        prefab = myprefab;
        Vector3 scale = prefab.transform.localScale;
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
    public ObstacleDown(Vector3 offset, GameObject myprefab)
    {
        prefab = myprefab;
        Vector3 scale = prefab.transform.localScale;
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(-1 * scale.x, 0, 0);
        positions[1] = offset + new Vector3(0, 0, 0);
        positions[2] = offset + new Vector3(1 * scale.x, 0, 0);
    }
}

class ObstacleUp : Obstacle
{
    public ObstacleUp(Vector3 offset, GameObject myprefab)
    {
        prefab = myprefab;
        Vector3 scale = prefab.transform.localScale;
        positions = new Vector3[3];
        positions[0] = offset + new Vector3(-1 * scale.x, 2 * scale.y, 0);
        positions[1] = offset + new Vector3(0, 2 * scale.y, 0);
        positions[2] = offset + new Vector3(1 * scale.x, 2 * scale.y, 0);
    }
}
