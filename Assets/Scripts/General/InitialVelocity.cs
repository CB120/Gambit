using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialVelocity : MonoBehaviour
{
    public Vector3 force;
    void Start()
    {
        this.GetComponent<Rigidbody>().AddForce(force);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
