using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generates the list of models after a category is selected
/// </summary>

public class ModelPlacementListGenerator : MonoBehaviour
{
    GameObject PreviousRegionNthInstance;
    [SerializeField] GameObject RoundedButton;
    [SerializeField] GameObject RegionN;
    [SerializeField] GameObject NoSavedModelsText; 
    [SerializeField] GameObject NoInternetConnectionContainerModelsListOnline;

    List<GameObject> CurrentRegions;
    public List<Button> CategoryButtonsSaved = new List<Button>();
    public List<Button> CategoryButtonsOnline = new List<Button>();
    Dictionary<string, List<GameObject>> AlreadyLoadedRegionsOnlineModels = new Dictionary<string, List<GameObject>>();
    List<GameObject> AlreadyLoadedRegionsSavedModels = new List<GameObject>();

    [SerializeField] Transform GridContentModelPlacementModelsListOnline;
    [SerializeField] Transform GridContentModelPlacementModelsListSaved;

    [SerializeField] RectTransform ModelInfo;
    [SerializeField] RectTransform ModelPlacementModelsListOnline;
    [SerializeField] RectTransform ModelPlacementModelsListSaved;
    [SerializeField] RectTransform ModelPlacementCategoriesListOnline;
    [SerializeField] RectTransform ModelPlacementCategoriesListSaved;

    [SerializeField] Color LightBlue;

    [SerializeField] Text ModelInfoTitle;
    [SerializeField] Text ModelDescriptionText;
    [SerializeField] Text ModelPlacementModelsListOnlineTitle;
    [SerializeField] Text ModelPlacementModelsListSavedTitle;

    [SerializeField] Image[] RoundedImagesModelInfo;

    [SerializeField] Button DirectoryDeleteHandlerMenuOpenerModelPlacement;

    string path;
    string categoryName;
    string currentUrl;
    [SerializeField] string url;
    [SerializeField] string descriptionFileNameUrl;

    string[] modelPaths;
    string[] modelNames;
    string[] modelDescriptions;

    public List<string> savedModelDirectoriesList = new List<string>();
    List<string> savedCategoryDirectoriesList;
    List<string> categoryDirectories;
    List<string> remainedDirectoriesAfterDeletingDirectoriesThatDoesNotContainModels = new List<string>();

    [SerializeField] int assetCount;
    int loadedModelsCount;
    int savedLoadedModelsCount;
    int savedModelIndex;
    int modelIndex;
    [SerializeField] List<int> savedModelIndices = new List<int>();

    [SerializeField] float roundedButtonMargin;

    public bool isModelInfoMenuOpen;
    public bool isListGenerated;
    public bool isModelPlacementListOnlineOpened;
    public bool isModelPlacementListSavedOpened;
    bool areThereSavedModels;
    bool isCategorySaved;
    bool isModelSaved;

    [SerializeField] FileLoader fileLoader;
    [SerializeField] InternetCheckerWhileDownloading internetCheckerWhileDownloading;
    [SerializeField] Menu menu;
    [SerializeField] ModelPlacementGenerator modelPlacementGenerator;
    [SerializeField] ModelDeleter modelDeleter;
    [SerializeField] Language language;
    [SerializeField] DirectoryHandler directoryHandler;

    void Start()
    {
        path = Application.persistentDataPath + "/Model placement";
        if(Directory.Exists(path))
        {
            savedCategoryDirectoriesList = new List<string>(Directory.GetDirectories(path));
        }
        else
        {
            savedCategoryDirectoriesList = new List<string>();
        }
        foreach(string savedCategoryDirectory in savedCategoryDirectoriesList)
        {
            savedModelDirectoriesList.AddRange(Directory.GetDirectories(savedCategoryDirectory));
        }
        for (int i = 0; i < savedModelDirectoriesList.Count; i++)
        {
            savedModelDirectoriesList[i] = savedModelDirectoriesList[i].Replace('\\', '/');
        }
        
    }

    IEnumerator WaitForAnimationToFinishAndDestroySavedRegionsCoroutine()
    {
        yield return new WaitForSeconds(menu.animationTime);
        DestroySavedRegions();
        ToggleCategoryButtonsSaved(true);
    }

    public void DestroySavedRegions()
    {
        foreach(GameObject Region in AlreadyLoadedRegionsSavedModels)
        {
            Destroy(Region);
        }
        AlreadyLoadedRegionsSavedModels = new List<GameObject>();
    }

    public void DestroyOnlineRegions()
    {
        if(AlreadyLoadedRegionsOnlineModels.TryGetValue(categoryName, out List<GameObject> Regions))
        {
            foreach(GameObject Region in Regions)
            {
                Destroy(Region);
            }
            AlreadyLoadedRegionsOnlineModels = new Dictionary<string, List<GameObject>>();
        }
    }

    public void StartModelListGeneration(string categoryName, string[] modelNames, bool isCategorySaved) // It is called, when a category's button is pressed
    {
        loadedModelsCount = 0;
        if(isCategorySaved)
        {
            ToggleCategoryButtonsSaved(false);
            menu.OpenMenu(ModelPlacementModelsListSaved);
            ModelPlacementModelsListSavedTitle.text = categoryName;
        }
        else
        {
            ToggleCategoryButtonsOnline(false);
            menu.OpenMenu(ModelPlacementModelsListOnline);
            ModelPlacementModelsListOnlineTitle.text = categoryName;
        }
        this.categoryName = categoryName;
        this.isCategorySaved = isCategorySaved;
        UnityEngine.Debug.Log("categoryName: " + categoryName);
        if(isCategorySaved)
        {
            isModelPlacementListSavedOpened = true;
            CurrentRegions = new List<GameObject>();
            this.assetCount = modelNames.Length;
            this.modelNames = modelNames;
            path = Application.persistentDataPath + "/Model placement/" + categoryName + "/";
            currentUrl = url + categoryName + "/";
            StartCoroutine(DownloadModelPlacementList(1));
        }
        else
        {
            isModelPlacementListOnlineOpened = true;
            if(!AlreadyLoadedRegionsOnlineModels.ContainsKey(categoryName))
            {
                CurrentRegions = new List<GameObject>();
                this.assetCount = modelNames.Length;
                this.modelNames = modelNames;
                path = Application.persistentDataPath + "/Model placement/" + categoryName + "/";
                currentUrl = url + categoryName + "/";
                UnityEngine.Debug.Log("DownloadModelPlacementList is started");
                internetCheckerWhileDownloading.ToggleInternetChecking(true);
                internetCheckerWhileDownloading.ActionExecutedWhenInternetIsWorking = () => Debug.Log("Internet is working"); 
                internetCheckerWhileDownloading.ActionExecutedWhenInternetIsNotWorking = () => Debug.Log("Internet is not working"); 
                StartCoroutine(DownloadModelPlacementList(1));
            }
            else
            {
                if(AlreadyLoadedRegionsOnlineModels.TryGetValue(categoryName, out List<GameObject> Regions))
                {
                    foreach(GameObject Region in Regions)
                    {
                        Region.SetActive(true);
                    }
                }
            }
        }  
    
       
    }

    void InternetConnection()
    {
        NoInternetConnectionContainerModelsListOnline.SetActive(false);
        UnityEngine.Debug.Log("Internet connection");

    }

    void NoInternetConnection()
    {
        NoInternetConnectionContainerModelsListOnline.SetActive(true);
        UnityEngine.Debug.Log("No internet connection"); 
        
    }

    IEnumerator DownloadModelPlacementList(int modelIndex)
    {
        this.modelIndex = modelIndex;
        if(modelIndex<=assetCount)
        {
            Directory.CreateDirectory(path + modelNames[modelIndex-1]);
            isModelSaved = isCategorySaved || File.Exists(path + modelNames[modelIndex-1] + "/model.glb");
            Debug.Log("image url: " + currentUrl + modelNames[modelIndex-1] + "/image.jpg");
            yield return StartCoroutine(fileLoader.DownloadImageCoroutine(currentUrl + modelNames[modelIndex-1] + "/image.jpg", path + modelNames[modelIndex-1] + "/image.jpg", true, (texture) => 
            {
                if(texture != null)
                {
                    StartCoroutine(fileLoader.DownloadTextCoroutine(currentUrl + modelNames[modelIndex-1] + "/" + descriptionFileNameUrl, path + modelNames[modelIndex-1] + "/description.txt", true, (text) =>
                    {
                        if(text != null)
                        {
                            LoadModelButton(new Model(modelNames[modelIndex-1], text, categoryName, path, Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), new Vector2(1,1)), isCategorySaved, isModelSaved ) );
                            StartCoroutine(DownloadModelPlacementList(modelIndex+1));
                        }
                        else
                        {
                            UnityEngine.Debug.Log("Text is null");
                            StartCoroutine(DownloadModelPlacementList(modelIndex));
                        }
                    }));
                }
                else
                {
                    UnityEngine.Debug.Log("Image is null");
                    StartCoroutine(DownloadModelPlacementList(modelIndex));
                }
            }));    
            
        }
        else
        {
            UnityEngine.Debug.Log("All models are loaded");
            isListGenerated = true;
            if(isCategorySaved)
            {
                AlreadyLoadedRegionsSavedModels.AddRange(CurrentRegions);
            }
            else
            {
                AlreadyLoadedRegionsOnlineModels.Add(categoryName, CurrentRegions);
            }
            internetCheckerWhileDownloading.ToggleInternetChecking(false);
        }
    }

    void LoadModelButton(Model model)
    {
        GameObject ModelButtonInstance = Instantiate(RoundedButton);
        GameObject RegionNthInstance;
        if(loadedModelsCount%2==0)
        {
            if(model.isInSavedModels)
            {
                RegionNthInstance = Instantiate(RegionN, GridContentModelPlacementModelsListSaved);
                RegionNthInstance.name = "SavedModelsRegion" + (loadedModelsCount/2);
            }
            else
            {
                RegionNthInstance = Instantiate(RegionN, GridContentModelPlacementModelsListOnline);
                RegionNthInstance.name = "AvailableOnlineRegion" + (loadedModelsCount/2);
            }
            CurrentRegions.Add(RegionNthInstance);
            PreviousRegionNthInstance = RegionNthInstance;
            RegionNthInstance.transform.SetAsLastSibling();
            ModelButtonInstance.transform.SetParent(RegionNthInstance.transform);
            ModelButtonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(-roundedButtonMargin,0);
        }
        else
        {
            ModelButtonInstance.transform.SetParent(PreviousRegionNthInstance.transform);
            ModelButtonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(roundedButtonMargin,0);
        }
        ModelButtonInstance.GetComponent<Button>().onClick.AddListener(() =>
        {
            modelPlacementGenerator.GenerateModelPlacement(model);
        });
        ModelButtonInstance.transform.GetChild(3).gameObject.SetActive(false);
        ModelButtonInstance.transform.GetChild(0).GetComponent<Text>().text = model.name;
        for(int i=ModelButtonInstance.transform.childCount-2;i>2;i--)
        {
            ModelButtonInstance.transform.GetChild(i).gameObject.SetActive(true);
            ModelButtonInstance.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = model.Sprite;
        }
        ModelButtonInstance.GetComponent<Image>().color = LightBlue;
        ModelButtonInstance.transform.GetChild(ModelButtonInstance.transform.childCount-2).GetComponent<Image>().preserveAspect = true;
        ModelButtonInstance.transform.localScale = new Vector3(1,1,1);
        ModelButtonInstance.transform.GetChild(ModelButtonInstance.transform.childCount-1).gameObject.SetActive(true);
        ModelButtonInstance.transform.GetChild(ModelButtonInstance.transform.childCount-1).GetComponent<Button>().onClick.AddListener(() =>
        {
            menu.OpenMenu(ModelInfo);
            DirectoryDeleteHandlerMenuOpenerModelPlacement.interactable = model.isSaved;
            modelDeleter.model = model;
            isModelInfoMenuOpen = true;
            SetModelInfo(model);
        });
        loadedModelsCount++;
        
    }

    IEnumerator WaitForAnimationToFinishAndDeactivateOnlineRegions()
    {
        yield return new WaitForSeconds(menu.animationTime);
        foreach(List<GameObject> regions in AlreadyLoadedRegionsOnlineModels.Values)
        {
            foreach(GameObject region in regions)
            {
                region.SetActive(false);
            }
        }
        ToggleCategoryButtonsOnline(true);
        
    }

    public void SetModelInfo(Model model)
    {
        ModelInfoTitle.text = model.name;
        ModelDescriptionText.text = model.description;
        foreach(Image RoundedImageModelInfo in RoundedImagesModelInfo)
        {
            RoundedImageModelInfo.sprite = model.Sprite;
        }
    }

    void DeleteCategoryDirectoriesExceptThoseThatContainAtleastOneModelDirectory()
    {
        string modelPlacementPath = Application.persistentDataPath + "/Model placement";
        if(File.Exists(modelPlacementPath + "/categoryCount.txt"))
        {
            File.Delete(modelPlacementPath + "/categoryCount.txt");
        }
        if(Directory.Exists(modelPlacementPath))
        {
            string[] categoryDirectories = Directory.GetDirectories(modelPlacementPath);
            foreach(string categoryDirectory in categoryDirectories)
            { 
                string[] modelDirectories = Directory.GetDirectories(categoryDirectory);
                List<string> modelDirectoriesThatContainModels  = new List<string>();
                foreach(string modelDirectoy in modelDirectories)
                {
                    if(File.Exists(modelDirectoy + "/model.glb"))
                    {
                        modelDirectoriesThatContainModels.Add(modelDirectoy);
                    }
                }
                if(modelDirectoriesThatContainModels.Count == 0)
                {
                    directoryHandler.DeleteDirectory(categoryDirectory);
                }
                else
                {
                    remainedDirectoriesAfterDeletingDirectoriesThatDoesNotContainModels.Add(categoryDirectory);
                }        
            }
            if(Directory.GetDirectories(modelPlacementPath).Length == 0)
            {
                Directory.Delete(modelPlacementPath);
            }
        }
        
        DeleteModelDirectoriesExceptSavedOnes();
    }

    void DeleteModelDirectoriesExceptSavedOnes()
    {
        foreach(string remainedDirectoryAfterDeletingDirectoriesThatDoesNotContainModels in remainedDirectoriesAfterDeletingDirectoriesThatDoesNotContainModels)
        {
            string[] modelDirectories = Directory.GetDirectories(remainedDirectoryAfterDeletingDirectoriesThatDoesNotContainModels);
            foreach(string modelDirectory in modelDirectories)
            {
                bool wasTheSameModelFound = false;
                foreach(string savedModelDirectory in savedModelDirectoriesList)
                {
                    if(string.Equals(Path.GetFullPath(modelDirectory), Path.GetFullPath(savedModelDirectory), StringComparison.OrdinalIgnoreCase))
                    {
                        wasTheSameModelFound = true;
                    }
                }
                if(!wasTheSameModelFound)
                {
                    directoryHandler.DeleteDirectory(modelDirectory);
                }
            }
            if(Directory.GetDirectories(remainedDirectoryAfterDeletingDirectoriesThatDoesNotContainModels).Length == 0)
            {
                Directory.Delete(remainedDirectoryAfterDeletingDirectoriesThatDoesNotContainModels);
            }
        }
    }

    public void BackFromModelPlacementListOnline()
    {
        isModelPlacementListOnlineOpened = false;
        menu.CloseMenuAndSetCurrentCanvas(ModelPlacementModelsListOnline, ModelPlacementCategoriesListOnline);
        StartCoroutine(WaitForAnimationToFinishAndDeactivateOnlineRegions());
    }

    public void BackFromModelPlacementListSaved()
    {
        isModelPlacementListSavedOpened = false;
        menu.CloseMenuAndSetCurrentCanvas(ModelPlacementModelsListSaved, ModelPlacementCategoriesListSaved);
        StartCoroutine(WaitForAnimationToFinishAndDestroySavedRegionsCoroutine());
    }

    string GetNthRowFromText(string text, int n)
    {
        return text.Split(new []{ Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[n];
    }

    void ToggleCategoryButtonsOnline(bool value)
    {
        foreach(Button categoryButtonOnline in CategoryButtonsOnline)
        {
            categoryButtonOnline.interactable = value;
        }
    }

    void ToggleCategoryButtonsSaved(bool value)
    {
        foreach(Button CategoryButtonSaved in CategoryButtonsSaved)
        {
            CategoryButtonSaved.interactable = value;
        }
    }

    void OnDisable()
    {
        modelPlacementGenerator.DeleteModelIfItShouldNotBeSaved();
        DeleteCategoryDirectoriesExceptThoseThatContainAtleastOneModelDirectory();
    }

    void OnApplicationPause() // use this instead of OnApplicationQuit
    {
        modelPlacementGenerator.DeleteModelIfItShouldNotBeSaved();
        DeleteCategoryDirectoriesExceptThoseThatContainAtleastOneModelDirectory();
    }

}