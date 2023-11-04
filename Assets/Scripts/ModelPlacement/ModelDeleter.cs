using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the deleting of the model
/// </summary>

public class ModelDeleter : MonoBehaviour
{
    [SerializeField] Transform ScrollableListModelInfo;

    [SerializeField] RectTransform DirectoryDeleteHandlerMenuModelPlacement;
    [SerializeField] RectTransform ModelInfo;
    [SerializeField] RectTransform ModelPlacementModelsListSaved;
    [SerializeField] RectTransform ModelPlacementModelsListOnline;
    [SerializeField] RectTransform ModelPlacementCategoriesListSaved;
    [SerializeField] RectTransform ModelPlacementCategoriesListOnline;
    [SerializeField] RectTransform AboutMenuInApp;
    RectTransform CanvasThatShouldBeOpenedAfterClosingDirectoryDeleteHandlerMenu;

    [SerializeField] Button DirectoryDeleteHandlerMenuOpenerModelPlacement;

    string currentPathModelPlacement;
    string[] modelNames;

    [SerializeField] ModelPlacementGenerator modelPlacementGenerator;
    [SerializeField] ModelPlacementListGenerator modelPlacementListGenerator;
    [SerializeField] CategoryListGenerator categoryListGenerator;
    [SerializeField] Menu menu;

    public Model model;

    public bool canTheModelListBeGenerated;
    public bool isModelInfoOpenedFromModelsList;
    bool shouldTheModelListBeGenerated;

    public void BackFromDirectoryDeleteHandlerMenuModelPlacement() // It is called when the DirectoryDeleteHandlerMenuModelPlacementBackButton is pressed
    {
        menu.CloseMenuAndSetCurrentCanvas(DirectoryDeleteHandlerMenuModelPlacement, ModelInfo);
    }

    public void BackFromModelInfoMenu() // It is called when the BackFromModelInfo is pressed
    {
        if(isModelInfoOpenedFromModelsList)
        {
            menu.CloseMenu(ModelInfo);
        }
        else
        {
            if(CanvasThatShouldBeOpenedAfterClosingDirectoryDeleteHandlerMenu == null || CanvasThatShouldBeOpenedAfterClosingDirectoryDeleteHandlerMenu == ModelPlacementModelsListSaved || CanvasThatShouldBeOpenedAfterClosingDirectoryDeleteHandlerMenu == ModelPlacementModelsListOnline)
            {
                if(model.isInSavedModels)
                {
                    CanvasThatShouldBeOpenedAfterClosingDirectoryDeleteHandlerMenu = ModelPlacementModelsListSaved;
                }
                else
                {
                    CanvasThatShouldBeOpenedAfterClosingDirectoryDeleteHandlerMenu = ModelPlacementModelsListOnline;
                }
            }
            menu.CloseMenuAndSetCurrentCanvas(ModelInfo, CanvasThatShouldBeOpenedAfterClosingDirectoryDeleteHandlerMenu);
        } 
    }

    public void DeleteDataModelPlacement() // it is called from the DeleteContentButtonModelPlacement. It deletes the saved model from the device and also resets the list of the models, becuase the deleted model is no longer there(in case of saved models) or is no longer saved (in case of online models)
    {
        Debug.Log("Deleting " + model.name + " from " + model.path);
        Directory.Delete(model.path + "/" + model.name, true);
        modelPlacementListGenerator.savedModelDirectoriesList.Remove(model.path + "/" + model.name);
        string categoryPath = Application.persistentDataPath + "/Model placement/" + model.category;
        List<string> otherDirectoriesInTheCategoryWhichContainAModel = GetDirectoriesThatContainFile(categoryPath);
        if(otherDirectoriesInTheCategoryWhichContainAModel.Count == 0)
        {
            modelNames = File.ReadAllText(categoryPath + "/modelCount.txt").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Directory.Delete(categoryPath, true);
            if(model.isInSavedModels)
            {
                CanvasThatShouldBeOpenedAfterClosingDirectoryDeleteHandlerMenu = ModelPlacementCategoriesListSaved;
                categoryListGenerator.wasCategoriesListOpened = false;
                categoryListGenerator.DestroySavedRegions();
                categoryListGenerator.CheckIfThereAreSavedModelsAndGenerateSavedCategoriesList();
                modelPlacementListGenerator.BackFromModelPlacementListSaved();
                modelPlacementListGenerator.DestroyOnlineRegions();   
                categoryListGenerator.DestroyOnlineRegions();
            }
            else
            {
                CanvasThatShouldBeOpenedAfterClosingDirectoryDeleteHandlerMenu = ModelPlacementCategoriesListOnline;
                categoryListGenerator.wasCategoriesListOpened = false;
                categoryListGenerator.DestroyOnlineRegions();
                canTheModelListBeGenerated = false;
                categoryListGenerator.GenerateOnlineCategoriesList();
                modelPlacementListGenerator.DestroySavedRegions();
                modelPlacementListGenerator.DestroyOnlineRegions();
                shouldTheModelListBeGenerated = true;

            }
        }
        else
        {
            if(model.isInSavedModels)
            {
                modelPlacementListGenerator.DestroySavedRegions();
                modelPlacementListGenerator.StartModelListGeneration(model.category, GetNames(otherDirectoriesInTheCategoryWhichContainAModel), true );
                CanvasThatShouldBeOpenedAfterClosingDirectoryDeleteHandlerMenu = ModelPlacementModelsListSaved;
            }
            else
            {
                CanvasThatShouldBeOpenedAfterClosingDirectoryDeleteHandlerMenu = ModelPlacementModelsListOnline;
            }
        }
        modelPlacementListGenerator.savedModelDirectoriesList.Remove(model.path);
        DirectoryDeleteHandlerMenuOpenerModelPlacement.interactable = false;
        BackFromDirectoryDeleteHandlerMenuModelPlacement();
        model.isSaved = false;
    }

    void Update()
    {
        if(canTheModelListBeGenerated && shouldTheModelListBeGenerated)
        {
            canTheModelListBeGenerated = false;
            shouldTheModelListBeGenerated = false;
            modelPlacementListGenerator.StartModelListGeneration(model.category,  modelNames, false );
            
        }
    }

    string[] GetNames(List<string> paths)
    {
        string[] names = new string[paths.Count];
        for(int i = 0; i < names.Length; i++)
        {
            names[i] = Path.GetFileNameWithoutExtension(paths[i]);
        }
        return names;
    }

    public List<string> GetDirectoriesThatContainFile(string path)
    {
        string[] directories = Directory.GetDirectories(path);
        List<string> directoriesThatContainFile = new List<string>();
        foreach(string directory in directories)
        {
            if(File.Exists(directory + "/model.glb"))
            {
                directoriesThatContainFile.Add(directory);
            }
        }
        return directoriesThatContainFile;
    }
    
}