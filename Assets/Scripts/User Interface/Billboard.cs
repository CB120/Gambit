using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    //Properties
    public bool isUI = true;


    //References
    NewCameraMovement cameraController;


    //Engine-called
    void Start(){
        cameraController = GameObject.FindWithTag("Camera Parent").GetComponent<NewCameraMovement>();
    }

    void LateUpdate(){
        UpdateRotation();
    }

    void UpdateRotation(){
        if (isUI){
            transform.LookAt(transform.position + GetRotationTarget());
        } else {
            transform.rotation = Quaternion.LookRotation(transform.position + Camera.main.gameObject.transform.position) * Quaternion.Euler(30, 0, 0);
        }
    }

    Vector3 GetRotationTarget(){
        switch(cameraController.rotationPositionNormalised){
            case 1: //left
                return new Vector3(-150, 122.5f, 150);
            case 2: //back
                return new Vector3(150, 122.5f, 150);
            case 3: //right
                return new Vector3(150, 122.5f, -150);
            default: //same as 0
                return new Vector3(-150, 122.5f, -150);
        }
    }
}
