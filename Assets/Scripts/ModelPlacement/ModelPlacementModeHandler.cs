using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Sets the content of the ModelPlacementModeInfo menu, when the user presses the info button, either from a modelplacement mode or from a content positioning mode. The information is set about the current mode, so the user knows what the mode does
/// </summary>

public class ModelPlacementModeHandler : MonoBehaviour
{
    [SerializeField] RectTransform ModelPlacementModeInfo;

    [SerializeField] Text modelPlacementModeTitle;
    [SerializeField] Text modelPlacementModeDescription;

    [SerializeField] Dropdown modelPlacementModeDropdown;
    [SerializeField] Dropdown contentPositioningBehaviourModeDropdown;

    [SerializeField] Image RoundedImageModelPlacementInfo;

    [SerializeField] Sprite whiteSquare;
    [SerializeField] Sprite greenBall;

    [SerializeField] Menu menu;
    [SerializeField] Language language;

    [SerializeField] string[] modelPlacementModeDescriptionGroundPlaneTexts;
    [SerializeField] string[] modelPlacementModeDescriptionMidAirTexts;
    [SerializeField] string[] modelPlacementModeDescriptionOnceTexts;
    [SerializeField] string[] modelPlacementModeDescriptionMovableTexts;
    [SerializeField] string[] modelPlacementModeDescriptionMultipleTexts;
    
    public void SetModelPlacementModeInfo() // It is called from the ModeDropdownInfoButton
    {
        menu.OpenMenu(ModelPlacementModeInfo);
        string parentName = EventSystem.current.currentSelectedGameObject.transform.parent.name;
        string mode = parentName.Substring(parentName.IndexOf(':')+2);
        modelPlacementModeDropdown.Hide();
        RoundedImageModelPlacementInfo.transform.parent.parent.gameObject.SetActive(true);
        switch(mode)
        {
            case "Ground plane":
                modelPlacementModeTitle.text = "Ground Plane";
                language.ChangeTextBasedOnLanguage(modelPlacementModeDescription, modelPlacementModeDescriptionGroundPlaneTexts);
                RoundedImageModelPlacementInfo.sprite = whiteSquare;
                break;
            default:
                modelPlacementModeTitle.text = "Mid Air";
                language.ChangeTextBasedOnLanguage(modelPlacementModeDescription, modelPlacementModeDescriptionMidAirTexts);
                RoundedImageModelPlacementInfo.sprite = greenBall;
                break;
        }
    }

    public void SetContentPositioningBehaviourMode() // It is called from the ContentPositioningModeDropdownInfoButton
    {
        menu.OpenMenu(ModelPlacementModeInfo);
        string parentName = EventSystem.current.currentSelectedGameObject.transform.parent.name;
        string mode = parentName.Substring(parentName.IndexOf(':')+2);
        contentPositioningBehaviourModeDropdown.Hide();
        RoundedImageModelPlacementInfo.transform.parent.parent.gameObject.SetActive(false);
        switch(mode)
        {
            case "Once":
                modelPlacementModeTitle.text = "Once";
                language.ChangeTextBasedOnLanguage(modelPlacementModeDescription, modelPlacementModeDescriptionOnceTexts);
                break;
            case "Movable":
                modelPlacementModeTitle.text = "Movable";
                language.ChangeTextBasedOnLanguage(modelPlacementModeDescription, modelPlacementModeDescriptionMovableTexts);
                break;
            default:
                modelPlacementModeTitle.text = "Multiple";
                language.ChangeTextBasedOnLanguage(modelPlacementModeDescription, modelPlacementModeDescriptionMultipleTexts);
                break;
        }
    }
}