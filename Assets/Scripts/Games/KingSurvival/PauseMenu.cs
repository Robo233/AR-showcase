using UnityEngine;

/// <summary>
/// Handles the puasing and resuming of the game
/// </summary>

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    [SerializeField] GameStarter gameStarter;

    public void ResumeGame() // It is called from the ResumeButton
    {
        pauseMenu.SetActive(false);
        gameStarter.isPlaying = true;
        Time.timeScale = 1;
    }

    public void PauseGame() // It is called from the PauseButtonControllerCanvas
    {
        pauseMenu.SetActive(true);
        gameStarter.isPlaying = false;
        Time.timeScale = 0;
    }

    // The the functions below are responsible for pausing the game if the game is playing and the user soft closes the app, switches to another app, locks the screen or swipes down the notification panel

    void OnApplicationPause(bool pause)
    {
		if(pause && gameStarter.isPlaying)
        {
			PauseGame();
		}
	}

   void OnApplicationFocus(bool pause)
   {
		if (!pause && gameStarter.isPlaying)
        {
			PauseGame();
		}
	}

    public void Quit()
    {
        Application.Quit();
    }

}