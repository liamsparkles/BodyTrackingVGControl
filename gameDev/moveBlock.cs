using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveBlock : MonoBehaviour
{
    public float deleteDepth = -5.0f;
    public float speed = 0.001f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(0, 0, speed);

        if (transform.position.z <= deleteDepth)
        {
            Destroy(gameObject);
        }
    }
}
