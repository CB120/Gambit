using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MageAttackType {
    ChainClosest,
    Grenade
}

public class MageUnit : Unit
{
    //Properties
    [Tooltip("Set which mode the Mage's attack will follow.")]
    public MageAttackType attackMode;

    [Tooltip("Range of chained damage. Affects lightning jumping from Enemy to Enemy, not Mage to Enemy.")]
    public int chainRange = 3;
    [Tooltip("The maximum number of enemies damaged in one Attack. Set stupid-high to remove the limit")]
    public int maxDamagedEnemies = 5;
    [Tooltip("If True, each consecutive Unit in the attack chain will receive less damage, by factor of the Dropoff Rate")]
    public bool enableDamageDropoff = false;
    [Tooltip("To give finer control over the damage dropoff, please enter the damage each Unit receives. Element 0 = Unit the Player clicks on.")]
    public float[] damageInChain = {0};
    [SerializeField] private GameObject lightning;

    //Methods
        //Main method
    public override void AttackEnemy(Unit enemy){
        if (animator) animator.SetBool("AttackState", true);
        AudioManager.PlaySound(attackSound, AudioType.UnitSounds);

        switch (attackMode){
            case MageAttackType.Grenade:
                Grenade(enemy);
                break;
            default:
                ChainClosest(enemy);
                break;
        }
    }


        //Attack Mode Methods
    void ChainClosest(Unit enemy){
        List<Unit> enemyUnits = enemy.ownerParticipant.GetComponent<Participant>().GetUnits();

        Unit currentEnemy = enemy;
        Unit attackSource = this;
        for (int i = 0; i < maxDamagedEnemies && currentEnemy != null; i++){
            //Do damage to the current unit and remove it from the damage-able list
            Instantiate(lightning, currentEnemy.gameObject.transform);
            currentEnemy.DecreaseHealth(GetDamage(i));
            enemyUnits.Remove(currentEnemy);

            //Move 'where' the particle effect spawns between, to communicate the 'chaining'
            attackSource = currentEnemy;
            currentEnemy = null;

            //If there's more remaining enemies, do a min-search and set currentEnemy to the closest, so the loop doesn't exit
            if (enemyUnits.Count >= 1){
                Unit closestEnemy = enemyUnits[0];
                int closestDistance = enemyUnits[0].currentCell.FastMovesTo(attackSource);
                for (int u = 1; u < enemyUnits.Count; u++){
                    int distance = enemyUnits[u].currentCell.FastMovesTo(attackSource);
                    if (distance < closestDistance){
                        closestEnemy = enemyUnits[u];
                        closestDistance = distance;
                    }
                }

                //Make sure the closest enemy's actually within the chain range
                if (closestDistance <= chainRange){
                    currentEnemy = closestEnemy;
                }
            }
        }
    }

    void Grenade(Unit enemy){
        List<Unit> enemyUnits = enemy.ownerParticipant.GetComponent<Participant>().GetUnits();

        enemyUnits.Remove(enemy);
        enemy.DecreaseHealth(GetDamage(0));

        Instantiate(lightning, enemy.gameObject.transform);
        enemy.GetComponentInChildren<Animator>().SetTrigger("ShockState");

        int damagedEnemies = 1;
        for (int i = 0; i < enemyUnits.Count && damagedEnemies <= maxDamagedEnemies; i++){
            if (enemyUnits[i].currentCell.FastMovesTo(enemy) <= chainRange){
                enemyUnits[i].DecreaseHealth(GetDamage(i + 1));

                Instantiate(lightning, enemyUnits[i].gameObject.transform);
                enemyUnits[i].GetComponentInChildren<Animator>().SetTrigger("ShockState");

                damagedEnemies++;
            }
        }
    }


    //Functions
    float GetDamage(int counter){ //Protection to prevent out-of-range indexes being passed into damageInChain
        if (counter < 0){
            Debug.LogWarning("counter = " + counter + " passed in. Returning damageInChain[0]");
            return damageInChain[0];
        }

        if (enableDamageDropoff) {
            if (counter >= damageInChain.Length){
                return damageInChain[damageInChain.Length - 1];
            } else {
                return damageInChain[counter];
            }
        } else {
            return damageInChain[0];
        }
    }
}
