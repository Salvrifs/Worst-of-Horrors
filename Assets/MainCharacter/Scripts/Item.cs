using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class Item : MonoBehaviour
{

    public ItemScriptableObject i_item;
    public int amount;
    public bool IsTakedByPlayer;
    public static event Action<Item> OnItemCreated;
    public static event Action<Item> OnItemUsed;
    public Vector3 size;

    void Awake()
    {
        size = transform.localScale;
    }
    void Start()
    {
        size = new Vector3(111, 111, 111);
        OnItemCreated?.Invoke(this);  
        IsTakedByPlayer = false; 
    }
    

    public void OnDrop()
    {      
        IsTakedByPlayer = false;
        OnItemCreated?.Invoke(this);
    }

    public void OnUsed()
    {
        IsTakedByPlayer = false;
        OnItemUsed?.Invoke(this);
    }
    
}