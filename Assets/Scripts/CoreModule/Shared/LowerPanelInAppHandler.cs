using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Modifies the position of the buttons of the lower panel, depending on the mode
/// </summary>

public class LowerPanelInAppHandler : MonoBehaviour
{
    [SerializeField] RectTransform ScaleButton;
    [SerializeField] RectTransform SaveButton;
    [SerializeField] RectTransform TutorialButton;

    [SerializeField] Canvas MainCanvas;

    [SerializeField] Color normalColorButton;
    [SerializeField] Color selectedColorButton;

    [SerializeField] List<RectTransform> UpperPartButtons = new List<RectTransform>();
    [SerializeField] List<RectTransform> LowerPartButtons = new List<RectTransform>();

    [SerializeField] float buttonHeight;
    [SerializeField] float upperButtonPositionY;
    [SerializeField] float lowerButtonPositionY;

    public void ArrangeButtons(string mode)
    {
        if(!UpperPartButtons.Contains(ScaleButton))
        {
            UpperPartButtons.Insert(UpperPartButtons.Count, ScaleButton);
        }
        if(!UpperPartButtons.Contains(SaveButton))
        {
            UpperPartButtons.Insert(UpperPartButtons.Count, SaveButton);
        }
        if(!LowerPartButtons.Contains(TutorialButton))
        {
            LowerPartButtons.Insert(LowerPartButtons.Count, TutorialButton);
        }
            
        TutorialButton.anchoredPosition = new Vector2(TutorialButton.anchoredPosition.x, lowerButtonPositionY);

        float upperPartWidth = MainCanvas.GetComponent<CanvasScaler>().referenceResolution.x/UpperPartButtons.Count;
        for(int i=0; i<UpperPartButtons.Count; i++)
        {
            UpperPartButtons[i].sizeDelta = new Vector2(upperPartWidth, buttonHeight);
            UpperPartButtons[i].anchoredPosition = new Vector2((i+1)*(UpperPartButtons[i].rect.width) - UpperPartButtons[i].rect.width/2, UpperPartButtons[i].anchoredPosition.y);
        }

        float lowerPartWidth = MainCanvas.GetComponent<CanvasScaler>().referenceResolution.x/LowerPartButtons.Count;
        for(int i=0; i<LowerPartButtons.Count; i++)
        {
            LowerPartButtons[i].sizeDelta = new Vector2(lowerPartWidth, buttonHeight);
            LowerPartButtons[i].anchoredPosition = new Vector2((i+1)*LowerPartButtons[i].rect.width - LowerPartButtons[i].rect.width/2, LowerPartButtons[i].anchoredPosition.y);
        }
    }

    public void ResetLowerPanelButtons()
    {
        UpperPartButtons.Remove(ScaleButton);
        UpperPartButtons.Remove(TutorialButton);
        LowerPartButtons.Remove(TutorialButton);
    }

    public void ToggleButtonColors(Button button, bool value)
    {   
        ColorBlock colorBlock = button.colors;
        if(value)
        {
            colorBlock.selectedColor = normalColorButton;
            colorBlock.normalColor = normalColorButton;
            colorBlock.highlightedColor = normalColorButton;
        }
        else
        {
            colorBlock.selectedColor = selectedColorButton;
            colorBlock.normalColor = selectedColorButton;
            colorBlock.highlightedColor = selectedColorButton;
        }
        button.colors = colorBlock;
    }

}