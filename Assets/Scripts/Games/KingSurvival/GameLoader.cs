using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains methods which download files from a server and then use them in the game. It is used as a template for downloading games from the web
/// </summary>

public abstract class GameLoader : MonoBehaviour
{
    [SerializeField] protected UnityEngine.UI.Image[] Images;

    [SerializeField] protected AudioSource[] AudioSources;

    protected Action<GameObject>[] Actions;

    [SerializeField] FileLoader fileLoader;
    [SerializeField] InternetCheckerWhileDownloading internetCheckerWhileDownloading;

    int imageIndex;
    int audioIndex;
    int modelIndex;

    [SerializeField] string url;
    protected string path;

    [SerializeField] string[] modelNames;
    [SerializeField] string[] imageNames;
    [SerializeField] string[] audioNames;

    protected void StartGameDownload(string gameName)
    {
        if(!PlayerPrefs.HasKey(gameName))
        {
            internetCheckerWhileDownloading.ToggleInternetChecking(true);
            internetCheckerWhileDownloading.ActionExecutedWhenInternetIsWorking = () => { ToggleInternetConnectionMenu(true); };
            internetCheckerWhileDownloading.ActionExecutedWhenInternetIsNotWorking = () => { ToggleInternetConnectionMenu(false); };
        }
        StartCoroutine(DownloadImage());
    }

    IEnumerator DownloadImage()
    {
        if(imageIndex<imageNames.Length)
        {   
            string currentImagePath = path + "Images";
            Directory.CreateDirectory(currentImagePath);
            yield return StartCoroutine(fileLoader.DownloadImageCoroutine(url + "Images/" + imageNames[imageIndex] + ".png", currentImagePath + "/" + imageNames[imageIndex] + ".png", true, (texture) => 
            {
                if(texture)
                {
                    ToggleInternetConnectionMenu(true);
                    Images[imageIndex++].sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(1, 1));
                    StartCoroutine(DownloadImage());
                }
                else
                {
                    ToggleInternetConnectionMenu(false);
                    StartCoroutine(DownloadImage());
                }
            }));
            
        }
        else
        {
           StartCoroutine(DownloadAudio());
        }
        
    }

    IEnumerator DownloadAudio()
    {
        if (audioIndex < audioNames.Length)
        {
            string currentAudioPath = path + "Audio";
            Directory.CreateDirectory(currentAudioPath);
            yield return StartCoroutine(fileLoader.DownloadAudioCoroutine(url + "Audio/" + audioNames[audioIndex] + ".mp3", currentAudioPath + "/" + audioNames[audioIndex] + ".mp3", true, (AudioClip audioClip) =>
            {
                if(audioClip)
                {
                    ToggleInternetConnectionMenu(true);
                    AudioSources[audioIndex++].clip = audioClip;
                    StartCoroutine(DownloadAudio());
                }   
                else
                {
                    ToggleInternetConnectionMenu(false);
                    StartCoroutine(DownloadAudio());
                }
            }));
            
        }
        else
        {
            StartCoroutine(DownloadModel());
        }
    }

    IEnumerator DownloadModel()
    {
        if (modelIndex < modelNames.Length)
        {
            string currentModelPath = path + "Models";
            Directory.CreateDirectory(currentModelPath);
            yield return StartCoroutine(fileLoader.DownloadModelCoroutine(url + "Models/" + modelNames[modelIndex] + ".glb", currentModelPath + "/" + modelNames[modelIndex] + ".glb", "single", true, (GameObject model) =>
            {
                if(model)
                {
                    ToggleInternetConnectionMenu(true);
                    Actions[modelIndex++]?.Invoke(model);
                    StartCoroutine(DownloadModel());
                }
                else
                {
                    ToggleInternetConnectionMenu(false);
                    StartCoroutine(DownloadModel());
                }
            }));
            
        }
        else
        {
            GenerationIsOver();
            internetCheckerWhileDownloading.ToggleInternetChecking(false);
        }

    }

    protected abstract void GenerationIsOver(); // this is overriden depending on what should happen if the game is downloaded

    protected abstract void ToggleInternetConnectionMenu(bool isInternetWorking); // this is overriden depending on what should happen if the internet connection is lost depending on the current game

    public void AddComponents(GameObject target, params System.Type[] Components)
    {
        foreach (System.Type Component in Components)
        {
            target.AddComponent(Component);
        }
    }
    
}