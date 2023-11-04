using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Contains a method which checks if the device is connected to the internet
/// </summary>

public class InternetConnectionHandler : MonoBehaviour
{
    [SerializeField] string url; // an url of a website that is always online, so the method does not say that the intenet is not working when it actually is working
    
    public IEnumerator CheckInternetConnection(Action<bool> action)
    {
        UnityWebRequest request = new UnityWebRequest(url);
        yield return request.SendWebRequest();
        if(request.error != null)
        {
            action (false);
        }
        else
        {
            action (true);
        }
    }

}