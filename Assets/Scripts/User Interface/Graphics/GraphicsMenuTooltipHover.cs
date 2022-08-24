using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphicsMenuTooltipHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GraphicsMenuTooltipController controller;
    [SerializeField] string heading;
    [SerializeField] string content;
    [SerializeField] string p2;
    
    public void OnPointerEnter(PointerEventData eventData) {
        string text = "<b>" + heading + "</b> \n" + content + "\n" + p2;
        controller.SetText(text);
    }
    public void OnPointerExit(PointerEventData eventData) {
        controller.Unhovered();
    }
}
