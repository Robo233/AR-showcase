using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the main menu of the game, whether the tutorial is set or not and the highscore
/// </summary>

public class MenuHandler : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;

    [SerializeField] Text highScoreText;

    [SerializeField] Toggle tutorialToggle;

    public bool isTutorialSet;

    [SerializeField] ScoreHandler scoreHandler;
    [SerializeField] GameStarter gameStarter;
    [SerializeField] PauseMenu pauseMenu;

    void Start()
    {
        if(PlayerPrefs.HasKey("tutorialKingSurvival"))
        {
            if(PlayerPrefs.GetInt("tutorialKingSurvival")==0)
            {
                tutorialToggle.isOn = false;
                isTutorialSet = false;
            }
            else
            {
                tutorialToggle.isOn = true;
                isTutorialSet = true;
            }
        }
        else
        {
            PlayerPrefs.SetInt("tutorialKingSurvival",1);
            isTutorialSet = true;
        }

        highScoreText.text = "Highscore: " + PlayerPrefs.GetInt("highscore");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(gameStarter.isPlaying)
            {
                pauseMenu.PauseGame();    
            }
            else if(gameStarter.isGameStarted)
            {
                gameStarter.BackToMenu();
                scoreHandler.SetHighscore();
            }
            else
            {
                StartCoroutine(SceneLoader.LoadAsynchronously("MainMenu"));
            }
            
        }
    }

    public void ToggleTutorial()
    {
        if(tutorialToggle.isOn)
        {
            isTutorialSet = true;
            PlayerPrefs.SetInt("tutorialKingSurvival",1);
        }
        else
        {
            isTutorialSet = false;
            PlayerPrefs.SetInt("tutorialKingSurvival",0);
        }
        
    }

}