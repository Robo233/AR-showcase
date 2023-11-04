using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// It is responsible for changing the language of the app
/// </summary>

public class Language : MonoBehaviour
{
    [SerializeField] Dropdown LanguageDropdown;

    [SerializeField] LanguageArrayHolder[] languageArrayHolders;

    int languageID;

    [SerializeField] string[] addedLanguages; // doesn't contain English, because it is the default language

    void Awake()
    {
        languageID = PlayerPrefs.GetInt("languageID",-1); // checks wheter the language was already set from the settings
        if(languageID==-1)
        {
            for(int i=0;i<addedLanguages.Length;i++)
            {
                if(addedLanguages[i] == Application.systemLanguage.ToString())
                {
                    ModifyTexts(i+1);
                }
            }
            if(languageID==-1) // it sets the language to english, because the device's language is either English, ot it is not supported by the app
            {
                SetLanguageFromDropDown(0);
            }
        }
        else
        {
            SetLanguageFromDropDown(languageID);
        }
        
    }

    public void SetLanguageFromDropDown(int languageID) // It is called from the dropdown, from the Settings menu
    {
        PlayerPrefs.SetInt("languageID", languageID);
        ModifyTexts(languageID);
          
    }

    void ModifyTexts(int languageID)
    { 
        this.languageID = languageID;
        LanguageDropdown.value = languageID;
        for(int i=0;i<languageArrayHolders.Length;i++)
        {
            languageArrayHolders[i].setText(languageID);
        }
        
    }

    public void ChangeTextBasedOnLanguage(Text text, params string[] texts)
    {
        text.text = texts[languageID]; 
        
    }

}

[System.Serializable] class LanguageArrayHolder
{
    public Text[] Texts = new Text[0];
    public string[] textStrings = new string[0];
    
    public void setText(int languageID)
    {
        for(int i=0;i<Texts.Length;i++)
        {
            Texts[i].text = textStrings[languageID];
        }
    }
}