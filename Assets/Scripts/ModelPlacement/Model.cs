using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class makes it easier to work with models, by creating a model object, which contains all the information about a model
/// </summary>

public class Model
{
    public Sprite Sprite;

    public string name;
    public string description;
    public string category;
    public string path;

    public bool isInSavedModels;
    public bool isSaved;

    public Model(string name, string description, string category, string path, Sprite Sprite, bool isInSavedModels, bool isSaved)
    {
        this.name = name;
        this.description = description;
        this.category = category;
        this.path = path;
        this.Sprite = Sprite;
        this.isInSavedModels = isInSavedModels;
        this.isSaved = isSaved;
    }

}