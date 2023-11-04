using UnityEngine;

/// <summary>
/// Makes possible the interaction with a 3D model, this script is responsible for moving, rotating and scaling the model
/// </summary>

public class ObjectMover : MonoBehaviour
{
    public Transform Model;
    public Transform SizeCanvas;

    Vector2 previousDistance;
    Vector2 initialTouchPosition;

    [SerializeField] Camera MainCamera;

    Vector3 initialScale;
    Vector3 initialPosition;
    Vector3 initialPositionSizeCanvas;
    Vector3 initialRotation;

    Quaternion TargetRotation;

    [SerializeField] float rotationSpeed;
    [SerializeField] float moveSpeed;
    [SerializeField] float scalingSensitivity;
    [SerializeField] float scaleThreshold;
    [SerializeField] float horizontalSwipeThreshold; // decrease this to favor the horizontal swipe instead of the vertical one
    float initialScaleFactor;
    float initialScaleX;
    float initialScaleY;
    float initialScaleZ;

    bool isScaling;

    string rotationAxis;

    [SerializeField] Menu menu;
    [SerializeField] ModelScaler modelScaler;

    public void SetModel(Transform Model, string rotationAxis)
    {
        this.Model = Model;
        this.rotationAxis = rotationAxis;
        initialScale = Model.localScale;
        initialPosition = Model.position;
        initialRotation = Model.localEulerAngles;
        TargetRotation = Model.rotation;
    }


    public void SetSizeCanvas(Transform SizeCanvas)
    {
        this.SizeCanvas = SizeCanvas;
        initialPositionSizeCanvas = SizeCanvas.position;
    }

    void Update()
    {
        if(Model && !menu.isAtLeastOneMenuOpen)
        {
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    previousDistance = touch2.position - touch1.position;
                    initialScaleX = Model.transform.localScale.x;
                    initialScaleY = Model.transform.localScale.y;
                    initialScaleZ = Model.transform.localScale.z;
                    initialScaleFactor = (touch1.position - touch2.position).magnitude;
                    initialTouchPosition = (touch1.position + touch2.position) * 0.5f;
                }
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    Vector2 currentDistance = touch2.position - touch1.position;
                    float rawScaleFactor = currentDistance.magnitude / initialScaleFactor;

                    float scaleFactor = Mathf.Pow(rawScaleFactor, scalingSensitivity);

                    if (Mathf.Abs(scaleFactor - 1.0f) > (scaleThreshold / initialScaleFactor))
                    {
                        Model.localScale = new Vector3(initialScaleX * scaleFactor, initialScaleY * scaleFactor, initialScaleZ * scaleFactor);
                        isScaling = true;
                    }
                    else
                    {
                        isScaling = false;
                    }

                    if(!isScaling)
                    {
                        Vector2 currentTouchPosition = (touch1.position + touch2.position) * 0.5f;
                        Vector2 swipeDirection = currentTouchPosition - initialTouchPosition;
                        Vector3 moveDirection = MainCamera.transform.TransformDirection(new Vector3(swipeDirection.x, swipeDirection.y, 0)) * moveSpeed * Time.deltaTime;
                        Model.position += moveDirection;
                        if(SizeCanvas)
                        {
                            SizeCanvas.position += moveDirection;
                        }
                        initialTouchPosition = currentTouchPosition;
                    }
                }
            }
            else if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    float rotationX = touch.deltaPosition.x * rotationSpeed * Time.deltaTime;
                    float rotationY = touch.deltaPosition.y * rotationSpeed * Time.deltaTime;
                    float horizontalChecker;
                    float currentRotation;
                    if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight)
                    {
                        currentRotation = rotationY;
                        horizontalChecker = rotationX;
                    }
                    else
                    {
                        currentRotation = rotationX;
                        horizontalChecker = rotationY;
                    }

                    if (horizontalChecker > -horizontalSwipeThreshold && horizontalChecker < horizontalSwipeThreshold)
                    {
                        if (rotationAxis == "y")
                        {
                            Model.Rotate(Vector3.up, -currentRotation, Space.Self);
                            if (SizeCanvas)
                            {
                                SizeCanvas.Rotate(Vector3.forward, currentRotation, Space.Self);
                            }
                        }
                        else
                        {
                            Model.Rotate(Vector3.forward, -currentRotation, Space.Self);
                            if (SizeCanvas)
                            {
                                SizeCanvas.Rotate(Vector3.up, currentRotation, Space.Self);
                            }   
                        }
                    }
                }
            }
        }
    }

    public void ResetModel() // It is called when the "Reset" button is pressed, from the LowerPanel
    {
        if(Model)
        {
            if(modelScaler.gameObject.activeSelf)
            {
                Model.localScale = modelScaler.realWorldScale;
            }
            else
            {
                Model.localScale = Vector3.one;
            }
            Model.localEulerAngles = initialRotation;
            Model.localPosition = Vector3.zero;
        }
        
        if(SizeCanvas)
        {
            modelScaler.DeactivateSizeCanvases();
            modelScaler.ResetSizeCanvases();
        }
        
    }

/* This method is commented out, it is not known where was it used
    public void HideModel()
    {
        if(Model)
        {
            ResetModel();
            Model.gameObject.SetActive(false);
            Model = null;
            Debug.Log("Model is hidden");
        }
        
    }
*/
}