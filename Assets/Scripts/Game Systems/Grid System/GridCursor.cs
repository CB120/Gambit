using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    [SerializeField] GameObject spriteObject;
    [SerializeField] Image uiImage;

    [SerializeField] Sprite moveSprite;
    [SerializeField] Sprite attackSprite;
    
    public void MoveCursor(Cell cell) {
        transform.position = cell.transform.position;
        SetVisible(cell);
        uiImage.sprite = cell.currentUnit ? attackSprite : moveSprite;
    }

    public void SetVisible(bool visible) {
        spriteObject.SetActive(visible);
    }
}
