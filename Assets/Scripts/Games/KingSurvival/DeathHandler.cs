using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// It contains the method which is called when the player dies
/// </summary>

public class DeathHandler : MonoBehaviour
{
    [SerializeField] GameObject deathCanvas;
    [SerializeField] GameObject controllerCanvas;

    [SerializeField] Text scoreDeath;
    [SerializeField] Text highScoreDeath;

    [SerializeField] AudioSource playerDeathSound;

    [SerializeField] GameStarter gameStarter;

    [SerializeField] ScoreHandler scoreHandler;

    public void Death()
    {
        playerDeathSound.Play();
        deathCanvas.SetActive(true);
        controllerCanvas.SetActive(false);
        scoreDeath.text = "Score: " + scoreHandler.score;
        scoreHandler.SetHighscore();
        highScoreDeath.text = "Highscore: " + scoreHandler.highscore;
        gameStarter.isPlaying = false;
   }
  
}