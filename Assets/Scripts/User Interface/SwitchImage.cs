using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchImage : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private Image image;
    private int index = 0;

    void Awake(){
        if (image == null) image = GameObject.FindWithTag("RotateButton").GetComponent<Image>();
        if (image == null) Debug.LogWarning("No reference to the Rotate Button, is it missing its tag?");
    }

    public void ChangeSprite(bool clockwise){
        if (clockwise){
             index++;
        } else {
            index--;
        }

        if (index == sprites.Count){
            index = 0;
        } else if (index == -1){
            index = sprites.Count - 1;
        }

        if (image) image.sprite = sprites[index];
    }
}
