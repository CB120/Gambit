using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZoneScaler : MonoBehaviour
{
    // The rectTransform of the scaler - SHOULD ALWAYS BE USING STRETCH ANCHOR STYLE
    RectTransform rectTransform;
    // Last safe zone recorded.
    Rect safeZone;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        SetAnchors();
    }

    // Sets up the position anchors of the sprite
    void SetAnchors() {
        safeZone = Screen.safeArea;

        Vector2 minAnchor = new Vector2(safeZone.xMin / Screen.width, safeZone.yMin / Screen.height);
        Vector2 maxAnchor = new Vector2(safeZone.xMax / Screen.width, safeZone.yMax / Screen.height);

        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;
    }

    // TODO: should avoid having this running on *every* scaler's update
    // Could be moved to UI Manager that calls for every registered SafeZoneScaler to update its anchors when the safezone changes
    private void Update() {
        // When the safezone changes, update anchors
        if (Screen.safeArea != safeZone) {
            SetAnchors();
        }
    }
}
