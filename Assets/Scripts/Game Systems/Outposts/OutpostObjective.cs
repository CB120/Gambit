using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OutpostObjective : MonoBehaviour
{
    //Properties
    public bool outpostCaptureIsObjective = false;
    public int outpostHealth = 30;


    //Variables
    [HideInInspector] public static List<OutpostGroup> outpostGroups;

    static OutpostGroup currentHoveredGroup = null;


    //References
        //Asset references
    public GameObject damageTextPrefab;

        //Scene references
    static OutpostObjective Singleton;
    AIOutpostController aiOutpostController;
    [SerializeField] private bool isProcedurallyGenerated;


    //Methods
        //Engine-called
    void Awake(){
        Singleton = this;

        SetOutpostGroups();
    }

    void Start(){
        if (!isProcedurallyGenerated) OnStart();
    }

    public void OnStart(){
        aiOutpostController = GetComponent<AIOutpostController>();

        SetOutpostHealth();
        SetSpawnCoordinates();
    }


        //Public methods
    public static void AttackOutpost(Vector2Int coordinates, int damage){
        bool foundOutpost = false;
        foreach (OutpostGroup g in outpostGroups){
            foreach (Vector2Int c in g.outpostCoordinates){
                if (c == coordinates){
                    foundOutpost = true;

                    g.health -= damage;
                    Singleton.SpawnOutpostDamageText(g, damage);

                    if (g.health <= 0){
                        g.outpostObject.GetComponent<Outpost>().ActivateAnimation();
                        Singleton.OnOutpostDeath(g);
                    }
                    g.outpostObject.GetComponent<Outpost>().ShowHealthBar(g.health);
                }
            }
        }

        if (!foundOutpost) Debug.LogWarning("No Outpost at (" + coordinates.x + ", " + coordinates.y + ")!");
    }

    public void CheckOutpostHover(Cell cell){ //For EVERY hovered Cell, checks if its an Outpost. If so, Outline it, if not, un-Outline the previous one
        OutpostGroup currentGroup = GetOutpostGroupOnCell(cell);

        if (currentGroup != null) {
            currentGroup.outpostObject.GetComponent<Outpost>().OnHover();
            currentHoveredGroup = currentGroup;
        } else {
            if (currentHoveredGroup != null) currentHoveredGroup.outpostObject.GetComponent<Outpost>().OffHover();
            currentHoveredGroup = null;
        }
    }


        //Private methods
            //Events
    void OnOutpostDeath(OutpostGroup outpost){
        foreach (Vector2Int c in outpost.spawnCoordinates){
            aiOutpostController.spawnCoordinates.Remove(c);
        }
        outpost.captured = true;
        if (outpostCaptureIsObjective) CheckForOutpostObjective();

        UIManager.CaptureOutpost();
        Debug.Log("An Outpost has died!");

        outpost.outpostObject.GetComponent<Outpost>().OnDeath();
    }

            //Static methods
    static void SetOutpostGroups(){
        GridGenerator gridGenerator = GameObject.FindWithTag("Grid Controller").GetComponent<GridGenerator>();
        if (gridGenerator) {
            outpostGroups = gridGenerator.outpostGroups;
        } else {
            Debug.LogWarning("OutpostObjective couldn't find GridGenerator!");
        }
    }

            //Misc.
    void CheckForOutpostObjective(){
        foreach (OutpostGroup g in outpostGroups){
            if (g.captured == false) return; //if any of them aren't captured, exit the method
        }

        UIManager.EndGame(false); //You win! Yay!
    }

    void SpawnOutpostDamageText(OutpostGroup outpost, float damageDecrement){
        Vector3 position = outpost.outpostObject.transform.position + new Vector3(-1, 3, 0);
        GameObject text = Instantiate(damageTextPrefab, position, Quaternion.identity);
        text.GetComponent<Canvas>().worldCamera = Camera.main;
        text.GetComponentInChildren<TextMeshProUGUI>().text = "-" + damageDecrement.ToString();
    }

    void SetOutpostHealth(){
        foreach (OutpostGroup g in outpostGroups){
            g.health = outpostHealth;
            HealthBar outpostHealthBar = g.outpostObject.GetComponent<HealthBar>();
            outpostHealthBar.maxHealth = outpostHealth;
            outpostHealthBar.SetMaxHealth(outpostHealth);
        }
    }

    void SetSpawnCoordinates(){
        foreach (OutpostGroup g in outpostGroups){
            foreach (Vector2Int c in g.spawnCoordinates){
                aiOutpostController.spawnCoordinates.Add(c);
            }
        }
    }


    //Functions
        //Public getters
    public static OutpostGroup GetOutpostGroupOnCell(Cell cell){
        if (!cell) return null;
        return GetOutpostGroupOnCell(cell.coordinates);
    }

    public static OutpostGroup GetOutpostGroupOnCell(Vector2Int coordinates){
        foreach (OutpostGroup g in outpostGroups){
            foreach (Vector2Int c in g.outpostCoordinates){
                if (c == coordinates) return g;
            }
        }
        return null;
    }

    public static bool IsOutpostDead(Cell cell){
        OutpostGroup outpost = GetOutpostGroupOnCell(cell);
        if (outpost == null){
            Debug.LogWarning("No outpost at (" + cell.coordinates.x + ", " + cell.coordinates.y + ")");
            return true;
        } else {
            return outpost.captured;
        }
    }
}
