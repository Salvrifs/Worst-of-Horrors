using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private Camera mainCamera;
    public float reachDistance = 3f;
    [Header("Flashlight Settings")]
[SerializeField] private FlashlightBeh flashlight;
[SerializeField] private Transform flashlightParent;
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
        mainCamera = Camera.main;
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
                    audioSource.PlayOneShot(UseHeal);
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

                else if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.itemType == ItemType.Lighting)
                {
                    Debug.Log("Nu fonarik derezhish");
                    // Активируем фонарик и устанавливаем позицию
                    if (flashlight != null)
                    {
                        Debug.Log("flashKKKAAAA");
                        flashlight.gameObject.SetActive(true);
                        flashlight.GetComponent<Rigidbody>().isKinematic = true;
                        flashlight.transform.SetParent(flashlightParent);
                        flashlight.transform.localPosition = new Vector3(0.4f, 0.6f, 0.6f); 
                        flashlight.transform.localEulerAngles = new Vector3(-90, 0, 0);  
                        flashlight.ToggleFlashlight();
                    }
                } 
            }
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, reachDistance))
        {
            Rozetka itemInfo = hit.collider.gameObject.GetComponent<Rozetka>();
            if (itemInfo != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.itemType == ItemType.Lighting)
                    {
                        flashlight.gameObject.SetActive(false);
                        flashlight.RechargeBattery();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            
            if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item != null)
            {
                GameObject itemObject;
                //
                //Выброс доски
                //
                if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.itemType == ItemType.Board)
                {
                    itemObject = Instantiate(quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.itemPrefab, player.position + Vector3.up + player.forward, Quaternion.identity);
                }
                //
                //Выброс фонарика
                //
                else if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.itemType == ItemType.Lighting)
                {
                    Transform itemsContainer = GameObject.FindGameObjectWithTag("item").transform;
                    itemObject = Instantiate(quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.itemPrefab, player.position + Vector3.up + player.forward, Quaternion.identity, itemsContainer);
                    flashlight.gameObject.SetActive(false);
                }
                //
                //Выьрос хила 
                //
                else
                {
                    Transform itemsContainer = GameObject.FindGameObjectWithTag("item").transform;
                    itemObject = Instantiate(quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item.itemPrefab, player.position + Vector3.up + player.forward, Quaternion.identity, itemsContainer);
                }
                

                audioSource.PlayOneShot(DropItem);
                StartCoroutine(waitOfFall());
                audioSource.PlayOneShot(FallOfItem[UnityEngine.Random.Range(0, FallOfItem.Length)]);

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
                if (itemObject.GetComponent<Item>().i_item.itemType != ItemType.Board)
                {
                    itemObject.GetComponent<Item>().OnDrop();
                }
                else if (itemObject.GetComponent<Item>().i_item.itemType == ItemType.Board)
                {
                    flashlight.gameObject.SetActive(false);
                    flashlight.GetComponent<Rigidbody>().isKinematic = false;
                    flashlight.transform.position = player.position + Vector3.up + player.forward;
                    flashlight.transform.localPosition = player.position + Vector3.up + player.forward;
                }
            }
        }
        
        //RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 7))
        {
            GameObject obj = hit.collider.gameObject;
            if (obj.CompareTag("NPCgrib") && GribAnimator.GetBool("IsChasing"))
            {
                // GameObject GribText = GameObject.Find("GribTextApologize");
                // GribText.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    TryCalmNPC();

                    /*if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item != null)
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
                    }*/
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

    public void RemoveItem() 
    {
        if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().is_item != null)
        {
            quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().NullifySlotData();
        }
    }

    private IEnumerator waitOfFall()
    {
        yield return new WaitForSeconds(1.5f);
    }

    private bool TryCalmNPC()
{
    RaycastHit hit;
    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 7f))
    {
        if (hit.collider.CompareTag("NPCgrib"))
        {
            InventorySlot slot = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>();
            if (slot.is_item != null && slot.is_item.isConsumeable)
            {
                slot.amount--;
                audioSource.PlayOneShot(UseHeal);
                
                if (slot.amount <= 0) 
                    slot.NullifySlotData();
                else
                    slot.textItemAmount.text = slot.amount.ToString();

                GribAnimator.SetBool("IsChasing", false);
                GribAnimator.SetBool("IsApp", true);
                return true;
            }
            return false;
        }
        return false;
    }
    return false;
}
}

