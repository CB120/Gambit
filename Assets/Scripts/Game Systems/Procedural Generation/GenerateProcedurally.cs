using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Saving;

public class GenerateProcedurally : MonoBehaviour
{
    [Header("Objects")]
    #region References
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] public GridController gridController;
    [SerializeField] private UnitController unitController;
    [SerializeField] private OutpostObjective outpostObjective;
    [SerializeField] private GameObject castle;
    [SerializeField] private GameObject outpost;
    [SerializeField] private GameObject forest;
    private List<GameObject> objectsInScene = new List<GameObject>();
    #endregion
    [Header("Customisation")]
    #region Customisable Properties
    [SerializeField] int minimumMapSize = 15;//If we let the user choose map size these just need to be set to the same number
    [SerializeField] int maximumMapSize = 50;
    public bool generateRandomly = true; //If a random biome should be chosen for the user
    public BiomeType setBiome;
    [HideInInspector]
    public Biome currentBiome;           //Should be set if the user wants to choose their biome
    public Biomes biomes;//List of biomes
    public int smallMapSize = 25;
    public int mediumMapSize = 30;
    public int largeMapSize = 40;
    public MapSize mapSize;
    public SelectGameMode selectGameMode;
    public ChooseGameMode gameMode;
    public bool isDayTime;
    public TimeOfDay timeOfDay;
    #endregion
    [Header("Settings")]
    public MapSettings defaultSettings;

    [Header("Performance")]
    [SerializeField] int safeGuard = 50;
    private float[,] heightMap;

    private AIOutpostController outpostController;

    private void Start()
    {
        outpostController = outpostObjective.gameObject.GetComponent<AIOutpostController>();
        if (generateRandomly)
        {
            SavedData.GameData.dynamicDifficultyEnabled = true;
            GenerateRandomMap();
        }
    }

    #region Map Generation

    public void GenerateRandomMap()
    {
        outpostController.turnCounter = 0;
        outpostController.spawnCoordinates.Clear();
        outpostController.unitsSpawned = 0;
        outpostController.spawnMode = SpawnMode.Disabled;

        UIManager.RemoveDeadUnits();
        OutpostObjective.outpostGroups.Clear();
        currentBiome.biome = setBiome;
        gameObject.GetComponent<UnitController>().AIOutpostSpawningPositions.Clear();
        int contingency = 0;//Contingency in case the while loop runs too long
        CreateNewMap();


        bool isNotValidMap = false;//Bool to ensure the castle spawn position is valid
        int consistentHeight = gridGenerator.GetHeight(heightMap.GetLength(0) - 1, heightMap.GetLength(1) - 1);//The height of the last voxel position (to right corner of the map)
        while (!isNotValidMap)//Regenerate the map until a valid one is found
        {
            if (gameMode.type != GameMode.Outposts) AlterBlackList();//Add the 4x4 cells in the top right hand corner of the map to the blacklist (this is where the castle will spawn)
            for (int y = heightMap.GetLength(0) - 4; y < heightMap.GetLength(1); y++)//For the 4x4 voxel positions in the top right of the map
            {
                for (int x = 0; x < 4; x++)
                {
                    if (IsInvalidPosition(new Vector2Int(x, y), consistentHeight, false))//If the 4x4 positions in the top right of the map contain a water cell and each position is not the same height, the map is invalid
                    {
                        isNotValidMap = true;
                    }
                }
            }

            if (isNotValidMap)//If the map is not valid, regenerate a new map and try again 
            {
                contingency++;
                if (contingency > safeGuard)
                {
                    DefaultToKnownMap();
                }
                else
                {
                    CreateNewMap();
                    isNotValidMap = false;
                }
            }
            else//If the map is valid, procedurally generate objects
            {
                mapGenerator.GenerateMap();
                isNotValidMap = true;
            }
        }

        //Once a valid map is found, generate the Cells
        SpawnObjects();
        gridGenerator.RemoveChildren();
        gridGenerator.GenerateLevel();

        switch (mapSize)
        {
            case MapSize.Small:
                if (currentBiome.spawnForests)
                    foreach (GameObject tree in gridGenerator.GenerateForests(currentBiome.tree, GetTreeDensity(mapSize)))
                        objectsInScene.Add(tree);
                break;
            case MapSize.Medium:
                if (currentBiome.spawnForests)
                    foreach (GameObject tree in gridGenerator.GenerateForests(currentBiome.tree, GetTreeDensity(mapSize)))
                        objectsInScene.Add(tree);
                break;
            case MapSize.Large:
                if (currentBiome.spawnForests)
                    foreach (GameObject tree in gridGenerator.GenerateForests(currentBiome.tree, GetTreeDensity(mapSize)))
                        objectsInScene.Add(tree);
                break;
        }
        unitController.aiParticipant.GetComponent<AIController>().ForestLevel = currentBiome.spawnForests;
        EnableGameMode();
        SetTimeOfDay();
        gridController.OnAwake();

        if (gameMode.type == GameMode.Outposts)
            UIManager.SetOutPosts(OutpostObjective.outpostGroups.Count);

        outpostObjective.OnStart();

        SpawnFinalObjects();
    }

    private void SetTimeOfDay()//Wrote this drunk i know its shit but I cannot think straight;
    {
        if (!generateRandomly)
        {
            if (isDayTime)
            {
                timeOfDay.nightTime.SetActive(false);
                timeOfDay.dayTime.SetActive(true);
                return;
            }
            timeOfDay.nightTime.SetActive(true);
            timeOfDay.dayTime.SetActive(false);
            return;
        }

        float randRange = Random.Range(0f, 1f);
        if (randRange > 0.75f)
        {
            timeOfDay.nightTime.SetActive(true);
            timeOfDay.dayTime.SetActive(false);
            return;
        }
        timeOfDay.nightTime.SetActive(false);
        timeOfDay.dayTime.SetActive(true);
    }
    private int GetTreeDensity(MapSize mapSize)
    {
        switch (mapSize)
        {
            case MapSize.Small:
                return Random.Range(0, 3);
            case MapSize.Medium:
                return Random.Range(3, 10);
            case MapSize.Large:
                return Random.Range(6, 20);
            default: return 10;
        }
    }

    private void EnableGameMode()
    {
        outpostObjective.outpostCaptureIsObjective = false;
        if (gameMode.type == GameMode.Outposts)
            outpostObjective.outpostCaptureIsObjective = true;

        gameMode.UIToEnable.SetActive(true);
        foreach (GameObject obj in gameMode.UIToDisable)
        {
            obj.SetActive(false);
        }
        UIManager.UIObj.objectiveTitle.text = gameMode.mainTitle;
    }

    private ChooseGameMode GetGameMode(MapSize mapSize)
    {
        float randRange = Random.Range(0f, 1f);
        switch (mapSize)
        {
            case MapSize.Small:
                return selectGameMode.killAllEnemies;
            case MapSize.Medium:
                if (randRange > 0.5) return selectGameMode.catapult;
                return selectGameMode.killAllEnemies;
            case MapSize.Large:
                if (randRange > 0.66) return selectGameMode.catapult;
                else if (randRange > 0.33) return selectGameMode.killAllEnemies;
                return selectGameMode.captureOutPosts; //Change this to spawn outposts
        }
        return selectGameMode.killAllEnemies;
    }

    private void CreateNewMap()
    {
        SetBiome();
        mapGenerator._seed = (int)Random.Range(0, 100000);
        int mapSize = GetRandomSize();
        if (generateRandomly)
            gameMode = GetGameMode(this.mapSize);
        UIManager.UIObj.currentGameMode = this.gameMode.type;
        GameManager.gameMode = this.gameMode.type;
        mapGenerator._mapWidth = mapSize;
        mapGenerator._mapHeight = mapSize;
        gridGenerator.width = mapSize;
        gridGenerator.height = mapSize;
        heightMap = Noise.GenerateNoiseMap(mapSize, mapSize, mapGenerator._seed, defaultSettings.noiseScale, defaultSettings.octaves,
            defaultSettings.persistance, defaultSettings.lacunarity, defaultSettings.offset);
        MapGenerator.globalHeightMap = heightMap;
    }

    private void SetBiome()
    {
        if (generateRandomly)
            currentBiome.biome = (BiomeType)Random.Range(0, System.Enum.GetValues(typeof(BiomeType)).Length);

        gridGenerator.waterHeight = 0.2f;
        switch (currentBiome.biome)
        {
            case BiomeType.Forest:
                currentBiome = biomes.Forest;
                break;
            case BiomeType.Desert:
                gridGenerator.waterHeight = -1;
                currentBiome = biomes.Desert;
                break;
            case BiomeType.Snow:
                currentBiome = biomes.Snow;
                break;
            case BiomeType.Cherry:
                currentBiome = biomes.Cherry;
                break;
            case BiomeType.Volcano:
                currentBiome = biomes.Volcano;
                break;
            case BiomeType.Swamp:
                currentBiome = biomes.Swamp;
                break;
            case BiomeType.Mesa:
                currentBiome = biomes.Mesa;
                break;
            case BiomeType.Waterworld:
                currentBiome = biomes.Waterworld;
                break;
        }
        mapGenerator._regions = currentBiome.regions;

    }

    private void AlterBlackList()
    {
        //Clears the coordinate blacklist of the GridGenerator and then re-adds coordinates too it. 
        //This is performed as the grid can change size each time the map is generated
        gridGenerator.coordinateBlacklist.Clear();
        for (int y = heightMap.GetLength(0) - 4; y < heightMap.GetLength(0); y++)
        {
            for (int x = 0; x < 4; x++)
            {
                gridGenerator.coordinateBlacklist.Add(new Vector2Int(x, y));
            }
        }
    }

    private int GetRandomSize()//Return a random size as an odd number, castle positions fuck up if it's even
    {
        if (!generateRandomly)
        {
            switch (mapSize)
            {
                case MapSize.Small: return smallMapSize - 2;
                case MapSize.Medium: return mediumMapSize - 1;
                case MapSize.Large: return largeMapSize - 1;
            }
        }
        int number = (int)Random.Range(minimumMapSize, maximumMapSize);
        if (number % 2 == 1)
        {
            if (number < smallMapSize) mapSize = MapSize.Small;
            else if (number < mediumMapSize) mapSize = MapSize.Medium;
            else mapSize = MapSize.Large;
            return number;
        }
        else return GetRandomSize();
    }

    private bool IsInvalidPosition(Vector2Int position, int consistentHeight, bool isOutpost)//Ensure that a voxel position is Valid I.E. It is the same height as the others that an object is to spawn on and does not contain water.
    {
        if (heightMap[position.y, position.x] <= mapGenerator._regions[0].height || gridGenerator.GetHeight(position.y, position.x) != consistentHeight)
            return true;//Position is invalid if cell is water or is not equal to the consistent height

        if (!isOutpost) return false;
        foreach (Vector2Int pos in gridGenerator.coordinateBlacklist)//If the position is already in the coordinate blacklist
        {
            if (pos == position) return true;
        }

        return false;
    }


    #endregion

    #region Spawn World Objects
    /*=====================================
                    OBJECTS
     =====================================*/
    private void SpawnObjects()
    {
        float x = (heightMap.GetLength(1) / 2) - 1.5f; //Castle's x Position
        float y = gridGenerator.GetHeight(heightMap.GetLength(1) - 1, 0); //Height for the castle to spawn at
        float z = (heightMap.GetLength(1) / 2) - 1.5f;//Castle's y Position

        Vector3 spawnPos = new Vector3(x, y, z);//Get the castle's spawn position
        if (objectsInScene.Count > 0)//If there are previously objects in the scene, delete them
        {
            foreach (GameObject oldObject in objectsInScene)
                Destroy(oldObject);
        }

        GameObject newCastle = Instantiate(currentBiome.castle, spawnPos, Quaternion.Euler(new Vector3(0, -90, 0)));//Spawn the castle
        unitController.castle = newCastle;
        objectsInScene.Add(newCastle);//Add the castle to the objects list

        if (gameMode.type == GameMode.Outposts)//Disable the castle 
        {
            foreach (Transform child in newCastle.transform)
                child.gameObject.SetActive(false);
        }

        if (heightMap.GetLength(0) < smallMapSize) return;//If the map size is less than 25, spawn no outposts
        if (heightMap.GetLength(0) < mediumMapSize) { //else if the size is less than 30, spawn 1 outpost
            outpostController.spawnMode = SpawnMode.Limited;
            SpawnOutpost(); 
        }
        else if (heightMap.GetLength(0) < largeMapSize)//else if the size is less than 40, spawn 2 outpost
        {

            outpostController.spawnMode = SpawnMode.Limited;
            SpawnOutpost();
            SpawnOutpost();
            if (gameMode.type == GameMode.Outposts)
            {
                SpawnOutpost();
                outpostController.spawnMode = SpawnMode.Infinite;
            }
        }
    }

    private void SpawnOutpost()
    {
        GameObject parent = new GameObject();//The parent for the outpost
        parent.name = "OutpostParent";//Rename for testing purposes
        OutpostGroup outpostSettings = new OutpostGroup();
        GameObject newOutpost = Instantiate(outpost, GetOutpostPosition(outpostSettings), Quaternion.Euler(new Vector3(0, 90, 0)), parent.transform);//Spawn the outpost using GetOutpostPosition(), which will return a random position on the map (excludes user spawn area)
        outpostSettings.outpostObject = newOutpost;
        gridGenerator.outpostGroups.Add(outpostSettings);
        parent.transform.Rotate(new Vector3(0, 90, 0)); 
        objectsInScene.Add(parent); //Add the parent object and outpost object
        objectsInScene.Add(newOutpost);
    }
    private Vector3 GetOutpostPosition(OutpostGroup outpostSettings)//2x2 position
    {
        Vector2Int posToSpawn = GetRandomPosition();//Get a random position on the map
        int consistentHeight = gridGenerator.GetHeight(posToSpawn.y, posToSpawn.x);
        Vector2Int[] spawnPositions = new[]
        {
            posToSpawn,//The Initial Cell
            new Vector2Int(posToSpawn.x, posToSpawn.y + 1),//The position to the right
            new Vector2Int(posToSpawn.x + 1, posToSpawn.y + 1),//The position to the top right
            new Vector2Int(posToSpawn.x + 1, posToSpawn.y)//The position above
        };

        bool allPositionsValid = true;//Checks if all positions are valid for an outpost to spawn on
        foreach (Vector2Int position in spawnPositions)//Check if each position is valid, if not return false
        {
            if (IsInvalidPosition(position, consistentHeight, true))
                allPositionsValid = false;
        }

        if (allPositionsValid)//If positions are valid add them to the coordinate blacklist
        {
            Vector2Int[] AISpawnPositions = new[]// - - - - STORE OUTPOST SPAWNING POSITIONS FOR UNITS - - - - - -
             {
                //Top
                new Vector2Int(posToSpawn.x - 1, posToSpawn.y),
                new Vector2Int(posToSpawn.x - 1, posToSpawn.y + 1),
                //Left
                new Vector2Int(posToSpawn.x, posToSpawn.y - 1),
                new Vector2Int(posToSpawn.x + 1, posToSpawn.y - 1),
                //Right
                new Vector2Int(posToSpawn.x, posToSpawn.y + 2),
                new Vector2Int(posToSpawn.x + 1, posToSpawn.y + 2),
                //Bottom
                new Vector2Int(posToSpawn.x + 2, posToSpawn.y + 1),
                new Vector2Int(posToSpawn.x + 2, posToSpawn.y),
            };

            List<Vector2> outpostSpawnPositions = gameObject.GetComponent<UnitController>().AIOutpostSpawningPositions;
            foreach (Vector2 pos in AISpawnPositions) outpostSpawnPositions.Add(pos);


            outpostSettings.outpostCoordinates = spawnPositions;
            outpostSettings.spawnCoordinates = AISpawnPositions;
        }
        else
        {
            return GetOutpostPosition(outpostSettings);//If the positions were invalid, attempt again (Recursive - Don't freak out ethan)
        }

        float x = (posToSpawn.x + 0.5f) - ((heightMap.GetLength(1) - 1)) / 2;//The x position of the Outpost
        float z = (posToSpawn.y + 0.5f) - ((heightMap.GetLength(1) - 1)) / 2;//The y position of the Outpost
        return new Vector3(x, consistentHeight, z);//Return the position
    }

    private Vector2Int GetRandomPosition()//Gets a random position on the map
    {
        int randomX = Random.Range(1, mapGenerator._mapWidth - 1);//Random x, excluding edges
        int randomY = Random.Range(1, mapGenerator._mapHeight - 1);//Random y, excluding edges
        if (randomX > mapGenerator._mapWidth / 2 && randomY < mapGenerator._mapHeight / 2)//Ensure that the outpost cannot spawn in the player quarter of the map
        {
            return GetRandomPosition();
        }
        return new Vector2Int(randomX, randomY);
    }

    private void SpawnFinalObjects()
    {
        unitController.GenerateUnits();
    }
    #endregion

    #region DefaultToKnownMap
    private void DefaultToKnownMap()
    {
        int mapSize = 0;
        gridGenerator.waterHeight = 0.2f;
        switch (currentBiome.biome)
        {
            case BiomeType.Forest:
                mapGenerator._seed = this.mapSize == MapSize.Small ? 94161 : this.mapSize == MapSize.Medium ? 14423 : 27394;
                mapSize = this.mapSize == MapSize.Small ? 27 : this.mapSize == MapSize.Medium ? 29 : 39;
                break;
            case BiomeType.Desert:
                gridGenerator.waterHeight = -1;
                mapGenerator._seed = this.mapSize == MapSize.Small ? 38431 : this.mapSize == MapSize.Medium ? 87965 : 22172;
                mapSize = this.mapSize == MapSize.Small ? 23 : this.mapSize == MapSize.Medium ? 29 : 39;
                break;
            case BiomeType.Snow:
                mapGenerator._seed = this.mapSize == MapSize.Small ? 50493 : this.mapSize == MapSize.Medium ? 9496 : 945; ;
                mapSize = this.mapSize == MapSize.Small ? 23 : this.mapSize == MapSize.Medium ? 29 : 39;
                break;
            case BiomeType.Cherry:
                mapGenerator._seed = this.mapSize == MapSize.Small ? 76528 : this.mapSize == MapSize.Medium ? 48091 : 29060;
                mapSize = this.mapSize == MapSize.Small ? 23 : this.mapSize == MapSize.Medium ? 29 : 39;
                break;
            case BiomeType.Volcano:
                mapGenerator._seed = this.mapSize == MapSize.Small ? 35939 : this.mapSize == MapSize.Medium ? 14206 : 41291; ;
                mapSize = this.mapSize == MapSize.Small ? 23 : this.mapSize == MapSize.Medium ? 29 : 39;
                break;
            case BiomeType.Swamp:
                mapGenerator._seed = this.mapSize == MapSize.Small ? 11323 : this.mapSize == MapSize.Medium ? 69748 : 40800;
                mapSize = this.mapSize == MapSize.Small ? 23 : this.mapSize == MapSize.Medium ? 29 : 39;
                break;
            case BiomeType.Mesa:
                mapGenerator._seed = this.mapSize == MapSize.Small ? 31242 : this.mapSize == MapSize.Medium ? 4327 : 66932;
                mapSize = this.mapSize == MapSize.Small ? 23 : this.mapSize == MapSize.Medium ? 29 : 39;
                break;
            case BiomeType.Waterworld:
                mapGenerator._seed = this.mapSize == MapSize.Small ? 74479 : this.mapSize == MapSize.Medium ? 40551 : 43765;
                mapSize = this.mapSize == MapSize.Small ? 27 : this.mapSize == MapSize.Medium ? 29 : 39;
                break;
        }

        mapGenerator._mapHeight = mapSize;
        mapGenerator._mapWidth = mapSize;
        gridGenerator.width = mapSize;
        gridGenerator.height = mapSize;
        heightMap = Noise.GenerateNoiseMap(mapSize, mapSize, mapGenerator._seed, defaultSettings.noiseScale, defaultSettings.octaves,
            defaultSettings.persistance, defaultSettings.lacunarity, defaultSettings.offset);
        mapGenerator.GenerateMap();
        AlterBlackList();
    }
    #endregion
}

#region Structures
[System.Serializable]
public struct ChooseGameMode
{
    public GameMode type;
    public List<GameObject> UIToDisable;
    public GameObject UIToEnable;
    public string mainTitle;
}
[System.Serializable]
public struct SelectGameMode
{
    public ChooseGameMode killAllEnemies;
    public ChooseGameMode catapult;
    public ChooseGameMode captureOutPosts;
}

[System.Serializable]
public struct TimeOfDay
{
    public GameObject dayTime;
    public GameObject nightTime;
}
#endregion