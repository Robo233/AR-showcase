using UnityEngine;
using UnityEngine.UI;

public class LoadingRotation: MonoBehaviour {

    RectTransform CurrentRectTransform;

    [SerializeField] float rotateSpeed = 0.5f;
    [SerializeField] float angle = 45;
    float timePassed;

    void Start()
    {
        CurrentRectTransform = GetComponent<RectTransform>();
    }

    void FixedUpdate()
    {
        timePassed += Time.deltaTime;
        if(timePassed > rotateSpeed)
        {
            CurrentRectTransform.localEulerAngles = new Vector3(CurrentRectTransform.localEulerAngles.x, CurrentRectTransform.localEulerAngles.y, CurrentRectTransform.localEulerAngles.z - angle);
            timePassed = 0;
        }
        
    }

}