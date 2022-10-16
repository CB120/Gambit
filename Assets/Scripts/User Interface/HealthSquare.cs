using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSquare : MonoBehaviour
{
    Image img;
    [SerializeField] Sprite unfilledSprite;
    [SerializeField] Sprite halfFilledSprite;
    [SerializeField] Sprite filledSprite;

    private void Awake() {
        img = GetComponent<Image>();
    }

    public void SetAmount (int i) {
        Sprite newSprite;
        switch (i) {
            case 0 : newSprite = unfilledSprite; break;
            case 1 : newSprite = halfFilledSprite; break;
            case 2 : newSprite = filledSprite; break;
            default: newSprite = unfilledSprite; break;
        }
        img.sprite = newSprite;
    }
}
