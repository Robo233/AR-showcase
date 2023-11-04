using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// It is used to load a scene asynchronously, in this way, a loading screen can be displayed while the scene is loading
/// </summary>

public class SceneLoader : MonoBehaviour
{
    public static bool isVuforiaInitialized;

    public void LoadScene(string sceneName) // It is called from the HomeButton on LowerPanel
    {
        StartCoroutine(LoadAsynchronously(sceneName));
    }

    public static IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        while(!asyncOperation.isDone)
        {
            Debug.Log(asyncOperation.progress);
            yield return null;
        }
    }
}