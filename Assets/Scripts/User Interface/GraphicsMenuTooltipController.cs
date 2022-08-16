using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphicsMenuTooltipController : MonoBehaviour
{
    TMP_Text text;
    private void Awake() {
        text = GetComponent<TMP_Text>();
    }
    public void SetText(string s) {
        text.text = s;
    }

    public void Unhovered() {
        text.text = "";
    }
}
