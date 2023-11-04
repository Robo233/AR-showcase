using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the naviagation between differnet menus
/// </summary>

public class Menu : MonoBehaviour
{
    public RectTransform CurrentMenu;

    [SerializeField] RectTransform MainCanvas;
    [SerializeField] RectTransform Home;

    // Games
    [SerializeField] RectTransform GameInfo;
    [SerializeField] RectTransform DirectoryDeleteHandlerMenuGame;

    // Model placement
    [SerializeField] RectTransform ModelInfoMenu;
    [SerializeField] RectTransform ModelPlacementMainMenu;
    [SerializeField] RectTransform ModelPlacementModelsListOnline;
    [SerializeField] RectTransform ModelPlacementModelsListSaved;
    [SerializeField] RectTransform ModelPlacementCategoriesListOnline;
    [SerializeField] RectTransform ModelPlacementCategoriesListSaved;
    [SerializeField] RectTransform DirectoryDeleteHandlerMenuModelPlacement;

    [SerializeField] GraphicRaycaster graphicRaycaster;

    public List<RectTransform> Menus;
    Dictionary<RectTransform, Action> ActionsExecutedBasedOnCurrentMenu;

    public float animationTime;

    float canvasHeight;

    static string currentMenuName;

    public bool isAtLeastOneMenuOpen;

    [SerializeField] StartAppHandler startAppHandler;

    // Games
    [SerializeField] GameHandler gameHandler;

    // Model placement
    [SerializeField] CategoryListGenerator categoryListGenerator;
    [SerializeField] ModelPlacementListGenerator modelPlacementListGenerator;
    [SerializeField] ModelPlacementGenerator modelPlacementGenerator;
    [SerializeField] ModelDeleter modelDeleter;

    void Start()
    {
        PositionMenus(); // Each menu is scattered around the scene, and this method puts the menus, where they should be

        ActionsExecutedBasedOnCurrentMenu = new Dictionary<RectTransform, Action>
        {

            // Games
            { GameInfo, () => gameHandler.BackFromGameInfo() },
            { DirectoryDeleteHandlerMenuGame, () => gameHandler.BackFromDirectoryDeleteHandlerMenuGame() },

            // Model placement
            { DirectoryDeleteHandlerMenuModelPlacement, () => modelDeleter.BackFromDirectoryDeleteHandlerMenuModelPlacement() },
            { ModelInfoMenu, () => modelDeleter.BackFromModelInfoMenu() },
            { ModelPlacementMainMenu, () => modelPlacementGenerator.BackFromModelPlacementMainMenu() },
            { ModelPlacementModelsListOnline, () => modelPlacementListGenerator.BackFromModelPlacementListOnline() },
            { ModelPlacementModelsListSaved, () => modelPlacementListGenerator.BackFromModelPlacementListSaved() },
            { ModelPlacementCategoriesListOnline, () => categoryListGenerator.BackFromCategoriesOnline() },
            { ModelPlacementCategoriesListSaved, () => categoryListGenerator.BackFromCategoriesSaved() },
            
        };

        Debug.Log("currentMenuName: " + currentMenuName);
        if(string.IsNullOrEmpty(currentMenuName))
        {
            CurrentMenu = Home;
            isAtLeastOneMenuOpen = false;
            Debug.Log("isAtLeastOneMenuOpen: " + isAtLeastOneMenuOpen);
        }
        else
        {
            CurrentMenu = GameObject.Find(currentMenuName).GetComponent<RectTransform>();
            isAtLeastOneMenuOpen = true;
            Debug.Log("isAtLeastOneMenuOpen: " + isAtLeastOneMenuOpen);
        }

    }

    public void PositionMenus()
    {

        Debug.Log("Positioning menus");
        canvasHeight = MainCanvas.sizeDelta.y;
        foreach(RectTransform Menu in Menus)
        {
            if(Menu != CurrentMenu)
            {
                Menu.offsetMin = new Vector2(0, -canvasHeight);
                Menu.offsetMax = new Vector2(0, -canvasHeight);
            }
            else
            {
                Menu.offsetMin = new Vector2(0, 0);
                Menu.offsetMax = new Vector2(0, 0);
            }
            
        }
    }

    void Update() // Handles the naviagation, using the back button of the device
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Back button pressed");
            if(modelPlacementGenerator.isCameraOpen)
            {
                if(isAtLeastOneMenuOpen)
                {
                    CloseMenu(CurrentMenu);
                }
                else
                {
                    modelPlacementGenerator.BackFromModelPlacement();
                }
            }
            else
            {
                Debug.Log("Inside else");
                if(ActionsExecutedBasedOnCurrentMenu.ContainsKey(CurrentMenu))
                {
                    Debug.Log("Executing action on " + CurrentMenu.name);
                    ActionsExecutedBasedOnCurrentMenu[CurrentMenu]();
                }
                else if (isAtLeastOneMenuOpen)
                {
                    Debug.Log("Closing " + CurrentMenu.name);
                    CloseMenu(CurrentMenu);
                }
                else
                {
                    Application.Quit();
                    Debug.Log("Quit");
                }
            }
        }
    }

    public void SetCurrentCanvasName(string currentMenuName)
    {
        Menu.currentMenuName = currentMenuName;
    }

    public void OpenMenu(RectTransform menuRectTransform)
    {
        Debug.Log("Opening " + menuRectTransform.name);
        CurrentMenu = menuRectTransform;
        isAtLeastOneMenuOpen = true;
        PlayAnimationBottomToCenter(menuRectTransform);
    }

    public void CloseMenu(RectTransform menuRectTransform)
    {
        CurrentMenu = Home;
        isAtLeastOneMenuOpen = false;
        currentMenuName = string.Empty;
        PlayMenuAnimationCenterToBottom(menuRectTransform);
    }

    public void CloseMenuAndSetCurrentCanvas(RectTransform menuRectTransform, RectTransform CurrentMenu) // it is used, when the user closes a menu, but he is not in the main menu, because another menu is still open
    {
        this.CurrentMenu = CurrentMenu;
        isAtLeastOneMenuOpen = true;
        PlayMenuAnimationCenterToBottom(menuRectTransform);
    }

    public void PlayAnimationBottomToCenter(RectTransform rectTransform)
    {
        Debug.Log("Inside PlayAnimationBottomToCenter");
        StartCoroutine(ModifyOffsetMinAndMaxOfRectTransform(rectTransform, 0));
    }
    
    public void PlayMenuAnimationCenterToBottom(RectTransform rectTransform)
    {
        StartCoroutine(ModifyOffsetMinAndMaxOfRectTransform(rectTransform, -canvasHeight));
    }

    public IEnumerator ModifyOffsetMinAndMaxOfRectTransform(RectTransform rectTransform, float endValue)
    {   
        Debug.Log("Inside ModifyOffsetMinAndMaxOfRectTransform");
        Debug.Log("endValue for" + rectTransform.name + " is " + endValue);
        graphicRaycaster.enabled = false;
        float elapsedTime = 0;
        Vector2 startOffsetMin = rectTransform.offsetMin;
        Vector2 startOffsetMax = rectTransform.offsetMax;
        Vector2 endOffsetMin = new Vector2(rectTransform.offsetMin.x, endValue);
        Vector2 endOffsetMax = new Vector2(rectTransform.offsetMax.x, endValue);

        while(elapsedTime < animationTime)
        {
            rectTransform.offsetMin = Vector2.Lerp(startOffsetMin, endOffsetMin, elapsedTime / animationTime);
            rectTransform.offsetMax = Vector2.Lerp(startOffsetMax, endOffsetMax, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        rectTransform.offsetMin = endOffsetMin;
        rectTransform.offsetMax = endOffsetMax;
        Debug.Log("rectTransform.offsetMin for" + rectTransform.name + " is " + rectTransform.offsetMin);
        graphicRaycaster.enabled = true;
    }
}