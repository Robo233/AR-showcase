using System;
using UnityEngine;

/// <summary>
/// Checks the internet connection while the app is downloading something, its role is to show the user in time that the internet is not working and the download won't finish until the internet is reactivated
/// </summary>

public class InternetCheckerWhileDownloading : MonoBehaviour
{
    [SerializeField] float internetCheckFrequencyWhileLoading;
    float timePassed;

    public bool isDownloading;

    public Action ActionExecutedWhenInternetIsWorking;
    public Action ActionExecutedWhenInternetIsNotWorking;

    [SerializeField] InternetConnectionHandler internetConnectionHandler;

    public void ToggleInternetChecking(bool startChecking) // This method start the internet checking, it is called when a download is started somewhere in the app
    {
        isDownloading = startChecking;
        timePassed = internetCheckFrequencyWhileLoading;
    }

    void Update()
    {
        if(isDownloading)
        {
            timePassed += Time.deltaTime;
            if(timePassed>internetCheckFrequencyWhileLoading)
            {
                Debug.Log("check");
                StartCoroutine(internetConnectionHandler.CheckInternetConnection((isConnected)=>
                {
                    if(isConnected)
                    {
                        Debug.Log("Internet is working");
                        ActionExecutedWhenInternetIsWorking.Invoke();    
                    }
                    else
                    { 
                        Debug.Log("Internet is not working");
                        ActionExecutedWhenInternetIsNotWorking.Invoke();
                    }
                }));
                timePassed = 0;
            }
        }
    }

}