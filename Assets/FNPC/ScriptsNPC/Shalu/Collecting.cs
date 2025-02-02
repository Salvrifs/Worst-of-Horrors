using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public class CollectingNPC : MonoBehaviour
{
    Animator m_animator;
    private NavMeshAgent m_agent;
    private Transform Player;
    private Transform currentTarget = null;
    private Vector3 DropOffPoint;
    private List<Transform> items = new List<Transform>();
    
    private int RandInd;
    private bool IsHolding;
    private bool isSteal;
    private Transform LastItem;
    public Transform quickslotParent;

    float stealDist = 9f;
    

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        InitializeItems();
        MoveToItem();
        
    }

    void Update()
    {
        if (currentTarget == null)
        {
            MoveToItem();
            return; 
        }

        


        if (currentTarget != null)
        {
            float Distance = IsHolding ? Vector3.Distance(m_agent.transform.position, DropOffPoint) 
                                    : Vector3.Distance(m_agent.transform.position, currentTarget.position);

            isSteal = currentTarget.parent == Player;

            if (!IsHolding)
            {
                if (m_agent.stoppingDistance >= Distance)
                {
                    if (isSteal)
                    {
                        StealItemFromPlayer();
                    }

                    else
                    {
                        PickUpItem();
                    }
                    return;
                }

                else
                {
                    m_agent.SetDestination(currentTarget.position);
                    return;
                }
            }


            else
            {
                if (m_agent.stoppingDistance >= Distance)
                {
                    DropItem();
                }

                else
                {
                    m_agent.SetDestination(DropOffPoint);
                }
            }

            return;
        }
        


    }

    private void HandleStealing()
    {
        float distanceToPlayer = Vector3.Distance(Player.position, transform.position);
        if (distanceToPlayer <= stealDist) // Установить дистанцию воровства
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

    //Осуществление кражи
    private void StealItemFromPlayer()
    {

        //Если предмет уже не у игрока
        if (!isSteal || currentTarget.parent != Player || !CanReach(currentTarget.position))
        {

           

            Debug.Log("Random steal");
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
                    slot.amount--;
                    slot.textItemAmount.text = slot.amount.ToString();
                
                    if (slot.amount <= 0)
                    {
                        slot.NullifySlotData();
                    }
                    
                    MakeVisible();
                    
                    PickUpItem();

                    isSteal = false;
                    return;
                }
            }

            else 
            {
                RandInd = Random.Range(0, quickslotParent.childCount);
            }
        }
        }

        //Если предмет все еще у игрока
        else
        {
            Debug.Log($"Steal");
            for (int i = 0; i < quickslotParent.childCount; ++i)
            {
                if (quickslotParent.GetChild(i) == currentTarget)
                {
                    quickslotParent.GetChild(i).GetComponent<InventorySlot>().NullifySlotData();
                    return;
                }
            }
        }
    }

    //Идем к предмету
    private void MoveToItem()
    {
        bool SelectedItem = false;
        foreach (Transform item in items)
        {
            if (!item.GetComponent<Item>().IsTaked && LastItem != item)
            {
                currentTarget = item;
                LastItem = item;
                item.GetComponent<Item>().IsTaked = true;
                isSteal = currentTarget.parent == Player;
                m_agent.SetDestination(currentTarget.position);
                SelectedItem = true;
                m_animator.SetBool("IsWalking", true);
                return; 
            }
        }


        // Если все предметы собраны, или не находятся в зоне действия NPC тогда идем в рандомную точку
        //Если рандомная точка еще не установлена
        if (!SelectedItem)
        {
            Debug.Log($"!SelectedItem: {SelectedItem}");
            GenerateDropOffPoint();
            m_agent.SetDestination(DropOffPoint);
             m_animator.SetBool("IsWalking", true);
        }


    }


    //Уходим от игрока (в Update нужно постоянно проверять если NPC заметит игрока то NPC убегает от него пока не достигнуто нужное расстояние от игрока а затем работает по своему алгоритму)
    private void MoveAwayFromPlayer()
    {
        m_animator.SetBool("IsRunning", true);
        
        
        while (Vector3.Distance(m_agent.transform.position, Player.position) > 7f)
        {
           Vector3 directionAway = (transform.position - Player.position).normalized;
           Vector3 targetPosition = transform.position + directionAway * 1.5f; // Дистанция убегания 
            m_agent.SetDestination(targetPosition);
        }

        m_animator.SetBool("IsRunning", false);
        StartCoroutine(StopRunningAnimation());
    }

    //Подбираем предмет (должен находится сверху над головой NPC)
    private void PickUpItem()
    {

        if (currentTarget != null)
        {
            m_animator.SetBool("IsPickingUpHash", true);

            currentTarget.SetParent(m_agent.transform);
            //m_animator.SetTrigger("DroppingPickUp");
            currentTarget.localPosition = new Vector3(0, m_agent.transform.localScale.y + 5f, 0); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            IsHolding = true;
            Debug.Log($"Подбор предмета: {currentTarget.name}. Позиция агента: {m_agent.transform.position}");

            StartCoroutine(StopPickingUpAnimation());

            if (!isSteal)
                MoveToDrop();
            
            else
            {
                
                MoveAwayFromPlayer();
                
                MoveToDrop();
            }
        }
    }

    //Идем к точке сброса (в этом же методе и генерируется та самая точка)
    void MoveToDrop()
    {
        m_animator.SetBool("IsWalking", true);
        GenerateDropOffPoint(); 
        m_agent.SetDestination(DropOffPoint);
        //m_animator.SetTrigger("GoToTarget");
        Debug.Log($"Движение к точке сброса на позицию {DropOffPoint}");
    }

    //Метод для сброса предмета
    private void DropItem()
    {
        if (currentTarget != null)
        {
            m_animator.SetBool("IsDropping", true);
            currentTarget.SetParent(null);
            currentTarget.position = DropOffPoint;
            IsHolding = false;
            currentTarget.GetComponent<Item>().IsTaked = false;
            isSteal = false;
            currentTarget = null;
            DropOffPoint = default;
            StartCoroutine(StopDroppingAnimation());
            MoveToItem();
        }
    }

    //Метод инициализации предметов
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

    //генерация случайной точки на карте (либо точки сброса в зависимости от ситуации)
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

//Делает предмет видимым (используется если у игрока крадут предмет т.к. предмет невидимый находясь в инвентареpublic)
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

        currentTarget.GetComponent<Rigidbody>().isKinematic = false;
}

//Проверка на то возможно ли для NPC достичь предмет (есть ли он в NavMesh)
private bool CanReach(Vector3 targetPosition)
{
    NavMeshHit hit;
    return NavMesh.SamplePosition(targetPosition, out hit, 1.0f, NavMesh.AllAreas);
}


// Корутине для завершения анимации
private IEnumerator StopPickingUpAnimation()
{
    m_animator.SetBool("IsWalking", false);
    yield return new WaitForSeconds(1.5f); // Замените на длительность анимации
    m_animator.SetBool("IsPickingUpHash", false);
}

// Корутине для завершения анимации
private IEnumerator StopDroppingAnimation()
{
    m_animator.SetBool("IsWalking", false);
    yield return new WaitForSeconds(1.5f); // Замените на длительность анимации
    m_animator.SetBool("IsDropping", false);
}

private IEnumerator StopRunningAnimation()
{
    
    yield return new WaitForSeconds(1.5f);
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

