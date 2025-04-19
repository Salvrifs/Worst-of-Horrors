using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Desk Item", menuName = "Item Scriptable Object/Desk Item")]
public class DeskItem : ItemScriptableObject
{
    void Start()
    {
        itemType = ItemType.Board;
        isConsumeable = false;
    }
}