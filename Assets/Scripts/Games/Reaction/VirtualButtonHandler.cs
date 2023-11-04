using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Vuforia;

/// <summary>
/// Handles the gameplay
/// </summary>

public class VirtualButtonHandler : MonoBehaviour
{
    
    [SerializeField] GameObject levelText;
    [SerializeField] GameObject gameOverText;
    [SerializeField] GameObject levelCompletedText;
    [SerializeField] GameObject causeOfGameOverText;
    [SerializeField] GameObject completedButtonsCountText;
    [SerializeField] GameObject countDownTimerText;
    [SerializeField] GameObject countDownText;
    [SerializeField] GameObject homeVirtualButton;
    [SerializeField] GameObject mainCanvas;

    [SerializeField] GameObject[] virtualButtons;
    [SerializeField] GameObject[] planes;

    [SerializeField] RectTransform finger;

    [SerializeField] Material correctMaterial;
    [SerializeField] Material incorrectMaterial;
    [SerializeField] Material defaultMaterial;

    [SerializeField] AudioSource correctButtonPressedSound;
    [SerializeField] AudioSource incorrectButtonPressedSound;
    [SerializeField] AudioSource BeepSound;

    public int currentScoreThatMustBeAchievedToCompleteLevel;
    public int currentScore;
    public int currentLevel;
    [SerializeField] int buttonToPressIndex;
    [SerializeField] int countdownTime;
    [SerializeField] int amountOfButtonsToPressWhenTutorialIsSetBeforeFingerDisappears;
    int lastPressedIndex;

    [SerializeField] int[] scoresThatMustBeAchievedToCompleteLevel;

    public float elapsedTime;
    [SerializeField] float initialTimeToExecuteLevel;
    [SerializeField] float waitingTimeAfterLevelIsOver;
    [SerializeField] float fingerOffsetX;
    float timeToExecuteLevel;

    bool isPlaying;

    [SerializeField] StartAppHandlerReaction startAppHandlerReaction;

    void Start()
    {
        foreach(GameObject button in virtualButtons)
        {
            button.GetComponent<VirtualButtonBehaviour>().RegisterOnButtonPressed(OnButtonPressed);
        }
        
    }

    public void StartCountdown() // It is called from the StartAppHandlerReaction class
    {
        levelCompletedText.SetActive(false);
        gameOverText.SetActive(false);
        causeOfGameOverText.SetActive(false);
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        countDownText.SetActive(true);
        for(int i = countdownTime; i > 0; i--)
        {
            countDownText.GetComponent<Text>().text = i.ToString();
            BeepSound.Play();
            yield return new WaitForSeconds(1);
        }
        StartCoroutine(PlayBeepSoundTwiceCoroutine());
        countDownText.SetActive(false);
        isPlaying = true;
        currentScoreThatMustBeAchievedToCompleteLevel = scoresThatMustBeAchievedToCompleteLevel[currentLevel - 1];
        StartGame();
        timeToExecuteLevel = initialTimeToExecuteLevel;
        
    }

    IEnumerator PlayBeepSoundTwiceCoroutine()
    {
        BeepSound.Play();
        yield return new WaitForSeconds(BeepSound.clip.length);
        BeepSound.Play();
    }

    void Update()
    {
        if(isPlaying)
        {
            if(currentScore == currentScoreThatMustBeAchievedToCompleteLevel)
            {
                LevelCompleted();
                
            }
            timeToExecuteLevel -= Time.deltaTime;
            if(timeToExecuteLevel<=0)
            {
                isPlaying = false;
                incorrectButtonPressedSound.Play();
                GameOver("You've run out of time!");
            }
            else
            {
                countDownTimerText.GetComponent<Text>().text = "Time: " + (int)(timeToExecuteLevel);
            }
        }
    }

    void StartGame()
    {
        ToggleVirtualButtons(false);
        completedButtonsCountText.GetComponent<Text>().text = "0/" + currentScoreThatMustBeAchievedToCompleteLevel;
        levelText.SetActive(true);
        levelText.GetComponent<Text>().text = "Level " + currentLevel;
        buttonToPressIndex = Random.Range(0, virtualButtons.Length);
        virtualButtons[buttonToPressIndex].transform.GetChild(0).GetComponent<MeshRenderer>().material = correctMaterial;
        if(startAppHandlerReaction.isTutorialSet)
        {
            finger.gameObject.SetActive(true);
            SetFingerPosition(virtualButtons[buttonToPressIndex]);
        }
        
    }

    void ToggleVirtualButtons(bool wasPlaying)
    {
        countDownTimerText.SetActive(!wasPlaying);
        completedButtonsCountText.SetActive(!wasPlaying);
        for(int i=0; i<virtualButtons.Length;i++)
        {
            virtualButtons[i].GetComponent<VirtualButtonBehaviour>().enabled = !wasPlaying;
            planes[i].SetActive(!wasPlaying);
            planes[i].GetComponent<MeshRenderer>().material = defaultMaterial;
        }
        if(wasPlaying)
        {
            if(startAppHandlerReaction.isTutorialSet)
            {
                finger.gameObject.SetActive(true);
                SetFingerPosition(homeVirtualButton);
            }
            homeVirtualButton.GetComponent<VirtualButtonBehaviour>().enabled = true;
            homeVirtualButton.transform.GetChild(0).gameObject.SetActive(true);
            StartCoroutine(ActivateHomeButtonAfterDelay());
        }
        levelText.SetActive(!wasPlaying);
        
    }

    IEnumerator ActivateHomeButtonAfterDelay()
    {
        yield return new WaitForSeconds(waitingTimeAfterLevelIsOver);
        homeVirtualButton.GetComponent<VirtualButtonBehaviour>().RegisterOnButtonPressed(vb => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        
    }

    public void ToggleVirtualButtonBehaviours(bool value)
    {
        foreach(GameObject virtualButton in virtualButtons){
            virtualButton.GetComponent<VirtualButtonBehaviour>().enabled = value;
        }
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        if(isPlaying)
        {
            if(vb.gameObject == virtualButtons[buttonToPressIndex]) // correct button is pressed
            {
                currentScore++;
                correctButtonPressedSound.Play();
                vb.transform.GetChild(0).GetComponent<MeshRenderer>().material = defaultMaterial;
                lastPressedIndex = buttonToPressIndex;
                buttonToPressIndex = GetNextButtonIndex();
                virtualButtons[buttonToPressIndex].transform.GetChild(0).GetComponent<MeshRenderer>().material = correctMaterial;
                if(startAppHandlerReaction.isTutorialSet && currentScore<amountOfButtonsToPressWhenTutorialIsSetBeforeFingerDisappears)
                {
                    SetFingerPosition(virtualButtons[buttonToPressIndex]);
                }
                else
                {
                    finger.gameObject.SetActive(false);
                }
                Debug.Log(currentScore);
                completedButtonsCountText.GetComponent<Text>().text = currentScore + "/" + currentScoreThatMustBeAchievedToCompleteLevel;
            }
            else // incorrect button is pressed
            {
                isPlaying = false;
                incorrectButtonPressedSound.Play();
                vb.transform.GetChild(0).GetComponent<MeshRenderer>().material = incorrectMaterial;
                StartCoroutine(GameOverCorutineWrongButtonPressed());
            } 
        }
          
    }


    void SetFingerPosition(GameObject currentButton) // Sets the finger position to the button which should be pressed, when the tutorial is enabled
    {
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(finger.parent as RectTransform, RectTransformUtility.WorldToScreenPoint(Camera.main, currentButton.transform.position), Camera.main, out canvasPosition);
        finger.anchoredPosition = new Vector2(canvasPosition.x + fingerOffsetX, finger.anchoredPosition.y);
    }

    IEnumerator GameOverCorutineWrongButtonPressed()
    {
        yield return new WaitForSeconds(waitingTimeAfterLevelIsOver);
        GameOver("You've pressed the wrong button!");
    }

    int GetNextButtonIndex()
    {
        int nextIndex = Random.Range(0, virtualButtons.Length);
        while(nextIndex == buttonToPressIndex || nextIndex == lastPressedIndex)
        {
            nextIndex = Random.Range(0, virtualButtons.Length);
        }
        return nextIndex;
    }

    int getNextButtonIndex()
    {
        List<int> nums = new List<int>();
        for(int i = 0; i < virtualButtons.Length; i++)
        {
            if(i!=lastPressedIndex)
            {
                nums.Add(i);
            }
        }
        int randomNumber = nums[Random.Range(0, nums.Count)];
        return randomNumber;
    }

    public void Home(VirtualButtonBehaviour vb)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void LevelCompleted()
    {
        isPlaying = false;
        ToggleVirtualButtons(true);
        levelCompletedText.SetActive(true);
        PlayerPrefs.SetInt("maximumCompletedLevel", currentLevel+1);

    }

    void GameOver(string causeOfGameOver)
    {
        finger.gameObject.SetActive(false);
        ToggleVirtualButtons(true);
        gameOverText.SetActive(true);
        causeOfGameOverText.SetActive(true);
        causeOfGameOverText.GetComponent<Text>().text = causeOfGameOver;
      
    }
}