using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraMovement : MonoBehaviour
{
    static NewCameraMovement Instance;
    static Vector3 panPosition;

    [SerializeField] Transform rotationPivot;

    [SerializeField] float maxPan;
    [SerializeField] float maxZoom;
    [SerializeField] float minZoom;

    [SerializeField] float panSpeed = 3.0f;
    [SerializeField] float rotationSpeed = 0.05f;
    [SerializeField] float mousePanSpeed = 10.0f;
    [SerializeField] float mouseVerticalPanMultiplier = 20f;
    [SerializeField] float zoomSpeed = 10.0f;

    [SerializeField] float lerpSpeed = 0.5f;

    [SerializeField] float touchscreenPinchMultiplier = 0.5f;
    [SerializeField] float dragYMinimumThreshold = 60f;

    // 0 is default, 1, 2, 3 are 90 degree increments rotating clockwise across the map.
    int rotationPosition = 0; //used for the rotation calculation of rotationPivot (and can go negative)
    [HideInInspector] public int rotationPositionNormalised = 0; //used for the rotation of the health bars (limited between 0-3, wraps)

    [SerializeField] float cellHeightOffset = 1;

    float targetCameraSize;

    float oldPinchDistance = -1f; //-1 is a sentinal value
    bool pinchInProgress = false;

    private void Awake() {
        Instance = this;
    }

    void Start() {
        panPosition = transform.position;
        targetCameraSize = Camera.main.fieldOfView;
    } 

    void Update () {
        if (UIManager.isPaused) return; //GUARD to prevent camera movement when the game is paused

        UpdateKeyboardRotation();

        //GUARD to prevent the Player inputting camera panning during the AI's turn, created very hitchy camera movement
        if (!ParticipantManager.IsCurrentParticipantType(ParticipantType.AI)) TranslateCameraKeyboard();

        //UpdatePinchZoom() shouldn't call anything if the device doesn't have a touchscreen, but just in case
        if (SystemInfo.deviceType == DeviceType.Handheld) UpdatePinchZoom();
        
        UpdateScrollZoom();

        Vector3 targetPanPosition = UpdateCameraPosition();

        UpdateCameraRotation();

        UpdateCameraFOV();

        // Handle mouse input after sexy lerped stuff cause the mouse feels weird with that cringe shit
        if (Input.GetKey(KeyCode.Mouse0) && Input.touchCount < 2 && !pinchInProgress) UpdatePositionDrag(targetPanPosition);
    }

    void UpdateKeyboardRotation()
    {
        if (Input.GetKeyDown(KeyCode.Q)) RotateSnapped(true);
        if (Input.GetKeyDown(KeyCode.E)) RotateSnapped(false);
    }

    void UpdatePinchZoom()
    {
        if (Input.touchCount >= 2) //adapted from http://shorturl.at/deSV3
        {
            Vector2 touch0, touch1;
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            float distance = Vector2.Distance(touch0, touch1);

            if (oldPinchDistance >= 0f)
            {
                float delta = oldPinchDistance - distance;
                AdjustZoom(delta * touchscreenPinchMultiplier);
            }

            oldPinchDistance = distance;
            pinchInProgress = true;
        }
        else
        {
            oldPinchDistance = -1f;
        }

        //Had issues where if the Player released one finger but not the other, you'd get a weird camera snap-drag. This is to avoid that.
        if (Input.touchCount == 0) pinchInProgress = false;
    }

    void UpdateScrollZoom()
    {
        AdjustZoom(-Input.GetAxis("Mouse ScrollWheel"));
    }

    Vector3 UpdateCameraPosition()
    {
        Vector3 targetPanPosition = panPosition;
        targetPanPosition.x = Mathf.Clamp(targetPanPosition.x, -maxPan, maxPan);
        targetPanPosition.z = Mathf.Clamp(targetPanPosition.z, -maxPan, maxPan);
        transform.position = Vector3.Lerp(transform.position, targetPanPosition, lerpSpeed * Time.deltaTime);
        return targetPanPosition;
    }

    void UpdateCameraRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(Vector3.up * rotationPosition * 90);
        rotationPivot.rotation = Quaternion.Lerp(rotationPivot.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void UpdateCameraFOV()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetCameraSize, lerpSpeed * Time.deltaTime);
    }

    void UpdatePositionDrag(Vector3 targetPanPosition)
    {
        Vector2 mouseInput = new Vector2(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) * mousePanSpeed;
        if (Input.touchCount == 0)
        {
            if (!ParticipantManager.IsCurrentParticipantType(ParticipantType.AI)) TranslateCameraMouse(mouseInput);
        }
        else
        {
            Vector3 mousePosition = Input.mousePosition;
            if (mousePosition.y >= dragYMinimumThreshold && !ParticipantManager.IsCurrentParticipantType(ParticipantType.AI)) TranslateCameraMouse(mouseInput);
        }
        transform.position = targetPanPosition;
    }

    void TranslateCameraKeyboard() {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movement *= Time.deltaTime;
        movement *= panSpeed;
        Vector3 translation = new Vector3(movement.y + movement.x, 0, movement.y - movement.x);
        panPosition += rotationPivot.TransformVector(translation);
        panPosition.x = Mathf.Clamp(panPosition.x, -maxPan, maxPan);
        panPosition.z = Mathf.Clamp(panPosition.z, -maxPan, maxPan);
    }

    void TranslateCameraMouse(Vector2 movement) { //Adapted from TranslateCameraKeyboard but with extra calculations to account for mouse-style movement
        movement /= Screen.height;
        movement *= Camera.main.fieldOfView;
        movement *= 100; //to allow bigger numbers for mousePanSpeed
        movement.y *= mouseVerticalPanMultiplier;
        Vector3 translation = new Vector3(movement.y + movement.x, 0, movement.y - movement.x);
        panPosition += rotationPivot.TransformVector(translation);
        panPosition.x = Mathf.Clamp(panPosition.x, -maxPan, maxPan);
        panPosition.z = Mathf.Clamp(panPosition.z, -maxPan, maxPan);
    }

    public void RotateSnapped (bool clockwise) {
        //Changed code back as you can just do a check here to ensure the component is null, and when you click q and e the sprite wouldn't update
        SwitchImage switchImage = gameObject.GetComponent<SwitchImage>();
        if (switchImage) switchImage.ChangeSprite(clockwise);
        rotationPosition += clockwise ? 1 : -1;
        UpdateRotationPositionNormalised(clockwise);
    }

    void UpdateRotationPositionNormalised(bool clockwise){
        if (clockwise){
            if (rotationPositionNormalised < 3){
                rotationPositionNormalised++;
            } else {
                rotationPositionNormalised = 0;
            }
        } else {
            if (rotationPositionNormalised <= 0){
                rotationPositionNormalised = 3;
            } else {
                rotationPositionNormalised--;
            }
        }
    }

    void AdjustZoom (float delta) {
        targetCameraSize = Mathf.Clamp(targetCameraSize + delta * zoomSpeed, maxZoom, minZoom);
    }

    public static void JumpToCell (Cell c) {
        panPosition.x = c.transform.position.x;
        panPosition.z = c.transform.position.z;

        // Slight bodge here to make it so the elevation of cells is compensated for
        // Without this the camera always moves to focus on where the cell would be at 0 height, which can be off-screen especially at mobile-style zoom levels
        float offsetAmount = c.height * Instance.cellHeightOffset;
        panPosition.x += offsetAmount;
        panPosition.z += offsetAmount;
    }

    public static void JumpToCell(Vector2Int coordinates){
        JumpToCell(GridController.GetCellAt(coordinates));
    }

    public static void JumpToCenter(){
        panPosition.x = 0;
        panPosition.z = 0;
    }
}
