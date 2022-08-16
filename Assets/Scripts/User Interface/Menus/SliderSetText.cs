using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderSetText : MonoBehaviour
{
    TextMeshProUGUI text;

    [SerializeField] int upperBound;
    [SerializeField] string upperText;

    [SerializeField] Slider slider;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        if (slider) {
            SetText(slider.value);
        }
    }

    public void SetText(float t) {
        if (text) {
            text.text = Mathf.RoundToInt(t).ToString();
            if (t >= upperBound) {
                text.text = upperText;
            }
        }
    }
}
