using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenLoader : MonoBehaviour
{
    [SerializeField] List<Sprite> sprites = new List<Sprite>();
    [SerializeField] float changeRate;
    float timer;

    Image image;

    private void Awake() {
        image = GetComponent<Image>();
    }

    private void FixedUpdate() {
        timer++;

        if (timer > changeRate) {
            timer = 0;
            Sprite oldSprite = image.sprite;
            Sprite newSprite = oldSprite;
            while (oldSprite == newSprite) {
                newSprite = sprites[Random.Range(0, sprites.Count)];
            }
            image.sprite = newSprite;
        }
    }
    
}
