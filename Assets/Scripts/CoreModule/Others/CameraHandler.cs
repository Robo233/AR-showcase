using System.Collections;
using UnityEngine;

/// <summary>
/// When the camera button from the lowerPanel is pressed, a screenshot is taken, the UI elements dissappear, so it looks like a photo
/// </summary>

public class CameraHandler : MonoBehaviour
{
    [SerializeField] Transform InAppMenus;

    int imageCount; // this is used to name the photos

    void Start()
    {
        imageCount = PlayerPrefs.GetInt("imageCount", 0);
    }

    public void TakeScreenShot()
    {
        InAppMenus.localScale = Vector3.zero;
        StartCoroutine(TakeAndSaveScreenshot());
    }

    IEnumerator TakeAndSaveScreenshot()
    {
        imageCount++;
        PlayerPrefs.SetInt("imageCount", imageCount);
        yield return new WaitForEndOfFrame();
        Texture2D ScreenImage = new Texture2D(Screen.width, Screen.height);
        ScreenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ScreenImage.Apply();
        byte[] imageBytes = ScreenImage.EncodeToJPG();
        NativeGallery.SaveImageToGallery(imageBytes, Application.productName, Application.productName + imageCount + ".jpg", null); // The photo can be found in the device's gallery
        InAppMenus.localScale = Vector3.one;
    }
}