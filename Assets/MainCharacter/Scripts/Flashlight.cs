using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flashlight Item", menuName = "Item Scriptable Object/Flashlight Item")]
public class Flashlight : ItemScriptableObject 
{
    void Start()
    {
        itemType = ItemType.Lighting;
        isConsumeable = false;
    }
}
