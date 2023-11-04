using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the start of the game. If the tutorial is actviated from the menu, the tutorial menu is shown first, if not, the game is started directly
/// </summary>

public class GameStarter : MonoBehaviour
{
    [SerializeField] GameObject controllerCanvas;
    [SerializeField] GameObject planeFinder;
    [SerializeField] GameObject loadingScreenStart;
    [SerializeField] GameObject tutorialCanvas1;
    [SerializeField] GameObject tutorialCanvas2;
    [SerializeField] GameObject deathCanvas;
    [SerializeField] GameObject groundPlaneStage;
    [SerializeField] GameObject mainMenu;

    [SerializeField] float minimumWaitingTime;

    public bool isPlaying;
    public bool isGameStarted;
    public bool isLoading;
    public static bool isRestarted;

    public PlayerMovement playerMovement;
    [SerializeField] MenuHandler menuHandler;
    [SerializeField] EnemyGenerator enemyGenerator;
    [SerializeField] PartDestroyer partDestroyer;

    public void StartModelDetectionOrShowTutorial1() // It is called from the PlayButton
    {
        isGameStarted = true;
        mainMenu.SetActive(false);
        if(menuHandler.isTutorialSet)
        {
            tutorialCanvas1.SetActive(true);
        }
        else
        {
            StartModelDetection();
        }
    }

    public void StartModelDetection()
    {
        tutorialCanvas1.SetActive(false);
        planeFinder.SetActive(true);        
    }

    public void StartGameOrShowTutorial2()
    {
        controllerCanvas.SetActive(true);
        planeFinder.SetActive(false);
        if(menuHandler.isTutorialSet)
        {
            tutorialCanvas2.SetActive(true);
        }
        else
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        isPlaying = true;
        playerMovement.isPlayerPlaced = true;
        tutorialCanvas2.SetActive(false);
    }

    public void RestartGame()
    {
        isRestarted = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        isRestarted = false;
        Time.timeScale = 1;
    }
}
