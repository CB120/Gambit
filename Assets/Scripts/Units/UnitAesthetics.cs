using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitAesthetics : MonoBehaviour
{
    public GameObject Angel;
    public GameObject damageText;

    public void SpawnDamageText(float damageDecrement)
    {
        Vector3 position = gameObject.transform.position + new Vector3(-1, 2, 0);
        GameObject text = Instantiate(damageText, position, Quaternion.identity);
        text.GetComponent<Canvas>().worldCamera = Camera.main;
        text.GetComponentInChildren<TextMeshProUGUI>().text = "-" + damageDecrement.ToString();
    }
}
