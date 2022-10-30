using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    // DEPRECATED
    // IN NEARLY EVERY CASE YOU SHOULD BE USING HEALTHSQUAREBARS NOW
    // THIS JUST EXISTS FOR EASY INTEGRATION OF OLD STUFF
    // TODO: get rid of this script entirely and directly link stuff to healthsquarebar


    public bool isUnit = true;
    //[HideInInspector]
    public float maxHealth;
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill;
    [SerializeField] private TextMeshProUGUI dmgPoint;


    private void Start()
    {
        if (isUnit)
        {
            maxHealth = gameObject.GetComponent<Unit>().maxHealth;
            SetMaxHealth(maxHealth);
        }
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
        maxHealth = health;
    }

    public void SetHealth(float health)
    {
      
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
