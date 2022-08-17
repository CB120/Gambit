using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefReaderToggle : MonoBehaviour
{
    [SerializeField] string keyName;
    [SerializeField] bool defaultValue;
    private void OnEnable() {
        GetComponent<Toggle>().SetIsOnWithoutNotify(PlayerPrefs.GetInt(keyName, defaultValue ? 1 : 0) == 1);
    }
}
