using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig", order = 2)]
public class LevelConfig : ScriptableObject
{
    public int buildIndex;
    public BiomeConfig biome;
    public float timeBetweenWaves;
    public int waveCount;
    public bool alternate;
    public int enemyCountInWave; // Override biome min/max enemies if needed
    public int levelDifficultyMultiplier; // To adjust difficulty per level
    public int gridHeight;
    public int gridWidth;
    public int numberOfEntrances;
    public List<string> entranceDirections;
    public int minPathLength;
    public string endPoint;
    
}

