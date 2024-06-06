using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeConfig", menuName = "ScriptableObjects/BiomeConfig", order = 1)]
public class BiomeConfig : ScriptableObject
{
    public string biomeName;
    public int difficulty; // 1: easy, 2: medium, 3: hard
    public List<GameObject> enemyTypes;
    public float enemySpeed;
    public int minEnemies;
    public int maxEnemies;
    public List<LevelConfig> levels; // List of levels in this biome
}

