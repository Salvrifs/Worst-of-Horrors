using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CollectableItem : MonoBehaviour
{
    public GameObject TakeText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (TakeText.activeSelf)
            TakeText.SetActive(false);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 7))
        {
            Debug.Log(hit.collider.tag);
            CollectableItem itemInfo = hit.collider.GetComponent<CollectableItem>();
            if (itemInfo != null)
            {
                TakeText.SetActive(true);
            }
        }
    }
}
