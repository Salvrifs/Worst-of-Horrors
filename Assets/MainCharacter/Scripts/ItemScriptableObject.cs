using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType {
    Lighting,
    Heal,
    Board
}

public class ItemScriptableObject : ScriptableObject
{
    public string itemName;
    public int maximumAmount;
    public string itemDescription; // �� ������������
    public Sprite icon;
    public GameObject itemPrefab;
    public ItemType itemType;
    public bool isConsumeable;

    [Header("Consumable Characteristics")]
    public float changeHealth;
}
