using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapAlpha : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image[] images;
    void Start()
    {
        foreach (Image img in images)
        {
            img.alphaHitTestMinimumThreshold = 1f;
        }
    }

}
