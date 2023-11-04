using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Vuforia;

/// <summary>
/// Handles the start of the game. If the tutorial is actviated from the menu, the tutorial menu is shown first, if not, the game is started directly
/// </summary>

public class StartAppHandlerReaction : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject imageTargetVirtualButton;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject tutorialCanvas1;
    [SerializeField] GameObject tutorialCanvas2;

    [SerializeField] Button[] buttons;

    [SerializeField] Toggle tutorialToggle;

    public static bool isRestarted;
    public bool isTutorialSet;
    bool isImageDetected;
    bool isPlaying;

    string mode;

    [SerializeField] float minimumWaitingTime;

    [SerializeField] VirtualButtonHandler virtualButtonHandler;

    void Start()
    {
        int maximumCompletedLevel = PlayerPrefs.GetInt("maximumCompletedLevel", 1);
        for(int i=0;i<maximumCompletedLevel;i++)
        {
            buttons[i].interactable = true;
        }
        if(!SceneLoader.isVuforiaInitialized)
        {
            StartCoroutine(VuforiaInitializationCoroutine());
            VuforiaApplication.Instance.OnVuforiaInitialized += OnVuforiaInitialized;
        }
        else
        {
            loadingScreen.SetActive(false);
            mainMenu.SetActive(true);
        }

        if(PlayerPrefs.HasKey("tutorialReaction"))
        {
            if(PlayerPrefs.GetInt("tutorialReaction")==0)
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
            PlayerPrefs.SetInt("tutorialReaction",1);
            isTutorialSet = true;
        }

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPlaying)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                StartCoroutine(SceneLoader.LoadAsynchronously("MainMenu"));
            }
        }
        
    }

     public void ToggleTutorial() //It is called from the TutorialToggle toggle from the MainMenu
     {
        if(tutorialToggle.isOn)
        {
            isTutorialSet = true;
            PlayerPrefs.SetInt("tutorialReaction",1);
        }
        else
        {
            isTutorialSet = false;
            PlayerPrefs.SetInt("tutorialReaction",0);
        }
        
    }

    IEnumerator VuforiaInitializationCoroutine()
    {
        yield return null;
        VuforiaApplication.Instance.Initialize(); 

    }

    void OnVuforiaInitialized(VuforiaInitError error)
    {
        UnityEngine.Debug.Log(error);
        SceneLoader.isVuforiaInitialized = true;
        loadingScreen.SetActive(false);
        mainMenu.SetActive(true);
        
    }

    public void StartImageDetectionOrShowTutorial1() // It is called from the button1/button2... from the MainMenu
    {
        isPlaying = true;
        if(isTutorialSet)
        {
            tutorialCanvas1.SetActive(true);
        }else
        {
            imageTargetVirtualButton.SetActive(true);
        }
        mainMenu.SetActive(false);
        string currentButtonName = EventSystem.current.currentSelectedGameObject.name;
        virtualButtonHandler.currentLevel = int.Parse(currentButtonName.Substring(currentButtonName.Length - 1));
        
    }

    public void StartCountdownOrShowTutorial2() // It is called from the OKButton of the Tutorial1
    {
        if(!isImageDetected)
        {
            isImageDetected = true;
            if(isTutorialSet)
            {
                tutorialCanvas2.SetActive(true);
            }
            else
            {
                virtualButtonHandler.StartCountdown();
            }
        }
    }

}