using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCameraController : NewCameraMovement
{
    [SerializeField] private Renderer[] walls;
    private int index = 0;
    private void UpdateWallAlpha(int wallIndex)
    {
        foreach (var wall in walls)
        {
            Color alpha1 = wall.material.color;
            alpha1.a = 255;
            wall.material.color = alpha1;
        }
        Color alpha = walls[wallIndex].material.color;
        alpha.a = 0;
        walls[wallIndex].material.color = alpha;
        index++;
        if(index == walls.Length)
        {
            index = 0;
        }
        return;
    }

    public override void Update()
    {
        base.Update();
        if(Input.GetKeyDown(KeyCode.P)){
            UpdateWallAlpha(index);
            Debug.Log("Working");
            Debug.Log(index);
        }
    }
}
