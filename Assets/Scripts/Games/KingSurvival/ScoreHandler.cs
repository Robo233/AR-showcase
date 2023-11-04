using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the score system of the game
/// </summary>

public class ScoreHandler : MonoBehaviour
{
    [SerializeField] GameObject newHighscoreText;

    [SerializeField] Text scoreTextControllerCanvas;

    public int score;
    public int highscore;

    [SerializeField] float scoreTextDistanceFromLeftUnder10;
    [SerializeField] float scoreTextDistanceFromLeftAbove10;
    [SerializeField] float scoreTextDistanceFromLeftAbove100;
    [SerializeField] float scoreTextDistanceFromLeftAbove1000;

    RectTransform scoreTextControllerCanvasRectTransform;

    void Start(){
        highscore = PlayerPrefs.GetInt("highscore");
        scoreTextControllerCanvasRectTransform = scoreTextControllerCanvas.GetComponent<RectTransform>();
    }

    void Update(){ // if the score becomes big, the score text is moved to the left, so that it does not go out of the screen
        scoreTextControllerCanvas.text = score.ToString();
        if(score<10)
        {
            scoreTextControllerCanvasRectTransform.anchoredPosition = new Vector2(scoreTextDistanceFromLeftUnder10,scoreTextControllerCanvasRectTransform.anchoredPosition.y);
        }
        else if(score<100)
        {
           scoreTextControllerCanvasRectTransform.anchoredPosition = new Vector2(scoreTextDistanceFromLeftAbove10,scoreTextControllerCanvasRectTransform.anchoredPosition.y);
        }
        else if(score<1000)
        {
            scoreTextControllerCanvasRectTransform.anchoredPosition = new Vector2(scoreTextDistanceFromLeftAbove100,scoreTextControllerCanvasRectTransform.anchoredPosition.y);
        }
        else
        {
            scoreTextControllerCanvasRectTransform.anchoredPosition = new Vector2(scoreTextDistanceFromLeftAbove1000,scoreTextControllerCanvasRectTransform.anchoredPosition.y);
        }
        
    }

    public void SetHighscore()
    {
        if(score>highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("highscore",highscore);
            newHighscoreText.SetActive(true);
        }
    }

}