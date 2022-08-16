using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridGenerator))]
public class GridInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GridGenerator myGridGenerator = (GridGenerator)target;

        if (GUILayout.Button("Generate Grid"))
        {
            
            myGridGenerator.RemoveChildren();
            myGridGenerator.GenerateLevel();
        } 
    }
}
