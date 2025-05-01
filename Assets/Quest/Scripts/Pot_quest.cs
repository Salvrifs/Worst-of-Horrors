using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pot_quest : MonoBehaviour
{

    public GameObject End_scene;
    public GGCameraMoving cameraController;

    public QuickSlotPanel inventory;
    public List<string> requiredItems;
    private List<string> placedItems = new List<string>();
    public void GiveItem()
    {
        ItemScriptableObject activeItem = inventory.quickslotParent.GetChild(inventory.currentQuickslotID).GetComponent<InventorySlot>().is_item;

        if (activeItem == null)
        {
            Debug.Log("Нет активного предмета.");
            return;
        }

        if (requiredItems.Contains(activeItem.itemName))
        {
            placedItems.Add(activeItem.itemName);

            inventory.RemoveItem();

            Debug.Log($"Положил: {activeItem.itemName}");

            if (IsQuestCompleted())
            {
                EndGame();
            }
        }
        else
        {
            Debug.Log("Этот предмет не подходит.");
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
        //Debug.Log("Квест завершен.");
        End_scene.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cameraController.SetControlEnabled(false);
    }
}