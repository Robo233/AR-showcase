using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// It is used when when a part is started which is using the device's camera
/// </summary>

public class StartAppHandler : MonoBehaviour
{
    [SerializeField] GameObject LoadingScreen;
    [SerializeField] GameObject MainCanvas;
    [SerializeField] GameObject ScrollableListHome;
    [SerializeField] GameObject SettingsBackButton;
    [SerializeField] GameObject LoadingImage;
    [SerializeField] GameObject LoadingScreenBackButton;
    [SerializeField] GameObject LowerPanelMenu;

    [SerializeField] GameObject[] MainMenuElements;

    [SerializeField] Transform ScrollableListSettings;
    [SerializeField] Transform Settings;
    [SerializeField] Transform SettingsInApp;

    [SerializeField] Menu menu;

    public void StartApp(RectTransform currentMenuRectTransform) 
    {
        LoadingScreen.SetActive(true);
        LoadingImage.SetActive(true);
        LoadingScreenBackButton.SetActive(true);
        menu.CloseMenu(currentMenuRectTransform);
        ToggleMenu(false);
    }

    public void ToggleMenu(bool value)
    {
        LowerPanelMenu.SetActive(value);
        SettingsBackButton.SetActive(!value);
        if(value)
        {
            ScrollableListSettings.SetParent(Settings);
        }
        else
        {
            ScrollableListSettings.gameObject.SetActive(true);
            ScrollableListSettings.SetParent(SettingsInApp);
        }
        ScrollableListSettings.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        ScrollableListSettings.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        ScrollableListHome.SetActive(value);
    }
    
}