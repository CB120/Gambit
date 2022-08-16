using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float timeToDestroy = 5f;

    private void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
    }
}
