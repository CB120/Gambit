using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnClick : MonoBehaviour
{
    // Start is called before the first frame update


    public void DestroyParent()
    {
        Destroy(this.gameObject);
    }
}
