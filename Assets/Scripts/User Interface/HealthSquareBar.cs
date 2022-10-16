using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSquareBar : MonoBehaviour
{

    [SerializeField] GameObject healthSquarePrefab;

    List<HealthSquare> healthSquares = new List<HealthSquare>();

    public void Initialise(int maxHealth) {
        foreach (HealthSquare healthSquare in healthSquares) {
            Destroy(healthSquare.gameObject);
        }
        healthSquares.Clear();  

        for (int i = 0; i < maxHealth / 2; i++) {
            GameObject newObject = Instantiate(healthSquarePrefab, transform);
            HealthSquare newSquare = newObject.GetComponent<HealthSquare>();
            newSquare.SetAmount(2);
            healthSquares.Add(newSquare);
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

        // int i = 0;
        // while (health > 2) {
        //     if (healthSquares.Count < i) {
        //         healthSquares[i].SetAmount(2);
        //     } else {
        //         Debug.LogWarning("Health bar has less squares than required. You probably need to call the Initialise() function somewhere");
        //     }
        //     health -= 2;
        //     i++;
        // }

        // if (healthSquares.Count < i) {
        //     healthSquares[i].SetAmount(health);
        // } else {
        //     Debug.LogWarning("Health bar missing last square!");
        // }
    }
}
