using System;
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
    public Animator GribAnimator;
    AudioSource audioSource;
    [SerializeField] AudioClip UseHeal;
    [SerializeField] AudioClip[] FallOfItem;
    [SerializeField] AudioClip DropItem;

    public static event Action<ItemScriptableObject> OnItemUsed;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
                    //audioSource.PlayOneShot(UseHeal);
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
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.IsTakedByPlayer = false;
                    
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item != null)
            {
                Transform itemsContainer = GameObject.FindGameObjectWithTag("item").transform;
                GameObject itemObject = Instantiate(quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.itemPrefab, player.position + Vector3.up + player.forward, Quaternion.identity, itemsContainer);
                
                //audioSource.PlayOneShot(DropItem);
                //StartCoroutine(waitOfFall());
                //audioSource.PlayOneShot(FallOfItem[UnityEngine.Random.Range(0, FallOfItem.Length)]);

                if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount <= 1)
                {
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().NullifySlotData();
                }
                else
                {
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount--;
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().textItemAmount.text = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount.ToString();
                }
                
                //quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.IsTakedByPlayer = false;
                itemObject.GetComponent<Item>().OnDrop();
            }
        }
        
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 7))
        {
            GameObject obj = hit.collider.gameObject;
            if (obj.CompareTag("NPCgrib") && GribAnimator.GetBool("IsChasing"))
            {
                // GameObject GribText = GameObject.Find("GribTextApologize");
                // GribText.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item != null)
                    {
                        if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item
                                .isConsumeable &&
                            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite == selectedSprite)
                        {
                            GribAnimator.SetBool("IsChasing", false);
                            GribAnimator.SetBool("ChasingStop", false);
                            GribAnimator.SetBool("IsApp", true);

                            if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount <= 1)
                            {
                                quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>()
                                    .NullifySlotData();
                            }
                            else
                            {
                                quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount--;
                                quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>()
                                    .textItemAmount.text = quickslotParent.GetChild(currentQuickslotID)
                                    .GetComponent<InventorySlot>().amount.ToString();
                            }

                            quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item
                                .IsTakedByPlayer = false;

                        }
                    }
                }
            }
        }
    }

    private void ChangeCharacteristics()
    { 
        if (int.Parse(healthText.text) + quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.changeHealth <= 100)
        {
            float newHealth = int.Parse(healthText.text) + quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.changeHealth;
            Mathf.Lerp(newHealth, 0, 100);
            healthText.text = newHealth.ToString();
            HealthBar.value = newHealth;
        }
        else
        {
            healthText.text = "100";
        }
        quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.IsTakedByPlayer = false;
        OnItemUsed?.Invoke(quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item);
    }

    private IEnumerator waitOfFall()
    {
        yield return new WaitForSeconds(1.5f);
    }
}