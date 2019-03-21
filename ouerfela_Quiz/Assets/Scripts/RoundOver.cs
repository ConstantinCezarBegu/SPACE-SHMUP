using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundOver : MonoBehaviour
{
    
    public Text timeText;
    public Text scoreText;
    public Button menuButton;
    
    private void Start()
    {
        timeText.text = $"Time: {GameQuiz.TimeRemaining}";
        scoreText.text = $"Score: {GameQuiz.Score}/400";
        menuButton.onClick.AddListener(() => SceneManager.LoadScene("MenuScene"));
    }
}
