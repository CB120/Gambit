using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    //Properties
    [SerializeField] LayerMask mouseLayers;

    //Variables
    [HideInInspector] public bool cellSelectionEnabled = true; //true for Player Participant turns, false for AI Participant turns
    int uiLayer;


    //Engine-called methods
    void Start(){
        uiLayer = LayerMask.NameToLayer("UI");
    }

    void Update(){
        TestHit();
        CheckForMouseInput();
    }


    //Private methods
    void TestHit(){ //Raycasts from the Mouse into the World, calling CursorOnCell() on the Cell it hits, or GridController.CursorNotOnCell() if it doesn't hit anything
        if (!cellSelectionEnabled || EventSystem.current.IsPointerOverGameObject()){
            GridController.CursorNotOnCell();
        }

        if (Input.touchCount >= 1) //GUARD to prevent the raycast from firing when the Player's pinch-to-zoom-ing or tap-and-drag-to-pan-ing
        {
            GridController.CursorNotOnCell();
            return;
        }

        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(cameraRay.origin, cameraRay.direction, out hit, 1000, mouseLayers)) {
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.GetComponent<Cell>()) {
                hitObject.GetComponent<Cell>().CursorOnCell(); //this can be optimised at some point, store reference to Cell instead of the double-GetComponent()
            } else {
                GridController.CursorNotOnCell();
            }
        } else {
            GridController.CursorNotOnCell();
        }
    }

    void CheckForMouseInput(){
        if (Input.GetButtonDown("Mouse Select")) {
            if (!EventSystem.current.IsPointerOverGameObject()){
                GridController.OnMouseDown();
            }
        } else if (Input.GetButton("Mouse Select")){
            GridController.OnMouseHeld();
        } else if (Input.GetButtonUp("Mouse Select")){
            if (EventSystem.current.IsPointerOverGameObject()){
                GridController.clickHoveredCell = null;
            } else {
                GridController.OnMouseUp();
            }
        }
    }
}
