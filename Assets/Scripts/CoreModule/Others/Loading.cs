using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// It handles the loading screen, while something is loaded/downloaded. 
/// </summary>

public class Loading : MonoBehaviour
{
    [SerializeField] GameObject LoadingScreen;
    [SerializeField] GameObject LoadingImage;
    [SerializeField] GameObject NoInternetImage;
    [SerializeField] GameObject LoadingText;
    [SerializeField] GameObject NoInternetConnectionText;
    [SerializeField] GameObject ProgressText;

    [SerializeField] Transform LowerPanelMenu;

    [SerializeField] InternetCheckerWhileDownloading internetCheckerWhileDownloading;

    public void ToggleLoadingScreen(bool isInternetWorking)
    {
        ToggleLoadingScreenInternetIsWorking(isInternetWorking);
        ToggleLoadingScreenInternetIsNotWorking(!isInternetWorking);

    }

    public void ToggleLoadingScreenInternetIsWorking(bool isInternetWorking)
    {
        LoadingImage.SetActive(isInternetWorking);
        LoadingText.SetActive(isInternetWorking);
        ProgressText.SetActive(isInternetWorking);
    }

    public void ToggleLoadingScreenInternetIsNotWorking(bool isInternetWorking)
    {
        NoInternetImage.SetActive(isInternetWorking);
        NoInternetConnectionText.SetActive(isInternetWorking);
    }

    public void GenerationIsOver()
    {
        Debug.Log("Generation is over");
        LoadingScreen.SetActive(false);
        internetCheckerWhileDownloading.ToggleInternetChecking(false);
    }
}