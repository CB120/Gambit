using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outpost : MonoBehaviour
{
    //Variables
    bool dead = false;
    bool hovered = false;


    //References
        //In-prefab references
    [Header("In-prefab references")]
    public GameObject healthBar;
    public GameObject normalModel;
    public GameObject flag;
    public GameObject destroyedModel;
    public GameObject destroyedFlag;
    [SerializeField] private GameObject PS;
    public AudioClip outpostDeathSound;

    public List<GameObject> outlineModels = new List<GameObject>(); //Sorry for not doing it recursively, there was a bug and I was too tired to solve it

    HealthBar healthBarScript;


    //Methods
        //Engine-called
    void Awake(){
        healthBarScript = GetComponent<HealthBar>();
    }


        //Public methods
            //Events
    public void OnDeath(){
        dead = true;

        normalModel.SetActive(false);
        flag.SetActive(false);
        destroyedModel.SetActive(true);
        PS.SetActive(true);

        AudioManager.PlaySound(outpostDeathSound, AudioType.UnitSounds);

        if (hovered) SetOutlineActive(true);
    }

    public void OnHover(){
        healthBar.SetActive(true);
        SetOutlineActive(true);
        hovered = true;
    }

    public void OffHover(){
        healthBar.SetActive(false);
        SetOutlineActive(false);
        hovered = false;
    }

            //Misc.
    public void ShowHealthBar(int health){
        ActivateAnimation();
        healthBarScript.SetHealth(health);
        healthBar.SetActive(true);
        Invoke("HideHealthBar", 5f);
    }

    public void ActivateAnimation(){
        GetComponent<Animator>().SetTrigger("Clicked");
    }

    public void SetMaxHealth(int maxHealth){
        healthBarScript.SetMaxHealth(maxHealth);
    }


        //Private methods
    void HideHealthBar(){
        if (!hovered) healthBar.SetActive(false);
    }

    void SetOutlineActive(bool on){
        if (on){
            if (dead) {
                SetLayer(10);
            } else {
                SetLayer(11);
            }
        } else {
            SetLayer(0);
        }
    }

    void SetLayer(int layer){
        foreach (GameObject g in outlineModels){
            if (!g) continue; //GUARD to prevent NREs

            g.layer = layer;
            //if (g.tag == "IconUI") g.layer = isEnter ? 12 : 0; //tf is isEnter???
            if (g.tag == "UI") g.layer = 0;
        }
    }
}
