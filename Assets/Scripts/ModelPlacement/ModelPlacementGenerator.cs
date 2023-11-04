using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

/// <summary>
/// Generates the model placement, when a model's button is pressed
/// </summary>

public class ModelPlacementGenerator : MonoBehaviour
{
    [SerializeField] GameObject LoadingImage;
    [SerializeField] GameObject NoInternetConnectionText;
    [SerializeField] GameObject NoInternetImage;
    [SerializeField] GameObject ModelPlacementCategoriesListOnline;
    [SerializeField] GameObject ModelPlacementCategoriesListSaved;
    [SerializeField] GameObject LowerPanelMenu;
    [SerializeField] GameObject LoadingScreen;
    [SerializeField] GameObject DirectoryDeleteHandlerMenuOpenerModelPlacement;
    [SerializeField] GameObject ScaleButton;
    [SerializeField] GameObject TutorialContainer;
    [SerializeField] GameObject ModelPlacementTutorial;
    [SerializeField] GameObject TutorialMenuInApp;
    [SerializeField] GameObject DefaultMidAirIndicatorClone;
    GameObject CurrentPositionerInstance;
    GameObject Model;

    [SerializeField] Transform ARCamera;

    [SerializeField] Text LoadingText;
    [SerializeField] Text TutorialTextUp;

    [SerializeField] RectTransform ModelPlacementMainMenu;
    [SerializeField] RectTransform ModelPlacementModelsListOnline;
    [SerializeField] RectTransform ModelPlacementModelsListSaved;
    [SerializeField] RectTransform ModelInfo;

    [SerializeField] Dropdown ModelPlacementModeDropdown;
    [SerializeField] Dropdown ContentPositioningModeDropdown;

    [SerializeField] Button SaveButton;
    [SerializeField] Button BackFromCameraButton;
    [SerializeField] Button LoadingScreenBackButton;
    [SerializeField] Button AboutButtonInApp; 

    ColorBlock ColorBlockSavedButton;

    [SerializeField] string url;
    [SerializeField] string modelFileNameUrl;
    string modelUrl;
    string path;

    [SerializeField] string[] TutorialPanelTextStrings;
    [SerializeField] string[] TutorialPanelTextStringGroundPlane;
    [SerializeField] string[] TutorialPanelTextStringMidAir;

    [SerializeField] int modelPlacementMode;
    public int contentPositioningMode;

    [SerializeField] float minimumWaitingTimeForCheckingInternetConnection;

    public bool isCameraOpen;
    bool contentIsDownloaded;
    bool internetIsNotWorkingWhileInitializingVuforia;

    Action ActionExecutedWhenModelIsPlaced;

    [SerializeField] InternetCheckerWhileDownloading internetCheckerWhileDownloading;
    [SerializeField] FileLoader fileLoader;
    [SerializeField] Menu menu;
    [SerializeField] Language language;
    [SerializeField] StartAppHandler startAppHandler;
    [SerializeField] Loading loading;
    [SerializeField] ModelPlacementListGenerator modelPlacementListGenerator;
    [SerializeField] DropDownAnimationHandler dropDownAnimationHandler;
    [SerializeField] ModelDeleter modelDeleter;
    [SerializeField] StageGenerator stageGenerator;
    [SerializeField] ObjectMover objectMover;
    [SerializeField] LowerPanelInAppHandler lowerPanelInAppHandler;
    [SerializeField] ModelScaler modelScaler;
    [SerializeField] TutorialAnimationHandlerInApp tutorialAnimationHandlerInApp;

    public Model currentModel;

    void Start()
    {
        modelPlacementMode = PlayerPrefs.GetInt("modelPlacementMode");
        ModelPlacementModeDropdown.value = modelPlacementMode;
        contentPositioningMode = PlayerPrefs.GetInt("contentPositioningMode");
        ContentPositioningModeDropdown.value = contentPositioningMode;
        ColorBlockSavedButton = SaveButton.colors;

        ActionExecutedWhenModelIsPlaced = new Action(() =>
        {
            TutorialMenuInApp.SetActive(false);
            tutorialAnimationHandlerInApp.enabled = true;
        });
    }

    void Update() // Activates the tutorial text from the top of the screen, when the camera is opened and the model is not placed yet
    {
        if(isCameraOpen)
        {
            if( (CurrentPositionerInstance && CurrentPositionerInstance.transform.childCount == 1 && CurrentPositionerInstance.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled) || (DefaultMidAirIndicatorClone && DefaultMidAirIndicatorClone.activeSelf) )
            {
                language.ChangeTextBasedOnLanguage(TutorialTextUp, TutorialPanelTextStrings);
                ModelPlacementTutorial.SetActive(false);
            }
            else
            {
                if(modelPlacementMode == 0)
                {
                    language.ChangeTextBasedOnLanguage(TutorialTextUp, TutorialPanelTextStringGroundPlane);
                    ModelPlacementTutorial.SetActive(true);
                }
                else
                {
                    language.ChangeTextBasedOnLanguage(TutorialTextUp, TutorialPanelTextStringMidAir);
                    ModelPlacementTutorial.SetActive(true);
                }
            }
        }
        
    }

    public void BackFromModelPlacementMainMenu() // It is called when the BackFromModelPlacementMainMenuButton is pressed
    {
        menu.CloseMenu(ModelPlacementMainMenu);
        gameObject.SetActive(false);
    }

    public void SaveModel() // It is called when the LowerPanelInApp's SaveButton is pressed. This should be deleted because the model placement should use the same saving system as the museum's and that doesn't saving something after the camera is opened
    {
        if(!currentModel.isSaved)
        {
            UnityEngine.Debug.Log("Saving model");
            lowerPanelInAppHandler.ToggleButtonColors(SaveButton, false);
            currentModel.isSaved = true;
            modelPlacementListGenerator.savedModelDirectoriesList.Add(Application.persistentDataPath + "/Model placement/" + currentModel.category + "/" + currentModel.name);
        }
        else
        {
            UnityEngine.Debug.Log("Not saving model");
            lowerPanelInAppHandler.ToggleButtonColors(SaveButton, true);
            currentModel.isSaved = false;
            modelPlacementListGenerator.savedModelDirectoriesList.Remove(Application.persistentDataPath + "/Model placement/" + currentModel.category + "/" + currentModel.name);
        }
        
    }
    

    public void DeleteModelIfItShouldNotBeSaved()
    {
        if( currentModel!=null && !currentModel.isSaved )
        {
            if(File.Exists(currentModel.path + currentModel.name + "/model.glb"))
            {
                File.Delete(currentModel.path + currentModel.name + "/model.glb");
            }
        }
    }

    public void SetModelPlacementMode(int modelPlacementMode) // it is called from ModelPlacementModeDropdown
    {
        this.modelPlacementMode = modelPlacementMode;
        PlayerPrefs.SetInt("modelPlacementMode", modelPlacementMode);
    }

    public void SetContentPositioningMode(int contentPositioningMode) // It is called from the ContentPositioningModeDropdown
    {
        this.contentPositioningMode = contentPositioningMode;
        PlayerPrefs.SetInt("contentPositioningMode", contentPositioningMode);
    }

    public void GenerateModelPlacement(Model model) // It is called when a model's button is pressed, this method starts the model palcement generation
    {
        BackFromCameraButton.onClick.RemoveAllListeners();
        BackFromCameraButton.onClick.AddListener(() =>
        {
            BackFromModelPlacement();
        });
        LoadingScreenBackButton.onClick.RemoveAllListeners();
        LoadingScreenBackButton.onClick.AddListener(() =>
        {
            BackFromModelPlacement();
        });
        SaveButton.onClick.AddListener(() =>
        {
            SaveModel();
        });
        isCameraOpen = true;
        currentModel = model;
        ScaleButton.SetActive(true);
        if(currentModel.isInSavedModels)
        {
            startAppHandler.StartApp(ModelPlacementModelsListSaved);
            ModelPlacementCategoriesListSaved.SetActive(false);
        }
        else
        {
            startAppHandler.StartApp(ModelPlacementModelsListOnline);
            ModelPlacementCategoriesListOnline.SetActive(false);
        }
        if(currentModel.isSaved && SceneLoader.isVuforiaInitialized)
        {
            StartCoroutine(ModelPlacementGeneratorCoroutine());
        }
        else
        {
            StartCoroutine(WaitForAnimationToFinishAndStartModelPlacementGeneratorCoroutine());
        }
        ModelPlacementMainMenu.gameObject.SetActive(false);
        AboutButtonInApp.onClick.AddListener(() =>
        {
            DirectoryDeleteHandlerMenuOpenerModelPlacement.SetActive(false);
            menu.OpenMenu(ModelInfo);
            modelDeleter.isModelInfoOpenedFromModelsList = true;
            modelPlacementListGenerator.SetModelInfo(currentModel);
        });
        GetComponent<ModelScaler>().enabled = true;
    }

    public void BackFromModelPlacement() // It is called either from the LowerPanelInApp's BackFromCameraButton or the LoadingScreen's LoadingScreenBackButton
    {
        StopAllCoroutines();
        fileLoader.StopAllCoroutines();
        if(currentModel.isSaved)
        {
            internetCheckerWhileDownloading.ToggleInternetChecking(false);
        }
        DeleteModelIfItShouldNotBeSaved();
        if(currentModel.isInSavedModels)
        {
            menu.OpenMenu(ModelPlacementModelsListSaved);
        }
        else
        {
            menu.OpenMenu(ModelPlacementModelsListOnline);
        }   
        dropDownAnimationHandler.PlayDropDownAnimation();
        isCameraOpen = false;
        AboutButtonInApp.onClick.RemoveAllListeners();
        DirectoryDeleteHandlerMenuOpenerModelPlacement.SetActive(true);
        GetComponent<ModelScaler>().enabled = false;
        lowerPanelInAppHandler.ResetLowerPanelButtons();
        GetComponent<ModelScaler>().ClearLists();
        TutorialContainer.SetActive(true);
        Destroy(DefaultMidAirIndicatorClone);
        modelScaler.isModelPlaced = false;
        SaveButton.onClick.RemoveAllListeners();
        lowerPanelInAppHandler.ToggleButtonColors(SaveButton, true);
        LoadingText.text = string.Empty;
        internetCheckerWhileDownloading.ToggleInternetChecking(false);
        tutorialAnimationHandlerInApp.isFirstTimeThatAModelIsShown = true;
        StartCoroutine(WaitForAnimationToFinishAndToggleMenu()); // Some things are dissapeared for a short amount of time before the camera is closed, which doesn't look good, so this method waits for the animation to finish and then it toggles the menu
    }

    IEnumerator WaitForAnimationToFinishAndToggleMenu()
    {
        yield return new WaitForSeconds(menu.animationTime);
        modelScaler.DeactivateSizeCanvases();
        if(modelScaler.InstantiatedSizeCanvases.Count > 0)
        {
            modelScaler.DestroyInstantiatedSizeCanvases();
        }
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        if(Model)
        {
            Destroy(Model);
        }
        if(contentPositioningMode == 2)
        {
            GameObject[] Stages = GameObject.FindGameObjectsWithTag("Stage");
            foreach(GameObject Stage in Stages)
            {
                Destroy(Stage);
            }
        }
        else
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        if(currentModel.isInSavedModels)
        {
            ModelPlacementCategoriesListSaved.SetActive(true);
        }
        else
        {
            ModelPlacementCategoriesListOnline.SetActive(true);
        }
        TutorialMenuInApp.SetActive(true);
        TutorialTextUp.gameObject.SetActive(false);
        ModelPlacementTutorial.SetActive(false);
        LoadingScreen.SetActive(true);
        ModelPlacementMainMenu.gameObject.SetActive(true);
        startAppHandler.ToggleMenu(true);
        tutorialAnimationHandlerInApp.enabled = false;
    }

    IEnumerator WaitForAnimationToFinishAndStartModelPlacementGeneratorCoroutine()
    {
        yield return new WaitForSeconds(menu.animationTime);
        StartCoroutine(ModelPlacementGeneratorCoroutine());
    }

    IEnumerator ModelPlacementGeneratorCoroutine()
    {
        yield return (null);
        LoadingText.gameObject.SetActive(true);
        modelUrl = url + currentModel.category + "/" + currentModel.name + "/" + modelFileNameUrl;
        UnityEngine.Debug.Log(modelUrl);
        path = Application.persistentDataPath + "/Model placement/" ;
        UnityEngine.Debug.Log(path);
        SaveButton.interactable = !currentModel.isSaved;
        if(currentModel.isSaved)
        {
            UnityEngine.Debug.Log("Model is saved");
            contentIsDownloaded = true;
            
        }
        else
        {   
            internetCheckerWhileDownloading.ToggleInternetChecking(true);
            internetCheckerWhileDownloading.ActionExecutedWhenInternetIsWorking = () => InternetConnection(); 
            internetCheckerWhileDownloading.ActionExecutedWhenInternetIsNotWorking = () => NoInternetConnection();
        }
        
        StartCoroutine(InitializeVuforia());
     
    }

    public void NoInternetConnection()
    {
        LoadingText.gameObject.SetActive(false);
        LoadingImage.SetActive(false);
        NoInternetConnectionText.SetActive(true);
        NoInternetImage.SetActive(true);
        language.ChangeTextBasedOnLanguage(NoInternetConnectionText.GetComponent<Text>(), "No internet connection", "Nu există conexiunea la internet");
        UnityEngine.Debug.Log("<color=red>Internet is not working</color>");
    }

    public void InternetConnection()
    {
        if(!internetIsNotWorkingWhileInitializingVuforia)
        {
            LoadingText.gameObject.SetActive(true);
            LoadingImage.SetActive(true);
            NoInternetConnectionText.SetActive(false);
            NoInternetImage.SetActive(false);
            UnityEngine.Debug.Log("<color=green>Internet is working</color>");
        }
    }

    IEnumerator InitializeVuforia()
    {
        if(!SceneLoader.isVuforiaInitialized)
        {
            language.ChangeTextBasedOnLanguage(LoadingText, "Initializing engine", "Inițializare");
            yield return StartCoroutine(VuforiaInitializationCoroutine());
            SceneLoader.isVuforiaInitialized = true;
        }
        VuforiaApplication.Instance.OnVuforiaInitialized += OnVuforiaInitialized;
        
    }

    IEnumerator VuforiaInitializationCoroutine()
    {
        yield return null;
        UnityEngine.Debug.Log("Loading vuforia");
        VuforiaApplication.Instance.Initialize(); 

    }

    void OnVuforiaInitialized(VuforiaInitError error)
    {
        UnityEngine.Debug.Log(error);
        if(contentIsDownloaded)
        {
            language.ChangeTextBasedOnLanguage(LoadingText, "Loading content", "Se încarcă conținutul");
            
        }
        else
        {
            language.ChangeTextBasedOnLanguage(LoadingText, "Downloading content", "Se descarcă conținutul");
           
        }
        
        StartCoroutine(StartModelDownloading());
    }


    IEnumerator StartModelDownloading()
    {
        Directory.CreateDirectory(path);
        UnityEngine.Debug.Log("Starting model downloading");
        language.ChangeTextBasedOnLanguage(LoadingText, "Downloading content", "Se descarcă conținutul");
        UnityEngine.Debug.Log(currentModel.path);
        yield return StartCoroutine(fileLoader.DownloadModelCoroutine(modelUrl, currentModel.path + currentModel.name + "/model.glb", "single", true, (GameObject Model) =>
        {
            if(Model)
            {
                this.Model = Model;
                objectMover.SetModel(Model.transform,"y");
                ToggleMeshRenderers(false);
                lowerPanelInAppHandler.ArrangeButtons("ModelPlacement");
                if(modelPlacementMode == 0)
                {
                    language.ChangeTextBasedOnLanguage(TutorialTextUp, TutorialPanelTextStringGroundPlane);
                    switch(contentPositioningMode)
                    {
                        case 0:
                            stageGenerator.GenerateStage(this.transform, "GroundPlane", "Once", Model, ActionExecutedWhenModelIsPlaced);
                            break;
                        case 1:
                            stageGenerator.GenerateStage(this.transform, "GroundPlane", "Movable", Model, ActionExecutedWhenModelIsPlaced);
                            break;
                        default:
                            stageGenerator.GenerateStage(this.transform, "GroundPlane", "Multiple", Model, ActionExecutedWhenModelIsPlaced);
                            break;
                    }
                }
                else
                {
                    language.ChangeTextBasedOnLanguage(TutorialTextUp, TutorialPanelTextStringMidAir);
                    switch(contentPositioningMode)
                    {
                        case 0:
                            stageGenerator.GenerateStage(this.transform, "MidAir", "Once", Model, ActionExecutedWhenModelIsPlaced);
                            break;
                        case 1:
                            stageGenerator.GenerateStage(this.transform, "MidAir", "Movable", Model, ActionExecutedWhenModelIsPlaced);
                            break;
                        default:
                            TutorialContainer.SetActive(false);
                            stageGenerator.GenerateStage(this.transform, "MidAir", "Multiple", Model, ActionExecutedWhenModelIsPlaced);
                            break;
                    }    
                    DefaultMidAirIndicatorClone = ARCamera.GetChild(1).gameObject;
                }
                CurrentPositionerInstance = stageGenerator.CurrentPositionerInstance;
                loading.GenerationIsOver();
                ModelPlacementTutorial.SetActive(true);
                TutorialTextUp.gameObject.SetActive(true);
                internetCheckerWhileDownloading.ToggleInternetChecking(false);
            }
            else
            {
                NoInternetConnection();
                StartCoroutine(StartModelDownloading());
            }
        }));
        
    }

    void ToggleChildren(Transform transform, bool value)
    {
        foreach(Transform Child in transform)
        {
            Child.gameObject.SetActive(value);
        }
    }

    void ToggleMeshRenderers(bool value)
    {
        MeshRenderer[] MeshRenderers = Model.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer MeshRenderer in MeshRenderers)
        {
            MeshRenderer.enabled = value;
        }
    }
}