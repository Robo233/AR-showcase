using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This makes the naviagation possible between the main menus(Home, Settings and About)
/// </summary>

public class MainMenuSwipeAnimationHandler : MonoBehaviour
{
    [SerializeField] Color Color;
    [SerializeField] Color PressedColor;

    [SerializeField] Animator[] MenuAnimators;

    [SerializeField] Button[] LowerPanelButtons;

    [SerializeField] int scaledButtonFontSize;
    [SerializeField] int originalButtonFontSize; 

    [SerializeField] float scaledButtonSize;
    [SerializeField] float originalButtonSize; 

    [SerializeField] SwipeAnimationHandler swipeAnimationHandler;

    void Start()
    {
        SetAnimationSystemToMainMenu();
    }

    public void SetAnimationSystemToMainMenu() // It is called in the beginning, as well as when the user comes back to the home
    {
        swipeAnimationHandler.SetAnimationSystem(1, MenuAnimators, LowerPanelButtons, ModifyPressedButton, ModifyPreviousButton); 
    }

    void ModifyPressedButton(Button PressedButton)
    {
        Transform CurrentPressedButtonchild_0 = PressedButton.transform.GetChild(0);
        CurrentPressedButtonchild_0.GetComponent<RectTransform>().sizeDelta = new Vector2(scaledButtonSize,scaledButtonSize);
        CurrentPressedButtonchild_0.GetComponent<Image>().color = PressedColor;

        Transform CurrentPressedButtonchild_1 = PressedButton.transform.GetChild(1);
        CurrentPressedButtonchild_1.GetComponent<Text>().fontSize = scaledButtonFontSize;
        CurrentPressedButtonchild_1.GetComponent<Text>().color = PressedColor;
    }

    void ModifyPreviousButton(Button PreviousButton)
    {
        Transform PreviousSelectedButtonButtonchild_0 = PreviousButton.transform.GetChild(0);
        PreviousSelectedButtonButtonchild_0.GetComponent<RectTransform>().sizeDelta = new Vector2(originalButtonSize,originalButtonSize);
        PreviousSelectedButtonButtonchild_0.GetComponent<Image>().color = Color;

        Transform PreviousSelectedButtonButtonchild_1 = PreviousButton.transform.GetChild(1);
        PreviousSelectedButtonButtonchild_1.GetComponent<Text>().fontSize = originalButtonFontSize;
        PreviousSelectedButtonButtonchild_1.GetComponent<Text>().color = Color;
    }

}