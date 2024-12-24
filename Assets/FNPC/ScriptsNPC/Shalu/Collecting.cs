using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class CollectingNPC : MonoBehaviour
{
    Transform currentTarget;
    Vector3 DropOffPoint;
    List<Transform> items = new List<Transform>();
    List<Transform> availableItems = new List<Transform>();
    NavMeshAgent m_agent;
    Animator m_animator;
    int RandInd, RandBehaviour;
    bool IsHolding;
    Transform LastItem;
    Transform Player;
    AudioClip stealSound;
    float fleeDist = 5f;
    float stealDist = 2f;
    GameObject StolenItemPrefab;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        m_animator = GetComponent<Animator>();
        InitializeItems();
        InitializeAgent();
        MoveToItem();
    }

    void Update()
{
    float distanceToPlayer = Vector3.Distance(Player.position, transform.position);

    if (distanceToPlayer <= stealDist)
    {
        TryStealItem();
        return;
    }

    if (distanceToPlayer < fleeDist)
    {
        m_agent.speed = 2f;
    }

    else
    {
        m_agent.speed = 10f;

    }



    if (!IsHolding && currentTarget == null)
    {
        MoveToItem(); 
        return; 
    }
    
   /* if (Random.Range(0, 100) < 10) // 10% chance to stop
    {
            StartCoroutine(StopForDuration(2f)); // Stopping for 2 seconds
            return;
    }*/

    if (currentTarget != null)
    {
        float distance = IsHolding ? Vector3.Distance(m_agent.transform.position, DropOffPoint)
                                   : Vector3.Distance(m_agent.transform.position, currentTarget.position);
        
        if (distance < m_agent.stoppingDistance)
        {
            if (!IsHolding)
            {
                PickUpItem();
            }
            else
            {
                DropItem();
            }
        }
        
    }
    else
    {
        MoveToItem();
    }
}

    void InitializeItems()
    {
        Transform ItemsContainer = GameObject.FindGameObjectWithTag("item")?.transform;
        if (ItemsContainer != null)
        {
            foreach (Transform tr in ItemsContainer)
            {
                Debug.Log($"item: {tr.name}");
                items.Add(tr);
            }
        }
        else
        {
            Debug.LogError("Не найдены предметы");
            return;
        }
    }
    
    void InitializeAgent()
    {
        m_agent = GetComponent<NavMeshAgent>();
        if (m_agent == null)
        {
            Debug.LogError("Инициализация агента не удалась!");
            return;
        }
    }

    void MoveToItem()
    {
        bool foundFreeItem = false;

    foreach (Transform item in items)
    {
        if (!item.GetComponent<Item>().IsTaked && item != LastItem)
        {
            currentTarget = item.transform;
            item.GetComponent<Item>().IsTaked = true;
            LastItem = item;
            m_agent.SetDestination(currentTarget.position);
            foundFreeItem = true;
            return; 
        }
    }

    
    if (!foundFreeItem)
    {
        GenerateDropOffPoint();
        currentTarget = new GameObject("RandomDropOff").transform; 
        currentTarget.position = DropOffPoint; 
        m_agent.SetDestination(DropOffPoint); 
        return; 
    }
        
    }

    void MoveToDrop()
    {
        GenerateDropOffPoint();
        m_agent.SetDestination(DropOffPoint);
        //m_animator.SetTrigger("GoToTarget");
        Debug.Log($"Движение к точке сброса на позицию {DropOffPoint}");
    }
    
    void PickUpItem()
    {
        if (currentTarget != null)
        {
            currentTarget.SetParent(m_agent.transform);
            //m_animator.SetTrigger("DroppingPickUp");
            currentTarget.localPosition = new Vector3(0, m_agent.transform.localScale.y + 5f, 0);
            IsHolding = true;
            Debug.Log($"Подбор предмета: {currentTarget.name}. Позиция агента: {m_agent.transform.position}");
            MoveToDrop(); 
        }
    }

    void DropItem()
    {
        if (currentTarget != null)
        {
            currentTarget.SetParent(null);
            //m_animator.SetTrigger("DroppingPickUp");
            currentTarget.position = DropOffPoint; 
            IsHolding = false; 
            currentTarget.GetComponent<Item>().IsTaked = false;

            
            currentTarget = null;
            MoveToItem(); 
            
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

void TryStealItem()
{
    InventoryManager inventory = Player.GetComponent<InventoryManager>();
    ItemScriptableObject currentTargetItem = currentTarget.GetComponent<Item>().i_item;

    for (int i = 0; i < inventory.slots.Count; ++i)
    {
        InventorySlot slot = inventory.slots[i];
        
        if (!slot.isEmpty && slot.is_item == currentTargetItem)
        {
            //AudioSource.PlayClipAtPoint(stealSound, transform.position);

            GameObject stolenItem = Instantiate(StolenItemPrefab, transform.position + Vector3.up, Quaternion.identity);
            stolenItem.GetComponent<Item>().i_item = slot.is_item;

            slot.NullifySlotData();
            MoveAwayFromPlayer();
        }
    }
}


void MoveAwayFromPlayer()
{
    Vector3 directionAway = (transform.position - Player.position).normalized;
    Vector3 targetPosition = transform.position + directionAway*fleeDist;
    m_agent.SetDestination(targetPosition);
    //StopForDuration(2f);
    MoveToDrop();
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