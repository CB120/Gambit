using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MapGenerator : MonoBehaviour
{
    [Header("Map parameters")]
    public int _mapWidth = 2;
    public int _mapHeight = 2;

    
    public int _seed = 0;
    [SerializeField]
    private Vector2 _offset = Vector2.zero;

    [SerializeField]
    private float _noiseScale = 1;

    [SerializeField]
    private int _octaves = 1;
    [SerializeField] [Range(0, 1)]
    private float _persistance = 0.5f;
    [SerializeField]
    private float _lacunarity = 2f;

    [Header("Map generator display")]
    public bool AutoUpdate = true;

    [SerializeField]
    private MapDisplay _mapDisplay = null;

    [SerializeField]
    private bool _generateObstacles = false;

    public static float[,] globalHeightMap;
    public static TerrainType[] globalRegions;

    public enum DrawMode { NoiseMap, RegionColorMap, Mesh, Mesh3D };
    [SerializeField]
    private DrawMode _drawMode = DrawMode.NoiseMap;
    [SerializeField]
    public TerrainType[] _regions = null;

    [Header("Forests")]
    [SerializeField] private Vector2Int forestMinMax;
    [SerializeField] private GameObject forestPrefab;
    public List<GameObject> forestObjects;
    [SerializeField] private float forestOffset = 0.5f;
    [SerializeField] private int distanceFromEdges;
    [SerializeField] private bool doGenerateForest = true;
    [SerializeField] bool doPCGForest = false;
    [SerializeField] List<Vector2Int> forestCoordinates = new List<Vector2Int>();
    [HideInInspector] public List<Vector2Int> forestCoordinatesInternal = new List<Vector2Int>();

    bool lateStartRun = false;

    private void Awake()
    {
        GenerateMap();
    }

    void LateStart()
    {
        lateStartRun = true;
        // GridController.SetForestCells(forestCoordinatesInternal);
    }

    void Update()
    {
        if (!lateStartRun) LateStart();
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(_mapWidth, _mapHeight, _seed, _noiseScale, _octaves, _persistance, _lacunarity, _offset);
        globalHeightMap = noiseMap;
        globalRegions = _regions;
        Color[] colormap = SetColorMap(noiseMap);

        // Display
        if (_drawMode == DrawMode.NoiseMap)
        {
            _mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (_drawMode == DrawMode.RegionColorMap)
        {
            _mapDisplay.DrawTexture(TextureGenerator.TextureFromColormap(colormap, _mapWidth, _mapHeight));
        }
        else if (_drawMode == DrawMode.Mesh)
        {
            var meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, _regions[0].height, _regions[2].height, _regions[3].height, _regions[4].height);
            _mapDisplay.DrawMesh(meshData, TextureGenerator.TextureFromColormap(colormap, _mapWidth, _mapHeight));
            if (_generateObstacles)
                ObstacleGenerator.GenerateObstacleMesh(meshData);
        }
        else if (_drawMode == DrawMode.Mesh3D)
        {
            _mapDisplay.DrawMesh3D(MeshGenerator3D.GenerateTerrainMesh(noiseMap), TextureGenerator.TextureFromColormap(colormap, _mapWidth, _mapHeight));
        }

        if (doGenerateForest)
        {
            GenerateForests();
        }
    }

    public void GenerateForests()
    {
        //Get the width and height of the maps "pixels"
        int width = globalHeightMap.GetLength(0);
        int height = globalHeightMap.GetLength(1);

        //Ensure the distance from edges is not greater than the width or height
        if (distanceFromEdges >= width || distanceFromEdges >= height)
        {
            Debug.LogWarning("Distance From Edges is Greater than Width OR Height \n Setting distance to 0");
            distanceFromEdges = 0;
        }

        //Regenerate forests for each game
        foreach (GameObject forest in forestObjects)
        {
            DestroyImmediate(forest);
        }
        //clear the list keeping track of current forest objects
        forestObjects.Clear();
        forestCoordinatesInternal.Clear();

        if (doPCGForest){
            List<Vector2> positions = new List<Vector2>();

            //get a random amount of forest objects to spawn
            int randRange = Random.Range(forestMinMax.x, forestMinMax.y);
            for (int i = 0; i < randRange; i++)
            {
                CreateForest(positions, 0);
            }
            gameObject.transform.Rotate(new Vector3(0, 90, 0));
        } else {
            PlaceNonPCGForest();
        }
    }

    GameObject CreateForest(List<Vector2> positions, int timesRun)
    {
        //Guard to ensure recursion is not infinite
        if (timesRun > 10) return null;

        int width = globalHeightMap.GetLength(0);
        int height = globalHeightMap.GetLength(1);
        //Get a random position to spawn the forest object within the constraints
        int x = Random.Range(0 + distanceFromEdges, width - distanceFromEdges);
        int y = Random.Range(0 + distanceFromEdges, height - distanceFromEdges);

        //coordinates to spawn at
        float xPos = (x - (width - 1) / 2) - forestOffset;
        float yPos = (y - (height - 1) / 2) - forestOffset;

        //Only spawn a forest if it has a valid spawn point (i.e. not on water)
        if (globalHeightMap[y, x] > globalRegions[0].height && !positions.Contains(new Vector2(xPos, yPos)))
        {
            positions.Add(new Vector2(xPos, yPos));
            forestCoordinatesInternal.Add(new Vector2Int(x, y));
            GameObject newForest = Instantiate(forestPrefab, new Vector3(xPos, GetHeight(y, x), yPos), Quaternion.identity, transform);
            newForest.transform.parent = this.gameObject.transform;
            forestObjects.Add(newForest);
            return newForest;
        }

        //Recursive if a forest object aready exists there

        timesRun++; //ensure the recursion does not run infinitely
        return CreateForest(positions, timesRun);
    }

    void PlaceNonPCGForest(){
        //Get the width and height of the maps "pixels"
        int width = globalHeightMap.GetLength(0);
        int height = globalHeightMap.GetLength(1);

        //Ensure the distance from edges is not greater than the width or height
        if (distanceFromEdges >= width || distanceFromEdges >= height){
            Debug.LogWarning("Distance From Edges is Greater than Width OR Height \n Setting distance to 0");
            distanceFromEdges = 0;
        }

        //Regenerate forests for each game
        foreach (GameObject forest in forestObjects){
            DestroyImmediate(forest);
        }
        //clear the list keeping track of current forest objects
        forestObjects.Clear();
        forestCoordinatesInternal.Clear();

        foreach (Vector2Int v in forestCoordinates){
            //yeah ik it's repeated code, sue me
            float xPos = (v.y - (width - 1) / 2) - forestOffset;
            float yPos = ((height - 1) / 2) - v.x - forestOffset + 1;

            forestCoordinatesInternal.Add(v);
            GameObject newForest = Instantiate(forestPrefab, new Vector3(xPos, GetHeight(v.y, v.x), yPos), Quaternion.identity, transform);
            forestObjects.Add(newForest);
        }
    }

    int GetHeight(int x, int y)
    {
        float mapValue = globalHeightMap[x, y];

        for (int i = 2; i < globalRegions.Length; i++)//determine the elevation
        {
            if (mapValue < globalRegions[i].height)
            {
                return i - 1;
            }
        }

        return 0;
    }

    private Color[] SetColorMap(float[,] noiseMap)
    {
        Color[] colormap = new Color[_mapWidth * _mapHeight];
        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];

                float lowerHeight = 0;
                for (int i = 0; i < _regions.Length; i++)
                {
                    TerrainType region = _regions[i];

                    if (currentHeight <= region.height)
                    {
                        if (i > 0)
                            lowerHeight = _regions[i - 1].height;

                        float t = Mathf.InverseLerp(lowerHeight, region.height, currentHeight);
                        Color regionColor = Color.Lerp(region.startColor, region.endColor, t);
                        colormap[y * _mapWidth + x] = regionColor;
                        break;
                    }
                }
            }
        }
        return colormap;
    }

    private void OnValidate()
    {
        if (_mapHeight < 1)  _mapHeight = 1;
        if (_mapWidth < 1)   _mapWidth = 1;
        if (_lacunarity < 1) _lacunarity = 1;
        if (_octaves < 0)    _octaves = 0;
    }
}


[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color startColor;
    public Color endColor;
}
