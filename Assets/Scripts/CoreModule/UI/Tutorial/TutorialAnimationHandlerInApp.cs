using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// If tutorials are enabled in the settings, and a model is detected for the first time, the tutorial animations are shown. This scripts handles these animations
/// </summary>

public class TutorialAnimationHandlerInApp : MonoBehaviour
{
    [SerializeField] GameObject LowerPanelInApp;
    [SerializeField] GameObject MultipleAnimationsController;

    [SerializeField] Transform TutorialBackground;

    [SerializeField] Transform[] TutorialHandContainers;
    [SerializeField] Transform[] TutorialTexts;
    [SerializeField] Transform[] TutorialContainers;

    [SerializeField] Color LightBlue;
    [SerializeField] Color Blue;

    [SerializeField] float tutorialHandContainerPositionY;
    [SerializeField] float tutorialTextPositionY;
    [SerializeField] float tutorialTextPositionTutorialMenuY;

    int currentTutorialIndex = 0;

    public bool isFirstTimeThatAModelIsShown = true;

    void OnEnable() // The tutorials are moved from the TutorialMenu to the TutorialBackground, in case the tutorial is activated
    {
        if(isFirstTimeThatAModelIsShown && PlayerPrefs.GetInt("istutorialMuseumOn") == 1)
        {
            LowerPanelInApp.SetActive(false);
            MultipleAnimationsController.SetActive(false);
            TutorialBackground.gameObject.SetActive(true);
            for(int i=currentTutorialIndex;i<TutorialHandContainers.Length;i++)
            {
                TutorialHandContainers[i].SetParent(TutorialBackground);
                TutorialHandContainers[i].localPosition = new Vector3(0, tutorialHandContainerPositionY, 0);
                TutorialHandContainers[i].gameObject.SetActive(TutorialHandContainers[i] == TutorialHandContainers[currentTutorialIndex]);
                TutorialTexts[i].SetParent(TutorialBackground);
                TutorialTexts[i].localPosition = new Vector3(0, tutorialTextPositionY, 0);
                TutorialTexts[i].gameObject.SetActive(TutorialTexts[i] == TutorialTexts[currentTutorialIndex]);
                TutorialTexts[i].GetComponent<Text>().color = Blue; // The color of the text is changed to blue, bcause the camera is bright, and if the texts is set to lightblue, the contrast is not big enough
            }

        }
    }

    void Update() // Shows the next tutorial when the user presses the screen
    {
        if(Input.GetMouseButtonDown(0))
        {
            SetTutorialBackToTutorialMenu(currentTutorialIndex);
            if(TutorialHandContainers.Length>currentTutorialIndex+1)
            {
                TutorialHandContainers[currentTutorialIndex+1].gameObject.SetActive(true);
                TutorialTexts[currentTutorialIndex+1].gameObject.SetActive(true);
            }
            if(TutorialHandContainers[currentTutorialIndex] == TutorialHandContainers[TutorialHandContainers.Length-1])
            {
                isFirstTimeThatAModelIsShown = false;
                currentTutorialIndex = 0;
                this.enabled = false;
                return;
            }
            currentTutorialIndex++;
            
        }
    }

    void SetTutorialBackToTutorialMenu(int currentTutorialIndex)
    {
        TutorialHandContainers[currentTutorialIndex].SetParent(TutorialContainers[currentTutorialIndex]);
        TutorialHandContainers[currentTutorialIndex].localPosition = Vector3.zero;
        TutorialTexts[currentTutorialIndex].SetParent(TutorialContainers[currentTutorialIndex]);
        TutorialTexts[currentTutorialIndex].localPosition = new Vector3(0, tutorialTextPositionTutorialMenuY, 0);
        TutorialTexts[currentTutorialIndex].GetComponent<Text>().color = LightBlue;
    }

    void OnDisable()
    {
        LowerPanelInApp.SetActive(true);
        MultipleAnimationsController.SetActive(true);
        TutorialBackground.gameObject.SetActive(false);
    }
}