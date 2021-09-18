using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour
{
    public static PointsManager instance;

    public Text CoinText;
    public Text HighscoreText;

    int score = 0;
    int highscore = 0;

    public void Awake()
    {
        instance = this; 
    }

    // Start is called before the first frame update
    public void Start()
    {
        highscore = PlayerPrefs.GetInt("highscore", 0);
        CoinText.text =  "Score: " + score.ToString();
        HighscoreText.text =  "Highscore: " + highscore.ToString();
        
    }

    public void AddPoint()
    {
        score++;
        CoinText.text =  "Score: " + score.ToString();
        if(highscore < score)
        {
            PlayerPrefs.SetInt("highscore", score);
        }

    }

    public void ResetScore()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
