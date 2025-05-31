using UnityEngine;
using System;

[Serializable]
public class ItemDropConfig
{
    public ItemType itemType;
    public GameObject prefab;
    [Range(0, 1)] public float dropChance;
    public int minAmount = 1;
    public int maxAmount = 1;
}

[CreateAssetMenu(fileName = "New Enemy Drop Config", menuName = "Game/Enemy Drop Config")]
public class EnemyDropConfig : ScriptableObject
{
    public ItemDropConfig[] dropConfigs;
} 