using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Saving;
public class MapController : MonoBehaviour
{
    public MapRegion[] regions;
    public ColorBlock cb;
    public Button startButton;
    public MapTextBlocks mtb;

    void Start()
    {
        int max = regions.Length;
        //int lastMapUnlocked = SaveSystem.GetLatestUnlockedLevel();
        Debug.Log(SavedData.GameData);
        int lastMapUnlocked = SavedData.GameData.levelsCompleted;
        for (int i = 0; i < (lastMapUnlocked + 1 <= max ? lastMapUnlocked + 1 : max); i++)
        {
            Button b = regions[i].button;
            b.colors = cb;
            b.interactable = true;
        }

        regions[lastMapUnlocked].animator.enabled = true;
        UpdateText(lastMapUnlocked);
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

    public void ReturnToMenu()
    {
        SceneLoader.LoadScene("MainMenu");
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
