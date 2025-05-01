using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour
{
    
    public Transform quickSlotPanel;
    public List<InventorySlot> slots = new List<InventorySlot>();
    private Camera mainCamera;
    public float reachDistance = 3f;
    [SerializeField] AudioClip TakeBottle;
    [SerializeField] AudioClip TakeMushrom;
    [SerializeField] AudioClip TakeFlashlight;

    [SerializeField] AudioSource audioSource;

    public static event Action<Item> OnDestroyItem;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        for (int i = 0; i < quickSlotPanel.childCount; i++) 
        {
            if (quickSlotPanel.GetChild(i).GetComponent<InventorySlot>() != null )
            {
                slots.Add(quickSlotPanel.GetChild(i).GetComponent<InventorySlot>());
            }
        }
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetKeyDown(KeyCode.E)) {
            if (Physics.Raycast(ray, out hit, reachDistance))
            {
                if (hit.collider.TryGetComponent(out Pot_quest Pot))
                {
                    Pot.GiveItem();
                }
                
                Item itemInfo = hit.collider.gameObject.GetComponent<Item>();
                if (itemInfo != null && (itemInfo.i_item.itemType == ItemType.Heal) )
                {
                    PlaySoundTakingItem(itemInfo.i_item.name);
                    AddItem(itemInfo.i_item, itemInfo.amount);
                    itemInfo.i_item.IsTakedByPlayer = true;
                    OnDestroyItem?.Invoke(itemInfo);
                    Destroy(hit.collider.gameObject);
                }
                else if (itemInfo != null && itemInfo.i_item.itemType == ItemType.Lighting)
                {
                    AddItem(itemInfo.i_item, itemInfo.amount);
                    itemInfo.i_item.IsTakedByPlayer = true;
                    OnDestroyItem?.Invoke(itemInfo);
                    //hit.collider.gameObject.transform.SetParent(quickSlotPanel);
                    itemInfo.gameObject.SetActive(false);
                }

                else if (itemInfo != null && itemInfo.i_item.itemType == ItemType.Board)
                {
                    AddItem(itemInfo.i_item, itemInfo.amount);
                    itemInfo.i_item.IsTakedByPlayer = true;
                    Destroy(hit.collider.gameObject);
                } 
            }
        } 
    }

    private void AddItem(ItemScriptableObject _item, int _amount)
    {


        foreach (InventorySlot slot in slots)
        {
            if (slot.is_item == _item)
            {
                if (slot.amount + _amount <= _item.maximumAmount)
                {
                    slot.amount += _amount;
                    slot.textItemAmount.text = slot.amount.ToString();
                    return;
                }
                break;
            }
        }
        foreach (InventorySlot slot in slots)
        {
            if (slot.isEmpty)
            {
                slot.is_item = _item;
                slot.amount = _amount;
                slot.isEmpty = false;
                slot.SetIcon(_item.icon);
                slot.textItemAmount.text = _amount.ToString();
                
                break;
            }
        }
    }

    //
    //Проигрывание звука подбора соответствующего предмета
    //
    private void PlaySoundTakingItem(string itemName)
    {
        //
        //Гриб
        //
        if (itemName == "Mushroom")
        {
            Debug.Log("Sound of taking mush");
            audioSource.PlayOneShot(TakeMushrom);
        }
        //
        //Зелье
        //
        else if (itemName == "Potion")
        {
            Debug.Log("Sound of taking bottle");
            audioSource.PlayOneShot(TakeBottle);
        }
        //
        //Фонарик
        //
        else if (itemName == "Flashlight")
        {
            Debug.Log("Sound of taking dab");
            audioSource.PlayOneShot(TakeFlashlight);
        }
        else
        {
            Debug.Log("no sound? Why?");
        }
    }
}