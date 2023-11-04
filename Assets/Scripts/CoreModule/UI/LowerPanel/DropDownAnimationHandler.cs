using UnityEngine;

/// <summary>
/// Handles the animation of the lower panel, when the camera is used
/// </summary>

public class DropDownAnimationHandler : MonoBehaviour
{
    [SerializeField] Animator LowerPanelInAppAnimator;

    Vector2 startTouchPosition;
    Vector2 endTouchPosition;

    int DropDownHash = Animator.StringToHash("DropDown");
    int DropUpHash = Animator.StringToHash("DropUp");

    [SerializeField] float minimumSwipeDistance; // Increase this, to make it harder to open the lower panel by swiping

    bool isDroppedDown = true;

    void Update()
    {
        if (Input.touchCount == 1 && (Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)  )
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;

                float swipeDistanceY = endTouchPosition.y - startTouchPosition.y;
                float swipeDistanceX = endTouchPosition.x - startTouchPosition.x;
                if (Mathf.Abs(swipeDistanceY) > minimumSwipeDistance && Mathf.Abs(swipeDistanceY) > Mathf.Abs(swipeDistanceX))
                {
                    if (isDroppedDown && swipeDistanceY > 0)
                    {
                        PlayDropUpAnimation();
                    }
                    else if(!isDroppedDown && swipeDistanceY < 0)
                    {
                        PlayDropDownAnimation();
                    }
                }
            }
        }
    }

    public void PlayDropDownOrDropUpAnimation() // It is called when DropDownButtonLowerPanelInApp is clicked
    {
        if(!isDroppedDown)
        {
            PlayDropDownAnimation();
        }
        else
        {
            PlayDropUpAnimation();
        }
    }

    public void PlayDropDownAnimation()
    {
        LowerPanelInAppAnimator.SetBool(DropUpHash,false);
        LowerPanelInAppAnimator.SetBool(DropDownHash,true);
        isDroppedDown = true;
    }

    public void PlayDropUpAnimation()
    {
        LowerPanelInAppAnimator.SetBool(DropUpHash,true);
        LowerPanelInAppAnimator.SetBool(DropDownHash,false);
        isDroppedDown = false;
    }

    

    
}
