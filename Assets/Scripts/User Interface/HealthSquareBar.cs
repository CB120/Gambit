using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSquareBar : MonoBehaviour
{

    [SerializeField] GameObject healthSquarePrefab;

    // Most unit health values should be even so that everything lines up with full squares
    // In some contexts this doesn't matter as much, for example damage points shown on the unit info screen.
    // In these cases we can turn this bool on to not spit a warning when there is an odd health value.
    // This is not fully tested - you may need to flip the grid layout group and avoid using SetHealth.
    [SerializeField] bool experimental_expectOddMaxHealth = false; 

    List<HealthSquare> healthSquares = new List<HealthSquare>();

    public void Initialise(int maxHealth) {
        foreach (HealthSquare healthSquare in healthSquares) {
            Destroy(healthSquare.gameObject);
        }
        healthSquares.Clear();  

        // Creates a health square for every even health square.
        for (int i = 0; i < maxHealth / 2; i++) {
            CreateNewHealthSquare();
        }

        if (maxHealth % 2 != 0) {
            if (experimental_expectOddMaxHealth) {
                // create last health square
                CreateNewHealthSquare().SetAmount(1); // this is some gnarly shorthand lmao
            } else {
                // spit warning
                Debug.LogWarning("HealthSquareBar initialised with odd max health. Results might not be as expected (enable expectOddMaxHealth in the inspector if you want to create a half square anyway).", this);
            }
        }
    }
    // Redundancy to make it easy to integrate with existing code
    // Please directly use initialise for most implementations
    public void SetMaxHealth (int value) { Initialise(value); }
    
    public void SetHealth (int health) {
        
        for (int i = 0; i < healthSquares.Count; i++) {
            HealthSquare healthSquare = healthSquares[i];
            
            if (health >= 2) {
                healthSquare.SetAmount(2);
            } else {
                healthSquare.SetAmount(health);
            }

            health -= 2;
            if (health < 0) health = 0;
        }

    }

    public HealthSquare CreateNewHealthSquare() {
        GameObject newObject = Instantiate(healthSquarePrefab, transform);
        HealthSquare newSquare = newObject.GetComponent<HealthSquare>();
        newSquare.Setup();
        newSquare.SetAmount(2);
        healthSquares.Add(newSquare);
        return newSquare;
    }
}
