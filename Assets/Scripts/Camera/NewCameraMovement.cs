using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraMovement : MonoBehaviour
{
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

    // 0 is default, 1, 2, 3 are 90 degree increments rotating clockwise across the map.
    int rotationPosition = 0; //used for the rotation calculation of rotationPivot (and can go negative)
    [HideInInspector] public int rotationPositionNormalised = 0; //used for the rotation of the health bars (limited between 0-3, wraps)


    float targetCameraSize;

    void Start() {
        panPosition = transform.position;
        targetCameraSize = Camera.main.fieldOfView;
    }

    void Update () {
        if (UIManager.isPaused) return; //GUARD to prevent camera movement when the game is paused

        if (Input.GetKeyDown(KeyCode.Q)) RotateSnapped(true);
        if (Input.GetKeyDown(KeyCode.E)) RotateSnapped(false);

        Vector2 keyboardPanInput = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        TranslateCameraKeyboard(keyboardPanInput * Time.deltaTime);
        AdjustZoom(-Input.GetAxis("Mouse ScrollWheel"));

        Vector3 targetPanPosition = panPosition;
        targetPanPosition.x = Mathf.Clamp(targetPanPosition.x, -maxPan, maxPan);
        targetPanPosition.z = Mathf.Clamp(targetPanPosition.z, -maxPan, maxPan);
        transform.position = Vector3.Lerp(transform.position, targetPanPosition, lerpSpeed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.Euler(Vector3.up * rotationPosition * 90);
        rotationPivot.rotation = Quaternion.Lerp(rotationPivot.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetCameraSize, lerpSpeed * Time.deltaTime);

        // Handle mouse input after sexy lerped stuff cause the mouse feels weird with that cringe shit
        if (Input.GetKey(KeyCode.Mouse0)) {
            Vector2 mouseInput = new Vector2(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) * mousePanSpeed;
            TranslateCameraMouse(mouseInput);
            transform.position = targetPanPosition;
        }
    }

    void TranslateCameraKeyboard(Vector2 movement) {
        if (ParticipantManager.IsCurrentParticipantType(ParticipantType.AI)) return; //GUARD to prevent the Player inputting camera panning during the AI's turn, created very hitchy camera movement

        movement *= panSpeed;
        Vector3 translation = new Vector3(movement.y + movement.x, 0, movement.y - movement.x);
        panPosition += rotationPivot.TransformVector(translation);
        panPosition.x = Mathf.Clamp(panPosition.x, -maxPan, maxPan);
        panPosition.z = Mathf.Clamp(panPosition.z, -maxPan, maxPan);
    }

    void TranslateCameraMouse(Vector2 movement) { //Adapted from TranslateCameraKeyboard but with extra calculations to account for mouse-style movement
        if (ParticipantManager.IsCurrentParticipantType(ParticipantType.AI)) return; //GUARD to prevent the Player inputting camera panning during the AI's turn, created very hitchy camera movement

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
    }

    public static void JumpToCell(Vector2Int coordinates){
        JumpToCell(GridController.GetCellAt(coordinates));
    }

    public static void JumpToCenter(){
        panPosition.x = 0;
        panPosition.z = 0;
    }
}
