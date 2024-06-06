using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Type", menuName = "TowerDefense/Tower Type")]

public class TowerTypeObject : ScriptableObject
{
    public string towerName;
    public Sprite towerSprite;
    public GameObject towerPrefab;
}

