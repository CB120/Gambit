using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    bool onClickPressed = false;
    [Header("Zoom")]
    [SerializeField] private float moveCameraSpeed;
    [SerializeField] private Vector2 zoomRange;
    [SerializeField] private float zoomIncrement;
    private Camera sceneCamera;
    [Header("Scroll")]
    [SerializeField] private float scrollSpeed;

    private float currentFOV;
    private float velocity = 1f;
   
    void Start()
    {
        sceneCamera = gameObject.GetComponentInChildren<Camera>();
        currentFOV = sceneCamera.fieldOfView;
    }

    void Update()
    {
        ZoomCamera();
        RotateMap();
    }

    void ZoomCamera()
    {
        //Gets the axis
        currentFOV -= Input.GetAxis("Mouse ScrollWheel") * zoomIncrement;
        //Smoothly changed the field of view, clamped to a min and max range
        sceneCamera.fieldOfView = Mathf.Clamp(Mathf.SmoothDamp(sceneCamera.fieldOfView, currentFOV, ref velocity, scrollSpeed * Time.deltaTime), zoomRange.x, zoomRange.y);
        if (currentFOV < zoomRange.x - 0.5f|| currentFOV > zoomRange.y + 0.5f)
        {
            //if the FOV is below a certain threshold, lock it to prevent the axis from overflowing
            currentFOV = sceneCamera.fieldOfView;
        }
    }

    void RotateMap()
    {
        if (onClickPressed)
        {
            //Only rotate if click is pressed
            gameObject.transform.Rotate((Vector3.up * moveCameraSpeed * Input.GetAxis("Camera Horizontal")));
        }

        if (Input.GetButtonUp("Camera Drag"))
        {
            onClickPressed = false;
        }
        else if (Input.GetButtonDown("Camera Drag"))
        {
            onClickPressed = true;
        }
    }
}
