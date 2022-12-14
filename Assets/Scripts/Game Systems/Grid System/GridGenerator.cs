using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OutpostGroup
{
    public Vector2Int[] outpostCoordinates;
    public Vector2Int[] spawnCoordinates;

    public GameObject outpostObject;

    [HideInInspector] public bool captured = false;
    [HideInInspector] public int health;
}


[ExecuteInEditMode]
public class GridGenerator : MonoBehaviour
{
    [SerializeField] GameObject cellPrefab;
    [SerializeField] public int height;
    [SerializeField] public int width;
    [SerializeField] float spacing;
    [SerializeField] public float waterHeight = 0.2f;
    public List<Cell> cells;
    [SerializeField] private MapGenerator mapGenerator;
    //MapGenerator.globalHeightmap[x,y] is a static variable that contains the heightmap of the terrain

    public List<Vector2Int> coordinateBlacklist = new List<Vector2Int>();

    public List<OutpostGroup> outpostGroups = new List<OutpostGroup>();

    public void GenerateLevel () {
        cells.Clear();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < height; x++)
            {
                if (!(MapGenerator.globalHeightMap[y, x] <= mapGenerator._regions[0].height))
                {
                    if (!coordinateBlacklist.Contains(new Vector2Int(x, y))){
                        GameObject cell = Instantiate(
                            cellPrefab,
                            new Vector3(x * spacing - ((height - 1) * spacing) / 2,
                            GetHeight(y, x),
                            y * spacing - ((height - 1) * spacing) / 2),
                            Quaternion.identity,
                            transform
                        );
                        Cell cellScript = cell.GetComponent<Cell>();
                        cellScript.coordinates = new Vector2Int(x, y);
                        cellScript.height = GetHeight(y, x);
                        cells.Add(cellScript);

                        if (OutpostGroupContains(new Vector2Int(x, y))){
                            cellScript.mode = CellMode.AttackOnly;
                        }
                    }
                }

            }
        }
        gameObject.transform.Rotate(new Vector3(0, 90, 0)); //FOR DECLAN - (We need to figure out how to rotate the grid properly)
    }

    public int GetHeight(int x, int y)
    {
        float mapValue = MapGenerator.globalHeightMap[x, y];

        for(int i = 2; i < MapGenerator.globalRegions.Length; i++)//determine the elevation
        {
            if(mapValue < MapGenerator.globalRegions[i].height)
            {
                return i - 1;
            }
        }
        return 0;
    }

    public void RemoveChildren()
    {
        for (int i = this.transform.childCount; i > 0; --i)
        {
            GameObject.DestroyImmediate(this.transform.GetChild(0).gameObject);
        }
        mapGenerator.GenerateMap();
    }

    public List<GameObject> GenerateForests(GameObject forestObject, int range)
    {
        List<GameObject> trees = new();
        for(int i = 0; i < range; i++)
        {
            int randIndex = Random.Range(0, cells.Count - 1);
            if (!cells[randIndex].isForest)
            {
                cells[randIndex].isForest = true;

                GameObject tree = Instantiate(forestObject, cells[randIndex].gameObject.transform.position, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));

                trees.Add(tree);
            }
        }

        return trees;
    }

    bool OutpostGroupContains(Vector2Int coordinates){
        foreach (OutpostGroup g in outpostGroups){
            foreach (Vector2Int c in g.outpostCoordinates){
                if (c == coordinates) return true;
            }
        }
        return false;
    }
}
