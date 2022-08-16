using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuTextTips : MonoBehaviour
{
    public string[] textTips;
    public TextMeshProUGUI text;
    public Image textContainer;
    public Image selectIcon;
    public float timer = 5.0f;

    private void Start()
    {
        textContainer.enabled = false;
        text.enabled = false;
    }

    public void UpdateText(int textIndex)
    {
        GetComponent<RectTransform>().position = new Vector2(GetComponent<RectTransform>().position.x, selectIcon.GetComponent<RectTransform>().position.y);
        text.text = textTips[textIndex];
        Invoke("StartTextTimer", timer);
    }

    public void PointerExited()
    {
        textContainer.enabled = false;
        text.enabled = false;
    }

    private void StartTextTimer()
    {
        textContainer.enabled = true;
        text.enabled = true;
    }
}
