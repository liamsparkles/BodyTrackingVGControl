using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Points : MonoBehaviour
{
    public int score;
    public TextMeshProUGUI scoreTMP;

    // Start is called before the first frame update
    void Start()
    {

        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPoints()
    {
        score += 10;
        scoreTMP.text ="Score:" + score.ToString();
    }

    public void ResetScore()
    {
        print("Resetting score");
        score = 0;
        scoreTMP.text = "Score:" + score.ToString();
    }
}