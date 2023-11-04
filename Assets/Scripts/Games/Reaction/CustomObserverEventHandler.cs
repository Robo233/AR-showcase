using UnityEngine;

/// <summary>
/// A custom observer event handler that overrides the default one. This prevents the canvases to become disabled when the target is lost, this is becuase if they are disabled first and enabled later, they become blurred
/// </summary>

public class CustomObserverEventHandler : DefaultObserverEventHandler
{
    protected override void OnTrackingFound()
    {
        base.OnTrackingFound ();

    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost ();
        Canvas[] canvasComponents = GetComponentsInChildren<Canvas>();
        foreach (Canvas canvas in canvasComponents)
        {
            canvas.enabled = true;
        }
        
    }
}