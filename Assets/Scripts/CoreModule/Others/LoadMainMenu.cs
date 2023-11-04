using System.Collections;
using UnityEngine;

/// <summary>
/// When the app is first started, the "Loading" scene is the one which is loaded. After the FadeIn animation is played, the main scene is loaded
/// </summary>

public class LoadMainMenu : MonoBehaviour
{
    [SerializeField] Animation FadeInAnimation;

    void Start()
    {
        StartCoroutine(WaitForAnimationAndLoadScene());
    }

    IEnumerator WaitForAnimationAndLoadScene()
    {
        FadeInAnimation.Play();
        yield return new WaitForSeconds(FadeInAnimation.clip.length);
        StartCoroutine(SceneLoader.LoadAsynchronously("MainMenu"));
    }
}