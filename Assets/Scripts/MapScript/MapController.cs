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
        int lastMapUnlocked = SaveSystem.GetLatestUnlockedLevel();
        for(int i = 0; i < (lastMapUnlocked <= max ? lastMapUnlocked : max); i++)
        {
            Button b = regions[i].button;
            b.colors = cb;
            b.interactable = true;
        }

        regions[lastMapUnlocked - 1].animator.enabled = true;
        UpdateText(lastMapUnlocked - 1);
        mtb.progress.text = $"{lastMapUnlocked}/15";
    }

    public void ButtonClick(int index)
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(delegate { BeginLevel(index); });
        UpdateText(index);
        startButton.interactable = true;
    }

    private void UpdateText(int index)
    {
        mtb.mapName.text = $"{regions[index].mapData.mapName}";
        mtb.units.text = $"{regions[index].mapData.unitAmount}";
        mtb.enemies.text = $"{regions[index].mapData.enemyAmount}";
    }

    public void BeginLevel(int index)
    {
        SceneLoader.LoadScene($"{regions[index].mapData.levelName}");   
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
