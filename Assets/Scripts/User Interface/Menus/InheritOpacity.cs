using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InheritOpacity : MonoBehaviour
{
    [SerializeField] private Image opacityToInheritFrom;
    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Image>())
        {
            float colorToInherit = opacityToInheritFrom.color.a;
            Color newColor = GetComponent<Image>().color;
            newColor.a = colorToInherit;
            GetComponent<Image>().color = newColor;
        }
    }
}
