using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows the size of the model in centimeters
/// </summary>

public class ModelScaler : MonoBehaviour
{
    [SerializeField] GameObject SizeCanvas;
    GameObject InstantiatedSizeCanvas;

    public List<GameObject> InstantiatedSizeCanvases = new List<GameObject>();
    [SerializeField] List<GameObject> Models = new List<GameObject>();
    [SerializeField] List<GameObject> WidthTexts = new List<GameObject>();
    [SerializeField] List<GameObject> HeightTexts = new List<GameObject>();
    [SerializeField] List<GameObject> LengthTexts = new List<GameObject>();

    Transform OriginalStage;

    [SerializeField] Button ScaleButton;

    ColorBlock ColorBlockScaleButton;

    [SerializeField] Color normalColorButton;
    [SerializeField] Color selectedColorButton;

    public Vector3 realWorldScale;
    [SerializeField] Vector3 initialSizeCanvasEulerAngles;
    [SerializeField] Vector3 initialSizeCanvasPosition;

    [SerializeField] float scaleOffset; // It is used to convert the scale of the model from Unity units to real world units, it's not clear however why we need this offset, why the object doesn't have the same size inthe real world as in unity
    [SerializeField] float textOffset;

    public bool isScalingActivated;
    public bool isModelPlaced;

    [SerializeField] ObjectMover objectMover;
    [SerializeField] ModelPlacementGenerator modelPlacementGenerator;
    [SerializeField] LowerPanelInAppHandler lowerPanelInAppHandler;

    void Start()
    {
        initialSizeCanvasEulerAngles = SizeCanvas.transform.eulerAngles;
        initialSizeCanvasPosition = SizeCanvas.transform.localPosition;
    }

    public void ClearLists()
    {
        Models.Clear();
        InstantiatedSizeCanvases.Clear();
        WidthTexts.Clear();
        HeightTexts.Clear();
        LengthTexts.Clear();
    }

    public void AddModel(GameObject Model)
    {
        Vector3 originalScale = Model.transform.localScale;
        realWorldScale = new Vector3(originalScale.x * scaleOffset, originalScale.y * scaleOffset, originalScale.z * scaleOffset);
        Model.transform.localScale = realWorldScale;
        ColorBlockScaleButton = ScaleButton.colors;
        OriginalStage = Model.transform.parent;
        InstantiatedSizeCanvas = Instantiate(SizeCanvas, OriginalStage);
        InstantiatedSizeCanvas.SetActive(false);
        objectMover.SetSizeCanvas(InstantiatedSizeCanvas.transform);
        SetTextPositions();
    }


    public void DestroyInstantiatedSizeCanvases()
    {
        foreach(GameObject InstantiatedSizeCanvas in InstantiatedSizeCanvases)
        {
            Destroy(InstantiatedSizeCanvas);
        }
    }

    void SetTextPositions()
    {
        for(int i=0;i<Models.Count;i++)
        {
            Vector3 realWorldScale = Models[i].transform.localScale;
            WidthTexts[i].transform.localPosition = new Vector3(0, realWorldScale.x + textOffset, realWorldScale.z);
            HeightTexts[i].transform.localPosition = new Vector3(-realWorldScale.z, realWorldScale.x + textOffset, -realWorldScale.z);
            LengthTexts[i].transform.localPosition = new Vector3(realWorldScale.z + textOffset, 0, realWorldScale.z);
        }
        
    }

    void Update()
    {
        if(isScalingActivated && Models.Count>0)
        {
            for(int i=0; i<Models.Count; i++)
            {
                WidthTexts[i].GetComponent<Text>().text = Math.Round(Models[i].transform.localScale.z/scaleOffset,2)*100 + " cm";
                HeightTexts[i].GetComponent<Text>().text = Math.Round(Models[i].transform.localScale.y/scaleOffset,2)*100 + " cm";
                LengthTexts[i].GetComponent<Text>().text = Math.Round(Models[i].transform.localScale.x/scaleOffset,2)*100 + " cm";
            }
            SetTextPositions();
        }
        
    }

    public void ToggleSizeCanvas() // It is called, when the ScaleButton is pressed
    {
        if(!isScalingActivated)
        {
            ActivateSizeCanvases();
        }
        else
        {
            DeactivateSizeCanvases();
        }
        
    }

    public void ActivateSizeCanvases()
    {
        isScalingActivated = true;
        lowerPanelInAppHandler.ToggleButtonColors(ScaleButton, false);
        if(isModelPlaced)
        {
            List<GameObject> AllStagesList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Stage"));
            foreach(GameObject Stage in AllStagesList)
            {
                if(Stage == OriginalStage.gameObject && modelPlacementGenerator.contentPositioningMode==2 )
                {
                    AllStagesList.Remove(Stage);
                    break;
                }
            }
            foreach(GameObject Stage in AllStagesList)
            {   
                if(!Models.Contains(Stage.transform.GetChild(0).gameObject))
                {
                    Models.Add(Stage.transform.GetChild(0).gameObject);
                }
                if(!InstantiatedSizeCanvases.Contains(Stage.transform.GetChild(1).gameObject))
                {
                    InstantiatedSizeCanvases.Add(Stage.transform.GetChild(1).gameObject);
                }
            }
            foreach(GameObject InstantiatedSizeCanvas in InstantiatedSizeCanvases)
            {   
                InstantiatedSizeCanvas.SetActive(true);
                if(!WidthTexts.Contains(InstantiatedSizeCanvas.transform.GetChild(0).gameObject))
                {
                    WidthTexts.Add(InstantiatedSizeCanvas.transform.GetChild(0).gameObject);
                    HeightTexts.Add(InstantiatedSizeCanvas.transform.GetChild(1).gameObject);
                    LengthTexts.Add(InstantiatedSizeCanvas.transform.GetChild(2).gameObject);
                }
            
            }
            SetTextPositions();
        }
        
    }

    public void DeactivateSizeCanvases()
    {
        isScalingActivated = false;
        foreach(GameObject InstantiatedSizeCanvas in InstantiatedSizeCanvases)
        {
            InstantiatedSizeCanvas.SetActive(false);
        }
        lowerPanelInAppHandler.ToggleButtonColors(ScaleButton, true);
    }

    public void ResetSizeCanvases()
    {
        foreach(GameObject InstantiatedSizeCanvas in InstantiatedSizeCanvases)
        {
            InstantiatedSizeCanvas.transform.localEulerAngles = initialSizeCanvasEulerAngles;
            InstantiatedSizeCanvas.transform.localPosition = initialSizeCanvasPosition;
        }
    }
}