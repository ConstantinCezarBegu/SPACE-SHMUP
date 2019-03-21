using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button startButton;
    public Button exitButton;
    
    private void Start()
    {
        startButton.onClick.AddListener(() => SceneManager.LoadScene("GameScene"));
        exitButton.onClick.AddListener(() =>
        {
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        });
    }
}
