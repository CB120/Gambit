using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Tooltip("Horizontal speed, in units/sec")]
    public float speed = 10;

    [Header("Objects")]
    [HideInInspector]
    public Vector3 startPos;
    [HideInInspector]
    public Vector3 targetPos;
    [HideInInspector]
    public bool hasReachedPosition = false;

    [Header("BombInFlight")]
    [HideInInspector]
    public Vector3 nextBasePos;


    public void Start()
    {
        startPos = transform.position;
        targetPos = Vector3.zero;
    }

    public void Update()
    {
        // Compute the next position, with arc added in
        if (!hasReachedPosition)
        {
            nextBasePos = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            transform.position = nextBasePos;
        }
    }
}
