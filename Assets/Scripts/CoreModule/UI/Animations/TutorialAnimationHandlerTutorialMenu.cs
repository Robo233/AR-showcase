using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// When the user presses the "Tutorial" button on the lower panel, the tutorial menu is opened. This scripts handles the animations of the tutorials
/// </summary>

public class TutorialAnimationHandlerTutorialMenu : MonoBehaviour
{
    [SerializeField] GameObject TutorialMenu;

    [SerializeField] Animator[] TutorialAnimators;

    [SerializeField] Button[] Dots;

    [SerializeField] Sprite Circle;
    [SerializeField] Sprite EmptyDot;

    [SerializeField] Menu menu;

    [SerializeField] SwipeAnimationHandler swipeAnimationHandler;

    public void OpenTutorialMenu() // It is called from TutorialButton
    {
        TutorialMenu.SetActive(true);
        menu.OpenMenu(TutorialMenu.GetComponent<RectTransform>());
        swipeAnimationHandler.SetAnimationSystem(0, TutorialAnimators, Dots, ModifyPressedButton, ModifyPreviousButton);
    }

    public void BackFromTutorialMenu() // It is called from TutorialMenuBackButton
    {
        menu.CloseMenu(TutorialMenu.GetComponent<RectTransform>());
        swipeAnimationHandler.enabled = false;
        StartCoroutine(WaitForAnimationToFinishAndDeactivateTutorialMenu());
    }

    IEnumerator WaitForAnimationToFinishAndDeactivateTutorialMenu()
    {
        yield return new WaitForSeconds(menu.animationTime);
        TutorialMenu.SetActive(false);
        
    }

    void ModifyPressedButton(Button PressedButton)
    {
        PressedButton.GetComponent<Image>().sprite = Circle;
    }

    void ModifyPreviousButton(Button PreviousButton)
    {
        PreviousButton.GetComponent<Image>().sprite = EmptyDot;
    }

}