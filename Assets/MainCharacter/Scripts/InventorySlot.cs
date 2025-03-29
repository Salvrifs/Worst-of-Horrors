using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    //public ItemScriptableObject is_item;
    public Item is_item;
    public int amount;
    public bool isEmpty = true;
    public GameObject iconGO;
    public TMP_Text textItemAmount;

    private void Start()
    {
        textItemAmount = transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        iconGO = transform.GetChild(0).GetChild(0).gameObject;
    }

    public void SetIcon(Sprite icon)
    {
        iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        iconGO.GetComponent<Image>().sprite = icon;
    }

    public void NullifySlotData()
    {
        // ������� �������� InventorySlot
        is_item.i_item = null;
        is_item = null;
        amount = 0;
        isEmpty = true;
        iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        iconGO.GetComponent<Image>().sprite = null;
        textItemAmount.text = "";
    }
}
