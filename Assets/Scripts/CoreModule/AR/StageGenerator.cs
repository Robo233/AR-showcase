using System;
using UnityEngine;
using Vuforia;

/// <summary>
/// Creates either a GroundPlaneStage or a MidAirStage, depending on the model placement mode selected by the user
/// </summary>

public class StageGenerator : MonoBehaviour
{
    // the first 4 GameObjects should be assigned from Prefabs/ModelPlacement
    [SerializeField] GameObject GroundPlaneStage;
    [SerializeField] GameObject MidAirStage;
    [SerializeField] GameObject PlaneFinder;
    [SerializeField] GameObject MidAirPositioner;
    GameObject CurrentStage;
    GameObject CurrentPositioner;
    public GameObject CurrentPositionerInstance;

    [SerializeField] ModelScaler modelScaler;
    
    public void GenerateStage(Transform Parent, string modelPlacementMode, string contentPositioningMode, GameObject Model, Action ActionExecutedWhenModelIsPlaced)
    {
        if(modelPlacementMode == "GroundPlane")
        {
            CurrentStage = GroundPlaneStage;
            CurrentPositioner = PlaneFinder;
        }
        else
        {
            CurrentStage = MidAirStage;
            CurrentPositioner = MidAirPositioner;
        }
        CurrentPositionerInstance = Instantiate(CurrentPositioner, Parent);
        GameObject CurrentStageInstance = Instantiate(CurrentStage, Parent);
        Model.transform.SetParent(CurrentStageInstance.transform);
        modelScaler.AddModel(Model);
        
        CurrentPositionerInstance.GetComponent<ContentPositioningBehaviour>().AnchorStage = CurrentStageInstance.GetComponent<AnchorBehaviour>();
        if(contentPositioningMode == "Multiple")
        {
            CurrentPositionerInstance.GetComponent<ContentPositioningBehaviour>().DuplicateStage = true;
        }
        else
        {
            CurrentPositionerInstance.GetComponent<ContentPositioningBehaviour>().DuplicateStage = false;
        }
        
        if(contentPositioningMode == "Once")
        {
            CurrentPositionerInstance.GetComponent<ContentPositioningBehaviour>().OnContentPlaced.AddListener((GameObject Something) =>
            {
                Destroy(CurrentPositionerInstance);
            });
        }
        CurrentPositionerInstance.GetComponent<ContentPositioningBehaviour>().OnContentPlaced.AddListener((GameObject Something) =>
        {
            ActionExecutedWhenModelIsPlaced?.Invoke();
            CurrentStageInstance.transform.GetChild(1).GetComponent<Canvas>().enabled = true;
            modelScaler.isModelPlaced = true;
            if(modelScaler.isScalingActivated)
            {
                modelScaler.ActivateSizeCanvases();
            }
            
        });
            
    }
}