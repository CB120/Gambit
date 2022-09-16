using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeController : MonoBehaviour
{
   public Biomes biomes;
}

[System.Serializable]
public struct Biomes
{
    public Biome Forest;
    public Biome Desert;
    public Biome Snow;
    public Biome Cherry;
    public Biome Volcano;
    public Biome Swamp;
    public Biome Mesa;
    public Biome Waterworld;
}

[System.Serializable]
public struct Biome
{
    public BiomeType biome;
    public TerrainType[] regions;
    public GameObject tree;
    public GameObject castle;
    public bool spawnForests;
    public Sprite mapImage;
}

[System.Serializable]
public struct MapSettings
{
    public int noiseScale;
    public int octaves;
    public float persistance;
    public Vector2 offset;
    public float lacunarity;
}

public enum BiomeType
{
    Forest,
    Desert,
    Snow,
    Cherry,
    Volcano,
    Swamp,
    Mesa,
    Waterworld,
}

public enum MapSize
{
    Small,
    Medium,
    Large,
}
