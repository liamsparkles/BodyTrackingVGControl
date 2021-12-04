using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyDetection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalValue = Input.GetAxis("Horizontal");
        float verticalValue = Input.GetAxis("Vertical");

        if (horizontalValue != 0)
        {
            //print("Horizontal movement: " + horizontalValue);
            transform.position += new Vector3(horizontalValue, 0, 0);
            if (transform.position.x <= -1.5)
                transform.position = new Vector3(-1.5f, transform.position.y, transform.position.z);
            else if (transform.position.x >= 1.5)
                transform.position = new Vector3(1.5f, transform.position.y, transform.position.z);
        }
        if (verticalValue != 0)
        {
            transform.position += new Vector3(0, verticalValue, 0);
            if (transform.position.y <= 0)
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            else if (transform.position.y >= 2.5)
                transform.position = new Vector3(transform.position.x, 2.5f, transform.position.z);
        }
    }

    void OnCollisionEnter()
    {
        print("collided");
    }
}
