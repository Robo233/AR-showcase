using System.Collections;
using UnityEngine;

/// <summary>
/// When the main scene is loaded, the screen fades out, if the scene is loaded again, the screen does not fade out and it is deactiated instantly
/// </summary>

public class FadeOutAnimationHandler : MonoBehaviour
{
    [SerializeField] Animation FadeOutAnimation;

    public static bool wasSceneLoadedBefore;

    void Awake()
    {
        if(wasSceneLoadedBefore)
        {
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(FadeOutScreen());
        } 
    }

    IEnumerator FadeOutScreen()
    {
        FadeOutAnimation.Play();
        yield return new WaitForSeconds(FadeOutAnimation.clip.length);
        gameObject.SetActive(false);
        wasSceneLoadedBefore = true;
    }
}