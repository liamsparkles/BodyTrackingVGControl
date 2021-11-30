using UnityEngine;
public class dynamicTest : MonoBehaviour
{
    private Vector3[] leftpos = new Vector3[6];
    private Vector3[] rightpos = new Vector3[6];
    private Vector3[] uppos = new Vector3[3];
    private Vector3[] downpos = new Vector3[3];

    // Instantiates prefabs in a circle formation
    public GameObject prefab;
    private double newBlockTime;
    public float newBlockCreationDelay = 5;
    //private int curState;
    public float dist = 0;

    void Start()
    {
        leftpos[0] = transform.position + new Vector3(0, 2, dist);
        leftpos[1] = transform.position + new Vector3(0, 1, dist);
        leftpos[2] = transform.position + new Vector3(0, 0, dist);
        leftpos[3] = transform.position + new Vector3(1, 2, dist);
        leftpos[4] = transform.position + new Vector3(1, 1, dist);
        leftpos[5] = transform.position + new Vector3(1, 0, dist);
        rightpos[0] = transform.position + new Vector3(0, 2, dist);
        rightpos[1] = transform.position + new Vector3(0, 1, dist);
        rightpos[2] = transform.position + new Vector3(0, 0, dist);
        rightpos[3] = transform.position + new Vector3(-1, 2, dist);
        rightpos[4] = transform.position + new Vector3(-1, 1, dist);
        rightpos[5] = transform.position + new Vector3(-1, 0, dist);
        uppos[0] = transform.position + new Vector3(-1, 0, dist);
        uppos[1] = transform.position + new Vector3(0, 0, dist);
        uppos[2] = transform.position + new Vector3(1, 0, dist);
        downpos[0] = transform.position + new Vector3(-1, 2, dist);
        downpos[1] = transform.position + new Vector3(0, 2, dist);
        downpos[2] = transform.position + new Vector3(1, 2, dist);

        newBlockTime = 0;
        //curState = 0;
        //float angle = i * Mathf.PI * 2 / numberOfObjects;
        //float x = Mathf.Cos(angle) * radius;
        //float z = Mathf.Sin(angle) * radius;
        //float x = 0;
        //float y = 0;
        //float z = 0;
        //Vector3 pos = transform.position + new Vector3(x, y, z);

        //For the move left obstacle

        //Quaternion rot = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        Vector3[] curPos;
        newBlockTime -= Time.deltaTime;
        //int prevState = curState;
        int rInt = (int)Random.Range(0, 3.9f);
        if (newBlockTime <= 0)
        {
            if (rInt == 0) curPos = leftpos;
            else if (rInt == 1) curPos = rightpos;
            else if (rInt == 2) curPos = uppos;
            else if (rInt == 3) curPos = downpos;
            else curPos = leftpos;

            for (int i = 0; i < curPos.Length; i++)
            {
                Instantiate(prefab, curPos[i], Quaternion.identity); 
            }
            newBlockTime = newBlockCreationDelay;
        }
    }
}