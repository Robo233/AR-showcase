using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the generation of the categories list, when the SavedModelsButton or the OnlineModelsButton is pressed
/// </summary>

public class CategoryListGenerator : MonoBehaviour
{
    [SerializeField] GameObject RoundedButton;
    [SerializeField] GameObject NoSavedModelsText;
    [SerializeField] GameObject RegionN;
    [SerializeField] GameObject NoInternetConnectionContainerCategoriesListOnline;
    GameObject PreviousRegionNthInstanceOnlineModels;
    GameObject PreviousRegionNthInstanceSavedModels;
    
    List<GameObject> AlreadyLoadedRegionsSavedCategories = new List<GameObject>();
    List<GameObject> AlreadyLoadedRegionsOnlineCategories = new List<GameObject>();

    [SerializeField] Transform GridContentModelPlacementCategoriesListOnline;
    [SerializeField] Transform GridContentModelPlacementCategoriesListSaved;

    [SerializeField] RectTransform ModelPlacementMainMenu;
    [SerializeField] RectTransform ModelPlacementCategoriesListOnline;
    [SerializeField] RectTransform ModelPlacementCategoriesListSaved;

    [SerializeField] Color DarkPurple;
    [SerializeField] Color LightBlue;

    [SerializeField] string url;
    [SerializeField] string categoryCountFileNameUrl;
    [SerializeField] string categoryImageFileNameUrl;
    [SerializeField] string modelCountFileNameUrl;
    string path;

    string[] categoryNames;
    string[] modelNames;
    string[] modelDescriptions;

    [SerializeField] int categoryAmount;
    int categoryIndex;
    int loadedCategoriesCountOnlineModels;
    int loadedCategoriesCountSavedModels;

    int[] modelAmounts;

    [SerializeField] float roundedButtonMargin;

    public bool isCategoriesLisSavedOpened;
    public bool isCategoriesListOnlineOpened;
    public bool wasCategoriesListOpened;
    bool areThereSavedCategories;
    [SerializeField] bool areThereSavedModels;

    [SerializeField] FileLoader fileLoader;
    [SerializeField] InternetCheckerWhileDownloading internetCheckerWhileDownloading;
    [SerializeField] ModelPlacementListGenerator modelPlacementListGenerator;
    [SerializeField] Menu menu;
    [SerializeField] ModelDeleter modelDeleter;
    [SerializeField] EncryptionHelper encryptionHelper;

    void Start()
    {
        path = Application.persistentDataPath + "/Model placement/";
    }

    IEnumerator WaitForAnimationToFinishAndDestroySavedRegionsCoroutine()
    {
        yield return new WaitForSeconds(menu.animationTime);
        DestroySavedRegions();
    }

    public void DestroySavedRegions()
    {
        foreach(GameObject Region in AlreadyLoadedRegionsSavedCategories)
        {
            Destroy(Region);
        }
        modelPlacementListGenerator.CategoryButtonsSaved = new List<Button>();
    }

     public void DestroyOnlineRegions()
     {
        foreach(GameObject AlreadyLoadedRegionsOnlineCategory in AlreadyLoadedRegionsOnlineCategories)
        {
            Destroy(AlreadyLoadedRegionsOnlineCategory);
        }
        AlreadyLoadedRegionsOnlineCategories = new List<GameObject>();
        modelPlacementListGenerator.CategoryButtonsOnline = new List<Button>();
    }

    public void CheckIfThereAreSavedModelsAndGenerateSavedCategoriesList() // It is called when the SavedModelsButton is pressed
    {
        menu.OpenMenu(ModelPlacementCategoriesListSaved);
        isCategoriesLisSavedOpened = true;
        loadedCategoriesCountSavedModels = 0;
        if(Directory.Exists(path))
        {
            string[] categoryDirectories = Directory.GetDirectories(path);
            Dictionary<string, List<string>> categoriesAndItsModels = new Dictionary<string, List<string>>();
            foreach(string categoryDirectory in categoryDirectories)
            {
                UnityEngine.Debug.Log("categoryDirectory: " + categoryDirectory);
                string[] modelDirectories = Directory.GetDirectories(categoryDirectory);
                List<string> savedModelDirectoriesList = new List<string>();
                foreach(string modelDirectory in modelDirectories)
                {
                    UnityEngine.Debug.Log("modelDirectory: " + modelDirectory);
                    if(File.Exists(Path.Combine(modelDirectory, "model.glb")))
                    {
                        UnityEngine.Debug.Log("modelDirectory exists: " + Path.Combine(modelDirectory, "model.glb"));
                        savedModelDirectoriesList.Add(modelDirectory);
                    }
                    else
                    {
                        UnityEngine.Debug.Log("modelDirectory doesn't exist: " + Path.Combine(modelDirectory, "model.glb"));
                    }
                }
                if(savedModelDirectoriesList.Count > 0)
                {
                    UnityEngine.Debug.Log("savedModelDirectoriesList.Count: " + savedModelDirectoriesList.Count);
                    categoriesAndItsModels.Add(categoryDirectory, savedModelDirectoriesList);
                }
                else
                {
                    UnityEngine.Debug.Log("savedModelDirectoriesList.Count: " + savedModelDirectoriesList.Count);
                }
            }        
            if(categoriesAndItsModels.Count > 0)
            {
                areThereSavedModels = true;
            }
            else
            {
                areThereSavedModels = false;
            }
            foreach(KeyValuePair<string, List<string>> categoriesAndItsModelsKeyValuePair in categoriesAndItsModels)
            {
                Texture2D texture = new Texture2D(1,1);
                texture.LoadImage( encryptionHelper.Decrypt(File.ReadAllBytes(categoriesAndItsModelsKeyValuePair.Key + "/categoryImage.png")));
                string category = Path.GetFileName(categoriesAndItsModelsKeyValuePair.Key);
                string[] savedModelDirectories = categoriesAndItsModelsKeyValuePair.Value.ToArray();
                string[] modelNames = new string[savedModelDirectories.Length];
                for(int i=0;i<savedModelDirectories.Length;i++)
                {
                    modelNames[i] = Path.GetFileName(savedModelDirectories[i]);
                }
                LoadCategoryButtonSavedModels(category, modelNames, Sprite.Create(texture, new Rect(0,0,texture.width,texture.height), new Vector2(1,1)) );
            }     
        }
        else
        {
            UnityEngine.Debug.Log("The path doesn't exist");
            areThereSavedModels = false;
        }
        if(areThereSavedModels)
        {
            NoSavedModelsText.SetActive(false);
        }
        else
        {
            NoSavedModelsText.SetActive(true);
        }
        
    }   

    public void GenerateOnlineCategoriesList()  // It is called when the OnlineModelsButton is pressed
    {
        menu.OpenMenu(ModelPlacementCategoriesListOnline);
        isCategoriesListOnlineOpened = true;
        if(!wasCategoriesListOpened)
        {
            loadedCategoriesCountOnlineModels = 0;
            wasCategoriesListOpened = true;
            Directory.CreateDirectory(path);
            internetCheckerWhileDownloading.ToggleInternetChecking(true);
            internetCheckerWhileDownloading.ActionExecutedWhenInternetIsWorking = () => Debug.Log("Internet is working"); 
            internetCheckerWhileDownloading.ActionExecutedWhenInternetIsNotWorking = () => Debug.Log("Internet is not working"); 
            StartCoroutine(GetCategoryAmount());
        }
        
    }

    IEnumerator GetCategoryAmount() // first the amount of the categories that should be downloaded is get from the server and after that the downloading of the categories starts
    {
        yield return StartCoroutine(fileLoader.DownloadTextCoroutine(url + categoryCountFileNameUrl, path + "categoryCount.txt", true, (text) =>
        {
            if(text!=null)
            {
                InternetConnection();
                categoryNames = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                for(int i=0;i<categoryNames.Length;i++)
                {
                    categoryNames[i] = categoryNames[i].Trim(); // really important, causes error on android otherwise
                }
                categoryAmount = categoryNames.Length;
                StartCoroutine(DownloadCategory(1));
            }
            else
            {
                NoInternetConnection();
                StartCoroutine(GetCategoryAmount());
            }
        }));
        
    }

    IEnumerator DownloadCategory(int categoryIndex)
    {
        if(categoryIndex <= categoryAmount)
        {
            string currentCategory = categoryNames[categoryIndex-1];
            Directory.CreateDirectory(path + "/" + currentCategory);
            yield return StartCoroutine(fileLoader.DownloadImageCoroutine(url + currentCategory +  "/" + categoryImageFileNameUrl, path + "/" + categoryNames[categoryIndex-1] + "/categoryImage.png", true, (texture) => 
            {
                if(texture)
                {
                    StartCoroutine(fileLoader.DownloadTextCoroutine(url + currentCategory +  "/" + modelCountFileNameUrl, path + "/" + categoryNames[categoryIndex-1] + "/modelCount.txt", true, (text) =>
                    {
                        if(text != null)
                        {
                            string[] modelNamesArray = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                            for(int i=0;i<modelNamesArray.Length;i++)
                            {
                                modelNamesArray[i] = modelNamesArray[i].Trim(); // really important, causes error on android otherwise
                            }
                            LoadCategoryButtonOnlineModels(categoryNames[categoryIndex - 1], modelNamesArray, Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(1, 1)));
                            StartCoroutine(DownloadCategory(categoryIndex + 1));
                        }
                        else
                        {
                            StartCoroutine(DownloadCategory(categoryIndex));
                        }
                    })); 
                }
                else
                {
                    UnityEngine.Debug.Log("fileLoader.texture is null");
                    StartCoroutine(DownloadCategory(categoryIndex));
                }
            }));
            
        }
        else
        {
            internetCheckerWhileDownloading.ToggleInternetChecking(false);
            UnityEngine.Debug.Log("Finished downloading categories");
            modelDeleter.canTheModelListBeGenerated = true;
        }
    }

    // The LoadCategoryButtonOnlineModels and the LoadCategoryButtonSavedModels functions should not be melted into one single function, the program won't work! Please leave it like this. These are responsible to create the buttons that open a category

    void LoadCategoryButtonOnlineModels(string name, string[] modelNames, Sprite sprite)
    {
        GameObject CategoryButtonInstance = Instantiate(RoundedButton);
        if(loadedCategoriesCountOnlineModels%2==0)
        {
            GameObject RegionNthInstance = Instantiate(RegionN, GridContentModelPlacementCategoriesListOnline);
            PreviousRegionNthInstanceOnlineModels = RegionNthInstance;
            RegionNthInstance.transform.SetAsLastSibling();
            AlreadyLoadedRegionsOnlineCategories.Add(RegionNthInstance);
            CategoryButtonInstance.transform.SetParent(RegionNthInstance.transform);
            CategoryButtonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(-roundedButtonMargin,0);
        }
        else
        {
            CategoryButtonInstance.transform.SetParent(PreviousRegionNthInstanceOnlineModels.transform);
            CategoryButtonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(roundedButtonMargin,0);
        }
        CategoryButtonInstance.GetComponent<Button>().onClick.AddListener(() =>
        {
            modelPlacementListGenerator.StartModelListGeneration(name, modelNames, false);
        });
        CategoryButtonInstance.transform.GetChild(1).gameObject.SetActive(false);
        CategoryButtonInstance.transform.GetChild(0).GetComponent<Text>().text = name;
        CategoryButtonInstance.transform.GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        for(int i=CategoryButtonInstance.transform.childCount-2;i>2;i--)
        {
            CategoryButtonInstance.transform.GetChild(i).gameObject.SetActive(true);
            CategoryButtonInstance.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            CategoryButtonInstance.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = DarkPurple;
            DarkPurple.a = DarkPurple.a - 0.25f;
        }
        DarkPurple.a = 1;
        CategoryButtonInstance.transform.GetChild(CategoryButtonInstance.transform.childCount-1).GetComponent<Image>().preserveAspect = true;
        CategoryButtonInstance.GetComponent<Image>().color = LightBlue;
        CategoryButtonInstance.transform.localScale = new Vector3(1,1,1);
        modelPlacementListGenerator.CategoryButtonsOnline.Add(CategoryButtonInstance.GetComponent<Button>());
        loadedCategoriesCountOnlineModels++;
    }

    void LoadCategoryButtonSavedModels(string name, string[] modelNames, Sprite sprite)
    {
        GameObject CategoryButtonInstance = Instantiate(RoundedButton);
        if(loadedCategoriesCountSavedModels%2==0)
        {
            GameObject RegionNthInstance = Instantiate(RegionN, GridContentModelPlacementCategoriesListSaved);
            PreviousRegionNthInstanceSavedModels = RegionNthInstance;
            RegionNthInstance.transform.SetAsLastSibling();
            AlreadyLoadedRegionsSavedCategories.Add(RegionNthInstance);
            CategoryButtonInstance.transform.SetParent(RegionNthInstance.transform);
            CategoryButtonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(-roundedButtonMargin,0);
        }
        else
        {
            CategoryButtonInstance.transform.SetParent(PreviousRegionNthInstanceSavedModels.transform);
            CategoryButtonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(roundedButtonMargin,0);
        }
        CategoryButtonInstance.GetComponent<Button>().onClick.AddListener(() =>
        {
            modelPlacementListGenerator.StartModelListGeneration(name, modelNames, true);
        });
        CategoryButtonInstance.transform.GetChild(1).gameObject.SetActive(false);
        CategoryButtonInstance.transform.GetChild(0).GetComponent<Text>().text = name;
        CategoryButtonInstance.transform.GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        for(int i=CategoryButtonInstance.transform.childCount-2;i>2;i--)
        {
            CategoryButtonInstance.transform.GetChild(i).gameObject.SetActive(true);
            CategoryButtonInstance.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            CategoryButtonInstance.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = DarkPurple;
            DarkPurple.a = DarkPurple.a - 0.25f;
        }
        DarkPurple.a = 1;
        CategoryButtonInstance.transform.GetChild(CategoryButtonInstance.transform.childCount-1).GetComponent<Image>().preserveAspect = true;
        CategoryButtonInstance.GetComponent<Image>().color = LightBlue;
        CategoryButtonInstance.transform.localScale = new Vector3(1,1,1);
        modelPlacementListGenerator.CategoryButtonsSaved.Add(CategoryButtonInstance.GetComponent<Button>());
        loadedCategoriesCountSavedModels++;
        
    }

    public void BackFromCategoriesOnline() // It is called when the BackFromModelPlacementCategoriesListOnline is pressed
    {
        isCategoriesListOnlineOpened = false;
        menu.CloseMenuAndSetCurrentCanvas(ModelPlacementCategoriesListOnline, ModelPlacementMainMenu);
    }

    public void BackFromCategoriesSaved() // It is called when the BackFromModelPlacementCategoriesListSaved is pressed
    {
        isCategoriesLisSavedOpened = false;
        menu.CloseMenuAndSetCurrentCanvas(ModelPlacementCategoriesListSaved, ModelPlacementMainMenu);
        StartCoroutine(WaitForAnimationToFinishAndDestroySavedRegionsCoroutine());
    }

    string GetNthRowFromText(string text, int n)
    {
        return text.Split(new []{ Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[n];
    }

    void NoInternetConnection()
    {
        NoInternetConnectionContainerCategoriesListOnline.SetActive(true);
        Debug.Log("No internet connection");
    }

    void InternetConnection()
    {
        NoInternetConnectionContainerCategoriesListOnline.SetActive(false);
        Debug.Log("internet connection");
    }
}