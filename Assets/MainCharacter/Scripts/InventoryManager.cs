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
            InventorySlot slot = quickSlotPanel.GetChild(i).GetComponent<InventorySlot>();
            if (slot != null)
            {
                slots.Add(slot);
            }
        }
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(ray, out hit, reachDistance))
            {
                Item item = hit.collider.gameObject.GetComponent<Item>();
                if (item != null)
                {
                    AddItem(item.i_item, item.amount);
                    VanishMode(hit.collider.gameObject);
                    hit.collider.gameObject.transform.SetParent(transform);
                    Vector3 playerPosition = transform.position; 
                    hit.collider.gameObject.transform.position = playerPosition + transform.forward * 1.5f;
                }
            }
        } 
    }

    private void AddItem(ItemScriptableObject _item, int _amount = 1)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.isEmpty)
            {
                slot.is_item = _item;
                slot.amount = _amount;
                slot.isEmpty = false;
                slot.SetIcon(_item.icon);
                slot.textItemAmount.text = _amount.ToString();
                return;
            }
        }
    }

    void VanishMode(GameObject obj)
    {
        Renderer itemRender = obj.GetComponent<Renderer>();
        if (itemRender != null)
        {
            itemRender.enabled = false;
        } 

        Collider itemCollider = obj.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }
    }
}
