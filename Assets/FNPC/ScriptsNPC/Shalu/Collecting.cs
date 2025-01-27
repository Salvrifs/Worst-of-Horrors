using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CollectingNPC : MonoBehaviour
{
    private NavMeshAgent m_agent;
    private Transform Player;
    private Transform currentTarget;
    private Vector3 DropOffPoint;
    private List<Transform> items = new List<Transform>();
    
    private int RandInd;
    private bool IsHolding;
    private bool isSteal;

    public Transform quickslotParent;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        m_agent = GetComponent<NavMeshAgent>();
        InitializeItems();
        MoveToItem();
    }

    void Update()
    {
        if (!IsHolding && currentTarget == null)
        {
            MoveToItem();
            return; 
        }

        if (isSteal && !IsHolding)
        {
            HandleStealing();
            return;
        }

        if (currentTarget != null)
        {
            HandleItemInteraction();
        }

        
    }

    private void HandleStealing()
    {
        float distanceToPlayer = Vector3.Distance(Player.position, transform.position);
        if (distanceToPlayer <= 5f) // Установить дистанцию воровства
        {
            StealItemFromPlayer();
            return;
        }

        MoveAwayFromPlayer();
    }

    private void HandleItemInteraction()
    {
        float distance = Vector3.Distance(m_agent.transform.position, currentTarget.position);
        if (distance < m_agent.stoppingDistance)
        {
            if (!IsHolding)
                PickUpItem();
            else
                DropItem();
        }
    }

    private void StealItemFromPlayer()
    {
        while (true)
        {
            // Логика кражи
            RandInd = Random.Range(0, quickslotParent.childCount);

            if (quickslotParent.GetChild(RandInd) != null)
            {
                InventorySlot slot = quickslotParent.GetChild(RandInd).GetComponent<InventorySlot>();
                if (slot.is_item != null && slot.amount > 0)
                {
                    // Кража с проверкой
                    GameObject obj = Instantiate(slot.is_item.itemPrefab, transform.position + Vector3.up, Quaternion.identity);
                    obj.transform.SetParent(transform);
                    IsHolding = true;

                    slot.amount--;
                    slot.textItemAmount.text = slot.amount.ToString();
                
                    if (slot.amount <= 0)
                        slot.NullifySlotData();

                    currentTarget.position = currentTarget.position + Vector3.up;
                    currentTarget.parent = m_agent.transform;
                    MakeVisible();
                    MoveAwayFromPlayer();
                    return;
                }
            }

            else 
            {
                RandInd = Random.Range(0, quickslotParent.childCount);
            }
        }
    }

    private void MoveToItem()
    {
        foreach (Transform item in items)
        {
            if (!item.GetComponent<Item>().IsTaked)
            {
                currentTarget = item;
                item.GetComponent<Item>().IsTaked = true;
                isSteal = currentTarget.parent == Player ? true : false;
                m_agent.SetDestination(currentTarget.position);
                return; 
            }
        }

        // Если все предметы собраны, проверка инвентаря игрока
    /*    currentTarget = Player;
        isSteal = true;
        m_agent.SetDestination(Player.position);
    */


    }

    private void MoveAwayFromPlayer()
    {
        Vector3 directionAway = (transform.position - Player.position).normalized;
        Vector3 targetPosition = transform.position + directionAway * 1.5f; // Дистанция убегания
        m_agent.SetDestination(targetPosition);
    }

    private void PickUpItem()
    {
        if (currentTarget != null)
        {
            IsHolding = true;
            MoveToDrop();
        }
    }

    void MoveToDrop()
    {
        Debug.Log("Двигается к точке сброса");
        GenerateDropOffPoint();
        m_agent.SetDestination(DropOffPoint);
        //m_animator.SetTrigger("GoToTarget");
        Debug.Log($"Движение к точке сброса на позицию {DropOffPoint}");
    }

    private void DropItem()
    {
        if (currentTarget != null)
        {
            currentTarget.SetParent(null);
            currentTarget.position = DropOffPoint;
            IsHolding = false;
            currentTarget.GetComponent<Item>().IsTaked = false;
            isSteal = false;
            currentTarget = null;
            MoveToItem();
        }
    }

    private void InitializeItems()
    {
        Transform ItemsContainer = GameObject.FindGameObjectWithTag("item")?.transform;
        if (ItemsContainer != null)
        {
            foreach (Transform tr in ItemsContainer)
            {
                items.Add(tr);
            }
        }
    }

    void GenerateDropOffPoint()
{
    Terrain terrain = Terrain.activeTerrain;

    if (terrain == null)
    {
        Debug.LogError("Террайн не найден");
        return;
    }

    float terrainWidth = terrain.terrainData.size.x;
    float terrainHeight = terrain.terrainData.size.z;
    float groundHeight; 
    float maxSlopeAngle = 30f;  
    
    while (true)
    {
        float RandomX = Random.Range(0, terrainWidth);
        float RandomZ = Random.Range(0, terrainHeight);
        groundHeight = terrain.SampleHeight(new Vector3(RandomX, 0, RandomZ));
        // Проверяем нормаль к поверхности в этой точке
        Vector3 normal = Vector3.up; 
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(RandomX, groundHeight + 100, RandomZ), Vector3.down, out hit, Mathf.Infinity))
        {
            normal = hit.normal; 
        }

        
        if (Vector3.Angle(normal, Vector3.up) <= maxSlopeAngle)
        {
            DropOffPoint = new Vector3(RandomX, groundHeight, RandomZ);
            Debug.Log($"Сгенерирована подходящая точка сброса: {DropOffPoint}");
            break; 
        }
        else
        {
            Debug.Log("Выбрана не подходящая точка, повторяем.");
        }
    }
}

void MakeVisible()
{
    //Делаем невидимым
        Renderer itemRender = currentTarget.GetComponent<Renderer>();
        if (itemRender != null)
        {
            itemRender.enabled = true;
        } 

        //Отключение коллайдера
        Collider itemCollider = currentTarget.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }
}

/*private IEnumerator StopForDuration(float duration)
    {
        // Stop the agent
        m_agent.isStopped = true;
        m_animator.SetTrigger("Stop");

        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Resume the agent's movement
        m_agent.isStopped = false;
        m_animator.SetTrigger("Resume");
    }*/
}

