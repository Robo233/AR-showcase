using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the deleting of the selected game
/// </summary>

public class GameHandler : MonoBehaviour // This is ugly hardcode, it works only for the KingSurvival, it should be generalized
{
    [SerializeField] GameObject DirectoryDeleteHandlerMenuOpenerGame;

    [SerializeField] RectTransform DirectoryDeleteHandlerMenuGame;
    [SerializeField] RectTransform GameInfo;
    [SerializeField] RectTransform GamesMenu;

    [SerializeField] Text GameInfoTitle;
    [SerializeField] Text GameInfoDescription;

    [SerializeField] Image RoundedImageGameInfo;

    [SerializeField] Sprite[] GameSprites;

    [SerializeField] string[] kingSurvivalDescriptions;
    [SerializeField] string[] reactionDescriptions;

    int kingSurvivalFileCount = 11;

    [SerializeField] Menu menu;
    [SerializeField] Language language;

    bool isKingSurvivalDownloaded = false;

    void Awake()
    {
        if( Directory.Exists(Application.persistentDataPath + "/Games/KingSurvival") && Directory.GetFiles(Application.persistentDataPath + "/Games/KingSurvival", "*.*" ,SearchOption.AllDirectories).Length == kingSurvivalFileCount)
        {
            isKingSurvivalDownloaded = true;
        }
    }

    public void OpenGameInfo(string gameName) // It is called from the InfoButton which is located on the button of each game in the GamesMenu
    {
        menu.OpenMenu(GameInfo);
        GameInfoTitle.text = gameName;
        if(gameName == "King Survival")
        {
            DirectoryDeleteHandlerMenuOpenerGame.SetActive(true);
            language.ChangeTextBasedOnLanguage(GameInfoDescription, kingSurvivalDescriptions);
            RoundedImageGameInfo.sprite = GameSprites[0];
            if(isKingSurvivalDownloaded)
            {
                DirectoryDeleteHandlerMenuOpenerGame.GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            DirectoryDeleteHandlerMenuOpenerGame.SetActive(false);
            language.ChangeTextBasedOnLanguage(GameInfoDescription, reactionDescriptions);
            RoundedImageGameInfo.sprite = GameSprites[1];
        }
        
    }

    public void BackFromGameInfo()
    {
        menu.CloseMenuAndSetCurrentCanvas(GameInfo, GamesMenu);
    }

    public void BackFromDirectoryDeleteHandlerMenuGame()
    {
        menu.CloseMenuAndSetCurrentCanvas(DirectoryDeleteHandlerMenuGame, GameInfo);
    }

    public void DeleteGame()
    {
        Directory.Delete(Application.persistentDataPath + "/Games/KingSurvival", true);
        DirectoryDeleteHandlerMenuOpenerGame.GetComponent<Button>().interactable = false;
        isKingSurvivalDownloaded = false;
        PlayerPrefs.DeleteKey("KingSurvival");
        BackFromDirectoryDeleteHandlerMenuGame();
    }
}
