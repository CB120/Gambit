using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefReaderSlider : MonoBehaviour
{
    [SerializeField] string keyName;
    [SerializeField] int defaultValue;

    private void OnEnable() {
        GetComponent<Slider>().SetValueWithoutNotify(PlayerPrefs.GetInt(keyName, defaultValue));
        SliderSetText setText = GetComponentInChildren<SliderSetText>();
        if (setText) {
            setText.SetText(GetComponent<Slider>().value);
        }
    }
}
