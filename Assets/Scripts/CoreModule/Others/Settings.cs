using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the settings, that are set in the Settings menu, like language, volume or tutorials
/// </summary>

public class Settings : MonoBehaviour
{
    [SerializeField] Slider VolumeSlider;

    [SerializeField] Toggle TutorialToggle;

    [SerializeField] Image SoundButton;

    [SerializeField] Sprite MuteButton;
    [SerializeField] Sprite UnMuteButton;

    float previousVolume;

     void Awake()
     {
        if (PlayerPrefs.HasKey("volume"))
        {
            float volume = PlayerPrefs.GetFloat("volume");
            VolumeSlider.value = volume;
            previousVolume = PlayerPrefs.GetFloat("previousVolume");
        }
        if(PlayerPrefs.HasKey("istutorialMuseumOn"))
        {
            TutorialToggle.isOn = PlayerPrefs.GetInt("istutorialMuseumOn") == 1;
        }
        else
        {
            TutorialToggle.isOn = true;
            PlayerPrefs.SetInt("istutorialMuseumOn", 1);
        }
        
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("volume", volume);
        if(volume == 0)
        {
            SoundButton.sprite = UnMuteButton;
        }
        else
        {
            SoundButton.sprite = MuteButton;
        }
        if(volume!=0)
        {
            previousVolume = volume;
            PlayerPrefs.SetFloat("previousVolume",volume);
        }
        VolumeSlider.value = volume;
    }

    public void Mute()
    {
        if(AudioListener.volume==0)
        {
            AudioListener.volume = previousVolume;
            VolumeSlider.value = previousVolume;
            SoundButton.sprite = MuteButton;
        }
        else
        {
            AudioListener.volume = 0;
            VolumeSlider.value = 0;
            SoundButton.sprite = UnMuteButton;
        }
        PlayerPrefs.SetFloat("volume", AudioListener.volume);
        
    }

    public void SetTutorial(bool value)
    {
        PlayerPrefs.SetInt("istutorialMuseumOn", value ? 1 : 0);
    }
}