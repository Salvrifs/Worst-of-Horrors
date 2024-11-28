using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotPanel : MonoBehaviour
{
    public Transform quickslotParent;
    public InventoryManager inventoryManager;
    public int currentQuickslotID = 0;
    public Sprite selectedSprite;
    public Sprite notSelectedSprite;
    private Transform player;

    public Text healthText;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        if (mouseWheel > 0.1)
        {
            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;

            if (currentQuickslotID >= quickslotParent.childCount - 1)
            {
                currentQuickslotID = 0;
            }
            else
            {
                currentQuickslotID++;
            }

            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
        }

        if (mouseWheel < -0.1)
        {

            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
            
            if (currentQuickslotID <= 0)
            {
                currentQuickslotID = quickslotParent.childCount - 1;
            }
            else
            {
                currentQuickslotID--;
            }

            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
        }

        for (int i = 0; i < quickslotParent.childCount; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                if (currentQuickslotID == i)
                {
                    if (quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite == notSelectedSprite)
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
                    }
                    else
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
                    }
                }
                else
                {
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
                    currentQuickslotID = i;
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item != null)
            {
                if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.isConsumeable && quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite == selectedSprite)
                {
                    
                    ChangeCharacteristics();

                    if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount <= 1)
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().NullifySlotData();
                    }
                    else
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount--;
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().textItemAmount.text = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount.ToString();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item != null)
            {
                GameObject itemObject = Instantiate(quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.itemPrefab, player.position + Vector3.up + player.forward, Quaternion.identity);

                if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount <= 1)
                {
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().NullifySlotData();
                }
                else
                {
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount--;
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().textItemAmount.text = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount.ToString();
                }
            }
        }
    }

    private void ChangeCharacteristics()
    { 
        if (int.Parse(healthText.text) + quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.changeHealth <= 100)
        {
            float newHealth = int.Parse(healthText.text) + quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.changeHealth;
            healthText.text = newHealth.ToString();
        }
        else
        {
            healthText.text = "100";
        }
    }
}
