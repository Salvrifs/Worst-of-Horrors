using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Transform quickSlotPanel;
    public List<InventorySlot> slots = new List<InventorySlot>();
    private Camera mainCamera;
    public float reachDistance = 3f;

    void Start()
    {
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
                if (hit.collider.gameObject.GetComponent<Item>() != null)
                {
                    AddItem(hit.collider.gameObject.GetComponent<Item>().i_item, hit.collider.gameObject.GetComponent<Item>().amount);
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
}
