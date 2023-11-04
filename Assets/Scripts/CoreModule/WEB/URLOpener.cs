using UnityEngine;

/// <summary>
/// Contains a method which opens a website. This website will be opened in a browser of the device
/// </summary>

public class URLOpener : MonoBehaviour
{
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}