using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{

    public ItemScriptableObject i_item;
    public int amount;
    public bool IsTakedByPlayer;
    public static event Action<Item> OnItemCreated;
    

    void Start()
    {
        OnItemCreated?.Invoke(this);  
        IsTakedByPlayer = false; 
    }
    

    public void OnDrop()
    {      
        IsTakedByPlayer = false;
        OnItemCreated?.Invoke(this);
    }

    
}