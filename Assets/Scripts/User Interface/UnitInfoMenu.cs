using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitInfoMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Unit_SO[] unitProperties;
    [SerializeField] private Button[] unitButtons;
    [SerializeField] private Button selectedButton;
    [SerializeField] private TextMeshProUGUI description;
    // [SerializeField] private TextMeshProUGUI health; // Deprecated, use healthSquares instead.
    [SerializeField] private HealthSquareBar healthSquares;
    // [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private HealthSquareBar damageSquares;
    [SerializeField] private TextMeshProUGUI attackRange;
    [SerializeField] private TextMeshProUGUI movementRange;
    [SerializeField] private Image icon;

    [Header("Color Properties")]
    [SerializeField] private Color selected;
    [SerializeField] private Color disabled;
    private void Awake()
    {
        SetProperty(0);
    }

    public void SetOnEnum(Unit.UnitType type)
    {
        for(int i = 0; i < unitProperties.Length; i++)
        {
            if (unitProperties[i].unitType == type)
            {
                SetProperty(i);
                return;
            }
        }
    }

    public void SetProperty(int i)
    {
        gameObject.SetActive(true);
        selectedButton.image.color = disabled;
        selectedButton = unitButtons[i];
        selectedButton.image.color = selected;

        description.text = unitProperties[i].unitDescription;
        // health.text = unitProperties[i].health.ToString();
        healthSquares.Initialise(unitProperties[i].health);
        // damage.text = unitProperties[i].damage.ToString();
        damageSquares.Initialise((int)unitProperties[i].damage);
        attackRange.text = unitProperties[i].range.ToString();
        movementRange.text = unitProperties[i].movementRange.ToString();
        icon.sprite = unitProperties[i].icon;
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }
}
