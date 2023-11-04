using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// In the Tutorial1, there is a link, which directs to a website, where the image wich should be detected is. This script is attached to the link and it opens the link in the browser. It can't be done using a simple button, because the text is underlined and it doesn't change color when its parent, (which is a button) is pressed
/// </summary>

public class URLOpenerOnPress : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Color normalColor;
    [SerializeField] Color pressedColor;

    [SerializeField] Text[] childTexts;

    public Button targetButton;

    public string url;

    void Start()
    {
        childTexts = GetComponentsInChildren<Text>(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Application.OpenURL(url);
        foreach(Text childText in childTexts)
        {
            childText.color = pressedColor;
        }
        
    }

    void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            setColor();
        }
       
    }

    void OnApplicationFocus(bool pause)
    {
        if(pause)
        {
            setColor();
        }
	}

    void setColor()
    {
        foreach(Text childText in childTexts)
        {
            childText.color = normalColor;
        }
    }
}
