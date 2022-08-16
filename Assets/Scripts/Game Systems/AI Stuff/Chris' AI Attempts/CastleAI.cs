using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleAI : MonoBehaviour
{
    [Header("Castle Defence Range")]
    public Vector2Int defaultCastleCoordinates;
    public Cell castleCell;
    public int DefenceRadius = 10;
    public Cell[] DefenceRange;
    public bool inRange = false;

    [Header("Health Settings")]
    public float maxHealth = 400;
    public float health;

    [Header("Effects")]
    [SerializeField] private ParticleSystem smokePS;
    [SerializeField] private ParticleSystem firePS;

    void Start(){
        if (!castleCell) castleCell = GridController.GetCellAt(defaultCastleCoordinates);

        UIManager.SetObjectiveMaxHealth(maxHealth);
    }

    public void DrawDefenceRadius(){
        DefenceRange = castleCell.GetCellsInRange(DefenceRadius);
    }

    public bool PlayerInCastleRange(){
        DrawDefenceRadius();
        foreach (Cell c in DefenceRange)
        {
            if (c.attackScore > 50) // Each cell is given an attack score depending on the unit that is on it
            {                       // If the score is greater than 50, a unit is present (Default cellScore = 1, minimum unit cellScore = 100
                return inRange = true;
            } else
            {
                inRange = false;
            }
        }
        return inRange;
    }

    public void Attack(float Damage)
    {
        health = health - Damage;
        UIManager.DamageObjective(health);
        if(health <= maxHealth - 300)
        {
            firePS.Play();
        }
        else if(health <= maxHealth - 200)
        {
            firePS.Play();
        }
        else if (health <= maxHealth - 100){
            smokePS.Play();
        }

        if(health <= 0)
        {
            Destruction();
        }
    }

    public void Destruction()
    {
        smokePS.gameObject.SetActive(false);
        firePS.gameObject.SetActive(false);
        UIManager.enableGameResultState(false);
    }

    public bool CatapultInCastleRange(Unit Catapult)
    {
        bool CatInCastle = false;
        DrawDefenceRadius();
        foreach (Cell c in DefenceRange)
        {
            if (c.currentUnit == Catapult) 
            {
                CatInCastle = true;
                castleCell.attackScore = 1000;
            }
        }
        return CatInCastle;
    }

}
