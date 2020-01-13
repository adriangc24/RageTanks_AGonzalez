using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWatcher2 : MonoBehaviour
{
    public static Text scoreText;
    public static int currScore = 0;


    // Start is called before the first frame update
    void Start()
    {
        scoreText = gameObject.GetComponent<Text>();
        scoreText.text="0";
    }
    public static void addScore(int scoreToAdd)
    {
        currScore += scoreToAdd;
        scoreText.text = currScore.ToString();
    }
    

    
}
