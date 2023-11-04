using System.Collections;
using System;
using System.IO;
using System.Web;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

/// <summary>
/// Creates an imageTarget, from a texture from a url, and sets its width from a text from a url.
/// </summary>

public class ImageTargetGenerator : MonoBehaviour
{
    public GameObject CurrentImageTarget;

    Texture2D Texture;

    [SerializeField] int retryDelay;

    public string currentPath;
    string currentName;
    string imageURL;
    string imagePath;

    [SerializeField] Text loadingText;
    [SerializeField] FileLoader fileLoader;
    [SerializeField] Loading loading;

    public IEnumerator DownloadTextureAndGenerateImageTarget(GameObject Parent, string url, string path, float imageTargetWidth, Action<GameObject> OnImageTargetGenerated, int waitingTimeBeforeDownloadIsStarted = 0)
    {
        Debug.Log("imageURL: " + url + "/image.jpg");
        yield return new WaitForSeconds(waitingTimeBeforeDownloadIsStarted);
        yield return StartCoroutine(fileLoader.DownloadImageCoroutine(url + "/image.jpg", path + "/image.jpg", false, (texture) => 
        {
            if(texture)
            {
                loading.ToggleLoadingScreen(true);
                var ImageTarget = VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(texture, 0.1f, "ImageTarget" + Path.GetFileName(path)); // TODO: Useless, don't put the name like this
                CurrentImageTarget = ImageTarget.gameObject;
                CurrentImageTarget.transform.SetParent(Parent.transform);
                Debug.Log("ImageTarget is created: " + CurrentImageTarget.gameObject.name);
                CurrentImageTarget.GetComponent<ImageTargetBehaviour>().SetWidth(imageTargetWidth);
                OnImageTargetGenerated(CurrentImageTarget);
                
            }
            else
            {
                loading.ToggleLoadingScreen(false);
                StartCoroutine(DownloadTextureAndGenerateImageTarget(Parent, url, path, imageTargetWidth, OnImageTargetGenerated, 1));
            }

        }));
    }

    public IEnumerator DownloadDetectionInfo(string url, string path, string detectionType, string detectionName, Action<string> OnDetectionInfoDownloaded = null, int waitingTimeBeforeDownloadIsStarted = 0)
    {
        yield return new WaitForSeconds(waitingTimeBeforeDownloadIsStarted);
        string detectionInfoUrl = url + HttpUtility.UrlEncode(detectionType) + "/" + HttpUtility.UrlEncode(detectionName) + "/detectionInfo.json"; // TODO: To make this function more reusable put the final url and path as argument and not with detectionType and detectionName
        string detectionInfoPath = path + "/" + detectionType + "/" + detectionName + "/detectionInfo.json";
        Debug.Log("detectionInfoUrl: " + detectionInfoUrl);
        yield return StartCoroutine(fileLoader.DownloadTextCoroutine(detectionInfoUrl, detectionInfoPath, false, (detectionInfo) =>{
            if(!string.IsNullOrEmpty(detectionInfo))
            {
                OnDetectionInfoDownloaded(detectionInfo);
            }
            else
            {
                StartCoroutine(DownloadDetectionInfo(url, path, detectionType, detectionName, OnDetectionInfoDownloaded, retryDelay));
            }

        }));
    }
}