using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerPrefReaderDropdown : MonoBehaviour
{
    [SerializeField] string keyName;
    [SerializeField] int defaultValue;
    private void OnEnable() {
        GetComponent<TMP_Dropdown>().SetValueWithoutNotify(PlayerPrefs.GetInt(keyName, defaultValue));
    }
}
