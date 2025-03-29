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

    [SerializeField] private Slider HealthBar;

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
        //
        //Использование предмета
        //
        if (Input.GetKeyDown(KeyCode.Q))
        {
            InventorySlot curr_slot = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>();
            Item curr_item = curr_slot.is_item;
            
            if (curr_item != null)
            {
                if (curr_item.i_item.isConsumeable && quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite == selectedSprite)
                {
                    
                    ChangeCharacteristics();
                    if (curr_slot.amount <= 1)
                    {
                        curr_slot.NullifySlotData();
                    }
                    else
                    {
                        curr_slot.amount--;
                        curr_slot.textItemAmount.text = curr_slot.amount.ToString();
                    }
                }
                Destroy(curr_item.gameObject);
            }


            
        }
        //
        //Сброс предмета
        //
        if (Input.GetKeyDown(KeyCode.B))
        {
            //InventorySlot curr_Slot = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>();
            //Transform curr_Item = GameObject.Find(curr_Slot.is_item.name).transform; 
            //Debug.Log($"PLAYYYYYYYYYYEEEEEEERRRR: {curr_Item.name}");
            
            InventorySlot curr_slot = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>();
            Transform curr_item = curr_slot.is_item.transform;  

            if (curr_item != null)
            {
              
                //GameObject itemObject = Instantiate(Curr_Slot.is_item.itemPrefab, player.position + Vector3.up + player.forward, Quaternion.identity);
                curr_item.position = player.position + Vector3.up + player.forward;
                curr_item.rotation = Quaternion.identity;
                
                MakeVisible(curr_item);

                if (curr_slot.amount <= 1)
                {
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().NullifySlotData();
                }
                else
                {
                    curr_slot.amount--;
                    curr_slot.textItemAmount.text = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount.ToString();
                }
                curr_slot.is_item.IsTakedByPlayer = false;
                Debug.Log($"Parent: {curr_slot.is_item.transform.parent}");
                curr_slot.is_item.gameObject.transform.SetParent(null);
                //Debug.Log($"Parent: {curr_slot.is_item.transform.parent}");
            }
        }
    }
    //
    //              ВСПОМОГАТЕЛЬНАЯ
    //Сделать предмет видимым (для сброса предмета или кражи)
    //
    private void MakeVisible(Transform currItem)
    {
        Renderer itemRender = currItem.GetComponent<Renderer>();
        if (itemRender != null)
        {
            itemRender.enabled = true;
        }

        Collider itemCollider = currItem.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }

        currItem.GetComponent<Rigidbody>().isKinematic = false;
    }

    
    //
    //Изменение характеристик игрока
    //
    private void ChangeCharacteristics()
    { 
        if (int.Parse(healthText.text) + quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.i_item.changeHealth <= 100)
        {
            float newHealth = int.Parse(healthText.text) + quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.i_item.changeHealth;
            Mathf.Lerp(newHealth, 0, 100);
            healthText.text = newHealth.ToString();
            HealthBar.value = newHealth;
        }
        else
        {
            healthText.text = "100";
        }
    }

    
}
