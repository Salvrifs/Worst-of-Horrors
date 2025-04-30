using System.Collections.Generic;
using UnityEngine;

public class Pot_quest : MonoBehaviour
{
    public QuickSlotPanel inventory;
    public List<string> requiredItems;
    private List<string> placedItems = new List<string>();
    public void GiveItem()
    {
        ItemScriptableObject activeItem = inventory.quickslotParent.GetChild(inventory.currentQuickslotID).GetComponent<InventorySlot>().is_item;

        if (activeItem == null)
        {
            Debug.Log("��� ��������� ��������.");
            return;
        }

        if (requiredItems.Contains(activeItem.itemName))
        {
            placedItems.Add(activeItem.itemName);

            inventory.RemoveItem();

            Debug.Log($"�������: {activeItem.itemName}");

            if (IsQuestCompleted())
            {
                EndGame();
            }
        }
        else
        {
            Debug.Log("���� ������� �� ��������.");
        }
    }

    private bool IsQuestCompleted()
    {
        foreach (var itemName in requiredItems)
        {
            if (!placedItems.Contains(itemName))
                return false;
        }
        return true;
    }

    private void EndGame()
    {
        Debug.Log("����� ��������.");
    }
}