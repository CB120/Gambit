using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectRotator : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 5.0f;
    [SerializeField] private bool rotateX;
    [SerializeField] private bool rotateY;
    [SerializeField] private bool rotateZ;
    [SerializeField] private Transform pivot;

    // Update is called once per frame
    void Update()
    {
        float xRotation = 0;
        float yRotation = 0;
        float zRotation = 0;

        if (rotateX)
            xRotation = 1;
        if (rotateY)
            yRotation = 1;
        if (rotateZ)
            zRotation = 1;

        gameObject.transform.RotateAround(pivot.position, new Vector3(xRotation, yRotation, zRotation), rotateSpeed * Time.deltaTime);
    }
}
