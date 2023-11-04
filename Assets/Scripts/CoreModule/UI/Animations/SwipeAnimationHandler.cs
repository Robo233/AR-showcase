using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Creates a system, using the MenuAnimationController to navigate between elements using swipes and buttons
/// </summary>

public class SwipeAnimationHandler : MonoBehaviour
{
    Vector2 startPosition;
    Vector2 endPosition;

    Animator[] Animators;

    Button[] Buttons;

    [SerializeField] float minSwipeDistance = 50;
    [SerializeField] float animationDuration;

    [SerializeField] int currentAnimatorIndex;
    int previousAnimatorIndex;

    bool isAnimationPlaying;

    public Action<Button> ModifyPressedButtonAction;
    public Action<Button> ModifyPreviousButtonAction;

    public void SetAnimationSystem(int currentAnimatorIndex, Animator[] Animators, Button[] Buttons, Action<Button> ModifyPressedButtonAction, Action<Button> ModifyPreviousButtonAction)
    {
        this.currentAnimatorIndex = currentAnimatorIndex;
        this.enabled = true; // The scripts should be enabled and disabled properly, because the Update method checks constantly for swipe input
        this.Animators = Animators;
        this.Buttons = Buttons;
        this.ModifyPressedButtonAction = ModifyPressedButtonAction;
        this.ModifyPreviousButtonAction = ModifyPreviousButtonAction;
        animationDuration = Animators[0].runtimeAnimatorController.animationClips[0].length;
        if(currentAnimatorIndex != previousAnimatorIndex)
        {
            Debug.Log("Modifyinh previous button action");
            ModifyPreviousButtonAction(Buttons[previousAnimatorIndex]);
        }
        ModifyPressedButtonAction(Buttons[currentAnimatorIndex]);
        for(int i=0;i<Animators.Length;i++)
        {
            Animators[i].transform.GetChild(0).gameObject.SetActive(true);
        }
        SetTutorialPositions();
    }

    void SetTutorialPositions() // The tutorials are set to the correct position at the beginning
    {
        Debug.Log("Animators[currentAnimatorIndex].GetComponent<RectTransform>().anchoredPosition.x: " + Animators[currentAnimatorIndex].GetComponent<RectTransform>().anchoredPosition.x);
        if(Animators[currentAnimatorIndex].GetComponent<RectTransform>().anchoredPosition.x != 0)
        {
            PlayAnimation(Animators[currentAnimatorIndex], false, false, true, false);
        }
        
        for(int i=0;i<currentAnimatorIndex;i++)
        {
            Animators[i].Play("CenterToLeft", 0, 1);
            
        }

        for(int i=currentAnimatorIndex + 1;i<Animators.Length;i++)
        {
            Animators[i].Play("CenterToRight", 0, 1);
            
        }
    }

    void Update()
    {
        if(isAnimationPlaying)
        {
            return;
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endPosition = touch.position;
                Vector2 swipeDirection = endPosition - startPosition;
                float distance = Vector2.Distance(startPosition, endPosition);
                if (distance > minSwipeDistance)
                {
                    if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                    {
                        if (swipeDirection.x > 0) // goes right
                        {
                            if(currentAnimatorIndex > 0)
                            {
                                ChangeAnimation(false);
                                
                            }
                        }
                        else // goes left
                        {
                            if(currentAnimatorIndex < Animators.Length-1)
                            {
                                ChangeAnimation(true);
                            }
                            
                        }
                    }
                }
            }
        }
    }

    public void ChangeAnimation(bool isGoneLeft)
    {
        int directionIndex = isGoneLeft ? 1 : -1;
        PlayAnimation(Animators[currentAnimatorIndex], isGoneLeft, !isGoneLeft, false);
        PlayAnimation(Animators[currentAnimatorIndex+directionIndex], false, false, true);

        ModifyPressedButtonAction?.Invoke(Buttons[currentAnimatorIndex+directionIndex]);
        ModifyPreviousButtonAction?.Invoke(Buttons[currentAnimatorIndex]);

        currentAnimatorIndex += directionIndex;
        Animators[currentAnimatorIndex].transform.GetChild(0).gameObject.SetActive(true);
        if(Animators.Length < currentAnimatorIndex+directionIndex)
        {
            Animators[currentAnimatorIndex+directionIndex].transform.GetChild(0).gameObject.SetActive(true);
        }
        
    }

    public void ChangeAnimatedGameObjectFromButton() // It is called when a button is pressed
    {
        if(!EventSystem.current.currentSelectedGameObject)
        {
            return; // Sometimes the button is null and it causes runtime error
        }
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        int index = buttonName[buttonName.Length-1] - '0' - 1; // TODO: won't work if the buttonName ends with a double digit number
        if(index == currentAnimatorIndex)
        {
            return;
        }
        Animators[index].transform.GetChild(0).gameObject.SetActive(true);
        PlayAnimation(Animators[index], false, false, true);
        
        ModifyPressedButtonAction?.Invoke(Buttons[index]);
        ModifyPreviousButtonAction?.Invoke(Buttons[currentAnimatorIndex]);

        for(int i=0; i<Animators.Length; i++)
        {
            if(i == index)
            {
                continue;
            }

            if(i != currentAnimatorIndex)
            {
                Animators[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                Animators[i].transform.GetChild(0).gameObject.SetActive(true);
            }

            if(i < index)
            {
                Debug.Log(Animators[i].gameObject.name + "goes left");
                PlayAnimation(Animators[i], true, false, false);
            }
            else if(i > index)
            {
                Debug.Log(Animators[i].gameObject.name + "goes right");
                PlayAnimation(Animators[i], false, true, false);
            }
        }
        currentAnimatorIndex = index;
    }

    void PlayAnimation(Animator animator, bool isGoneLeft, bool isGoneRight, bool isCentered, bool setButtonsToUnInteractable = true)
    {
        if(setButtonsToUnInteractable)
        {
            foreach(Button button in Buttons)
            {
                button.interactable = false;
            }
            isAnimationPlaying = true;
        }
        animator.SetBool("isGoneLeft",isGoneLeft);
        animator.SetBool("isGoneRight",isGoneRight);
        animator.SetBool("isCentered",isCentered);
        if(setButtonsToUnInteractable)
        {
            StartCoroutine(WaitForAnimationToFinishAndSetButtonsToInteractable());
        }
    }

    IEnumerator WaitForAnimationToFinishAndSetButtonsToInteractable()
    {
        yield return new WaitForSeconds(animationDuration);
        foreach(Button button in Buttons)
        {
            button.interactable = true;
        }
        isAnimationPlaying = false;
    }

    void OnDisable()
    {
        previousAnimatorIndex = currentAnimatorIndex; // When the menu is opened, the previous button should be set to default form
    }
}
