using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Points : MonoBehaviour
{
    public int score; // score value
    public TextMeshProUGUI scoreTMP; // score gui object

    // Start is called before the first frame update
    void Start()
    {
        score = 0; // Start with 0 score
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPoints()
    {
        score += 10; //add 10 points to the score
        scoreTMP.text ="Score:" + score.ToString(); //update the GUI display
    }

    public void ResetScore()
    { //Resets the score and GUI display
        print("Resetting score");
        score = 0;
        scoreTMP.text = "Score:" + score.ToString();
    }
}
