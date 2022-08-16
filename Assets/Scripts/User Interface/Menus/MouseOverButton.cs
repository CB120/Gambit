using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MouseOverButton : MonoBehaviour, IPointerEnterHandler// required interface when using the OnPointerEnter method.
{
    [SerializeField] private GameObject selectIcon;
    [SerializeField] private Sprite levelIcon;
    [SerializeField] private Sprite levelIconHighlighted;

    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnPointerEnter(PointerEventData eventData)
    {

        //BODGED SOLUTION - WILL UPDATE LATER ~  David
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
        {
            //Move the select icon to a given option if the button is interactable 
            if (GetComponent<Button>().interactable)
            {
                selectIcon.GetComponent<RectTransform>().position = new Vector2(selectIcon.GetComponent<RectTransform>().position.x, gameObject.GetComponent<RectTransform>().position.y);
            }
        }
        else
        {
            if (GetComponent<Button>().interactable)
            {
                selectIcon.GetComponent<RectTransform>().position = new Vector2(gameObject.GetComponent<RectTransform>().position.x, gameObject.GetComponent<RectTransform>().position.y + 15.0f);
            }
        }
        
    }

    public void ChangeIconToNormal()
    {
        GetComponent<Image>().sprite = levelIcon;
    }

    public void ChangeIconToHighlighted()
    {
        GetComponent<Image>().sprite = levelIconHighlighted;
    }
}