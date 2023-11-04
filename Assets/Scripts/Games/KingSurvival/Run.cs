using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// THe run button is used to make the player run, this script should be attached to the RunButton. This can be handled by the PlayerMovement class, because we need to detect, whether the RunButton is pressed or not and this can be done only using the EventSystem's PointerHandlers
/// </summary>

public class Run : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerMovement playerMovement;

    public void OnPointerDown(PointerEventData eventData)
    {
        playerMovement.isRunning = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        playerMovement.isRunning = false;
    }
}