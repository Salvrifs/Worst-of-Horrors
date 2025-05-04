using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Bridge_quest : MonoBehaviour
{

    public GameObject End_scene;
    public List<GameObject> Bridge;
    public GGCameraMoving cameraController;

    public QuickSlotPanel inventory;
    public int activeBoard = 0;
    public void GiveItem()
    {
        ItemScriptableObject activeItem = inventory.quickslotParent.GetChild(inventory.currentQuickslotID).GetComponent<InventorySlot>().is_item;

        if (activeItem == null)
        {
            Debug.Log("��� ��������� ��������.");
            return;
        }

        if ((activeItem.itemName == "Desk") && (activeBoard != Bridge.Count))
        {

            Bridge[activeBoard].GetComponent<Collider>().enabled = true;
            Bridge[activeBoard].SetActive(true);

            activeBoard++;

            inventory.RemoveItem();

            Debug.Log($"�������: {activeItem.itemName}");

            if (activeBoard == (Bridge.Count))
            {
                Debug.Log("����� ��������.");
            }
        }
        else
        {
            Debug.Log("���� ������� �� ��������.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("����� ��������.");
            End_scene.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cameraController.SetControlEnabled(false);
        }
    }
}