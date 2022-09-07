using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapController : MonoBehaviour
{
    public MapRegion[] regions;
    public ColorBlock cb;
    public Button startButton;
    public MapTextBlocks mtb;

    void Start()
    {
        int max = regions.Length;
        int lastMapUnlocked = 0;
        for(int i = 0; i < (lastMapUnlocked < max ? lastMapUnlocked : max); i++)
        {
            Button b = regions[i].button;
            b.colors = cb;
            b.interactable = true;
        }

        regions[5].animator.enabled = true;
    }

    public void ButtonClick(int index)
    {
        startButton.onClick.AddListener(delegate { BeginLevel(index); });
        mtb.mapName.text = $"{regions[index].mapName}";
        mtb.units.text = $"Units: {regions[index].unitAmount}";
        mtb.enemies.text = $"Enemies: {regions[index].enemyAmount}";
        mtb.progress.text = $"4/15";
    }

    public void BeginLevel(int index)
    {
        
    }

    [System.Serializable]
    public struct MapTextBlocks
    {
        public TextMeshProUGUI mapName;
        public TextMeshProUGUI units;
        public TextMeshProUGUI enemies;
        public TextMeshProUGUI progress;
    }
}
