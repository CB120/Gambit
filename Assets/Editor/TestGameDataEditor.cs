using UnityEngine;
using UnityEditor;
using Saving;

[CustomEditor(typeof(TestGameData))]
public class TestGameDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TestGameData data = (TestGameData)target;

        if (GUILayout.Button("Assign"))
        {
            SavedData.gameData = data.gameData;
        }
        if (GUILayout.Button("Store"))
        {
            data.Store();
        }
        if (GUILayout.Button("Load"))
        {
            data.Load();
        }
        
    }
}
