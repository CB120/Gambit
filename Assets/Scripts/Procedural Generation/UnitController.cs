using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour //I am coding this while drunk, I promise I'm not a shit programmer
{
    [Header("Assignment Variables")]
    public List<ChooseUnit> selectedUnits;
    [HideInInspector]
    public List<ChooseUnit> selectedAIUnits = new List<ChooseUnit>();
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private GridGenerator gridGenerator;
    
    [SerializeField] private ParticipantManager participantManager;
    [SerializeField] private PlayerParticipant playerParticipant;
    [SerializeField] public AIParticipant aiParticipant;
    [SerializeField] private GenerateProcedurally proceduralComponent;
    [SerializeField] private GameObject catapult;
    

    [Header("Skirmish Mode")]
    [SerializeField] private bool chooseUnits;

    [Header("Endless Mode")]
    [SerializeField] private EndlessMode endlessMode;
    [Header("Runtime")]
    private List<GameObject> spawnedPlayerUnits = new List<GameObject>();
    private List<Cell> spawnedAIPositions = new List<Cell>();
    [HideInInspector]
    public GameObject castle;
    public List<Vector2> AIOutpostSpawningPositions = new List<Vector2>();
    public Vector2 minMaxUnits = new Vector2(3, 6);
    public MapSize mapSize;

    public void GenerateUnits()
    {
        if (!proceduralComponent.generateRandomly) chooseUnits = true;
        if (spawnedPlayerUnits.Count > 0)//Remove all Units (Used for runtime testing) 
        {
            DestroyObjectsInList(spawnedPlayerUnits);
            playerParticipant.units.Clear();
            aiParticipant.units.Clear();
            spawnedAIPositions.Clear();

        }

        List<Cell> playerCells = GetPlayerCells();//The list of cells the player units should spawn on
        List<Cell> enemyCells = GetAICells();
        proceduralComponent.gridController.defaultPlayerCell = GetLastCellInColumn(0, 0);
        List<Cell> outPostCells = new List<Cell>();
        foreach (Cell cell in gridGenerator.cells)
            foreach (Vector2 pos in AIOutpostSpawningPositions)
                if (cell.coordinates == pos) outPostCells.Add(cell);

        SpawnUnits(playerCells, enemyCells, outPostCells);

        //Debug.Log("=======================");  // - - - - - WHEN DECLAN FIXES THE GETPATH TO, ENSURES ALL AI ARE REACHABLE - - - - -
        //bool isReachable = true;
        List<Cell> playerCell = GetPlayerCells();
        foreach (Cell cell in spawnedAIPositions)
        {
            //foreach (Cell playerCell in GetPlayerCells())
            if (playerCell[0].GetPathTo(cell).Length == 0)
            {
                proceduralComponent.GenerateRandomMap();
                return;
            }
        }

        if(proceduralComponent.gameMode.type == GameMode.Catapult)//IF its catapult mode ensure the castle is reachable
        {
            if (playerCell[playerCell.Count - 1].GetPathTo(GetFirstCellInColumn(mapGenerator._mapWidth - 4, 0)).Length == 0)
            {
                proceduralComponent.GenerateRandomMap();
                return;
            }
        }

        if (proceduralComponent.generateRandomly)
         endlessMode.PopulateSettings();

        UIManager.SetToolBarImage(playerParticipant.units);
        PlayerParticipant.selectedUnit = 0;
        aiParticipant.GetComponent<AIController>().Castle = castle;
        
        if(proceduralComponent.gameMode.type == GameMode.Outposts)
        {
            castle.GetComponent<CastleAI>().castleCell = gridGenerator.cells[(int)(gridGenerator.cells.Count / 2)];
        }
        else
        {
            castle.GetComponent<CastleAI>().castleCell = GetFirstCellInColumn(mapGenerator._mapWidth - 4, 0);
        }
        aiParticipant.GetComponent<AIController>().LateStart();
        participantManager.StartNextTurn();
    }

    private List<Cell> GetPlayerCells()
    {
        List<Cell> playerCells = new List<Cell>();
        int count = selectedUnits.Count;
        if (proceduralComponent.gameMode.type == GameMode.Catapult) count++;
        for (int i = 0; i < count; i++)
        {
            for (int x = 0; x < 4; x++)
            {
                playerCells.Add(GetLastCellInColumn(x, i));//Get the last cell in a column, max y value is 4
            }
        }
        return playerCells;
    }

    private List<Cell> GetAICells()
    {
        List<Cell> AICells = new List<Cell>();

        for (int i = 0; i < (proceduralComponent.generateRandomly ? selectedUnits.Count : selectedAIUnits.Count); i++)
        {
            for (int x = mapGenerator._mapWidth - 1; x > mapGenerator._mapWidth - 6; x--)
            {
                AICells.Add(GetFirstCellInColumn(x, i));//Get the last cell in a column, max y value is 4
            }
        }
        return AICells;
    }
    private Cell GetLastCellInColumn(int yVal, int offset)
    {
        Cell currentCell = gridGenerator.cells[0];
        for(int i = 0; i < gridGenerator.cells.Count; i++)
        {
            if (gridGenerator.cells[i].coordinates.y > yVal) return currentCell; //Guard - Returns the last cell in that column
            if (gridGenerator.cells[i].coordinates.y == yVal && i - offset > 0)//Ensure cells are in the same column
            {
                currentCell = gridGenerator.cells[i - offset];
            }
        }
        return currentCell;
    }

    private Cell GetFirstCellInColumn(int yVal, int offset)
    {
        Cell currentCell = gridGenerator.cells[gridGenerator.cells.Count - 1];
        for (int i = 0; i < gridGenerator.cells.Count; i++)
        {
            if (gridGenerator.cells[i].coordinates.y > yVal) return currentCell; //Guard - Returns the last cell in that column
            if (gridGenerator.cells[i].coordinates.y == yVal)//Ensure cells are in the same column
            {
                if(i + offset < gridGenerator.cells.Count)
                    return gridGenerator.cells[i + offset];
            }
        }
        return currentCell;
    }

    [HideInInspector]
    public List<ChooseUnit> endlessUnits = new List<ChooseUnit>();

    private void SpawnUnits(List<Cell> playerCells, List<Cell> enemyCells, List<Cell> outpostCells)
    {
        endlessUnits.Clear();
        bool gameModeIsCatapult = proceduralComponent.gameMode.type == GameMode.Catapult;
        int min = (int)minMaxUnits.x;
        int max = (int)minMaxUnits.y;
        switch (proceduralComponent.mapSize)
        {
            case MapSize.Small:
                max /= 2;
                break;
            case MapSize.Medium:
                break;
            case MapSize.Large:
                min *= (int)1.5f;
                break;
        }

        int unitsToSpawn = (int)Random.Range(min, max);//A random amount of units to spawn
        if (chooseUnits) unitsToSpawn = selectedUnits.Count; //If the units to spawn have been selected, spawn the specific amount
        int unitsToSpawnAtOutPost = (int)(unitsToSpawn / 2);
        if (gameModeIsCatapult) unitsToSpawn++;
        for(int i = 0; i < unitsToSpawn; i++)
        {
            //- - - - - - Spawn Player Unit = - - - - -//
            int index = i;
            if (!chooseUnits)
                index = (int)Random.Range(0, selectedUnits.Count);

            if (i == unitsToSpawn - 1 && gameModeIsCatapult)
            {
                GameObject catapultObj = Instantiate(catapult, playerCells[i].gameObject.transform.position, Quaternion.identity);
                catapultObj.GetComponent<CatapultUnit>().startCell = playerCells[i];//Set their start cells
                catapultObj.GetComponent<CatapultUnit>().ownerParticipant = playerParticipant.gameObject;
                aiParticipant.GetComponent<AIController>().Catapult = catapultObj;
                spawnedPlayerUnits.Add(catapultObj.gameObject);
            }
            else
            {
                GameObject unit = Instantiate(selectedUnits[index].playerUnit.gameObject, playerCells[i].gameObject.transform.position, Quaternion.identity);//Instantiate the unit on the correct cell
                unit.GetComponent<Unit>().startCell = playerCells[i];//Set their start cells
                unit.GetComponent<Unit>().ownerParticipant = playerParticipant.gameObject;
                playerParticipant.units.Add(unit.GetComponent<Unit>());
                spawnedPlayerUnits.Add(unit.gameObject);//Add to the existing list
                endlessUnits.Add(selectedUnits[index]);
            }

            //- - - - - - Spawn Enemy Unit = - - - - -//
            if (!chooseUnits)
            {
                int randomEnemyCellIndex = (int)Random.Range(0, outpostCells.Count); //Get a random Outpost Cell
                Cell enemyCellToSpawnAt = enemyCells[i]; //Set the cell to spawn at 
                if (i >= unitsToSpawnAtOutPost && outpostCells.Count != 0) //If the adequate number of enemies have not been spawned at an outpost cell, spawn them at a random Outpost cell
                    enemyCellToSpawnAt = outpostCells[randomEnemyCellIndex];//Spawn at least half the units at an outpost, the rest at a castle

                GameObject AIUnit = Instantiate(selectedUnits[index].enemyUnit.gameObject, enemyCellToSpawnAt.gameObject.transform.position, Quaternion.identity);//Instantiate the unit on the correct cell
                AIUnit.GetComponent<Unit>().startCell = enemyCellToSpawnAt;//Set their start cells
                AIUnit.GetComponent<Unit>().currentCell = enemyCellToSpawnAt;//Set their start cells
                AIUnit.GetComponent<Unit>().ownerParticipant = aiParticipant.gameObject;//Set the owner participant
                                                                                        //aiParticipant.units.Add(AIUnit.GetComponent<Unit>());//Add the AI Units to their participant
                spawnedAIPositions.Add(enemyCellToSpawnAt);
                spawnedPlayerUnits.Add(AIUnit.gameObject);//Add to the existing list
                if (i >= unitsToSpawnAtOutPost && outpostCells.Count != 0)
                    outpostCells.RemoveAt(randomEnemyCellIndex);//Remove items in the specified list
            }
        }
        if(chooseUnits)
            SpawnAIUnits(enemyCells, outpostCells, unitsToSpawnAtOutPost);
    }

    private void SpawnAIUnits(List<Cell> enemyCells, List<Cell> outpostCells, int outPostUnits)
    {
        for (int i = 0; i < selectedAIUnits.Count; i++)
        {
            int randomEnemyCellIndex = (int)Random.Range(0, outpostCells.Count); //Get a random Outpost Cell
            Cell enemyCellToSpawnAt = enemyCells[i]; //Set the cell to spawn at 
            if (i >= outPostUnits && outpostCells.Count != 0) //If the adequate number of enemies have not been spawned at an outpost cell, spawn them at a random Outpost cell
                enemyCellToSpawnAt = outpostCells[randomEnemyCellIndex];//Spawn at least half the units at an outpost, the rest at a castle

            GameObject AIUnit = Instantiate(selectedAIUnits[i].enemyUnit.gameObject, enemyCellToSpawnAt.gameObject.transform.position, Quaternion.identity);//Instantiate the unit on the correct cell
            AIUnit.GetComponent<Unit>().startCell = enemyCellToSpawnAt;//Set their start cells
            AIUnit.GetComponent<Unit>().currentCell = enemyCellToSpawnAt;//Set their start cells
            AIUnit.GetComponent<Unit>().ownerParticipant = aiParticipant.gameObject;//Set the owner participant
                                                                                    //aiParticipant.units.Add(AIUnit.GetComponent<Unit>());//Add the AI Units to their participant
            spawnedAIPositions.Add(enemyCellToSpawnAt);
            spawnedPlayerUnits.Add(AIUnit.gameObject);//Add to the existing list
            if (i >= outPostUnits && outpostCells.Count != 0)
                outpostCells.RemoveAt(randomEnemyCellIndex);//Remove items in the specified list
        }
    }

    private void DestroyObjectsInList(List<GameObject> list)
    {
        foreach (GameObject obj in list)
        {
            DestroyImmediate(obj.gameObject);
        }
        list.Clear();
    }

}


[System.Serializable]
public struct ChooseUnit
{
    public Unit.UnitType unitType;
    public Unit playerUnit;
    public Unit enemyUnit;
    public GameObject button;
}

