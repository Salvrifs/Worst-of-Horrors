using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using Unity.VisualScripting;
using UnityEngine.XR;

public class CollectingNPC : MonoBehaviour
{
    //Фундамент
    private Animator m_animator;
    private NavMeshAgent m_agent;
    private Transform Player;
    private Coroutine escapeCoroutine;
    private Transform currentTarget = null;
    private float escapeTimer;
    private float escapeDist = 15f;
    private Vector3 DropOffPoint;
    //private List<Transform> items = new List<Transform>();
    private HashSet<Transform> active_items = new HashSet<Transform>();

    // Параметры для кражи
    private Transform LastItem;
    private int RandInd;
    private bool IsHolding, isSteal, isRunningAway;
    public Transform quickslotParent;
    
    // Настройки обзора
    [SerializeField] private Transform Eye;
    private float ViewDistance = 75f;
    [Range(0, 360)] private float ViewAngle = 130f;

    // Время и звуки
    private float timer = 0f;
    public AudioSource walkSoundS; 
    public AudioSource pickUpSound; 
    public AudioSource dropSound; 
    public AudioSource walkSound; 
    public AudioSource runSound; 
    //public AudioSource jumpSound; 

    [SerializeField] float escapeDuration = 15f;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        InitializeItems();
        MoveToItem();

        Item.OnItemCreated += HandleItemCreating;
        InventoryManager.OnDestroyItem += HandleItemDestroy;
        QuickSlotPanel.OnItemUsed += HandleItemUsed;
    }

    void Update()
    {
       /* if (IsInView())
        {
            MoveAwayFromPlayer();
            return;
        }*/
        
        Debug.Log($"{m_agent.name}: {currentTarget.name}");

        if (currentTarget == null && DropOffPoint == default)
        {
            //Debug.Log("Both things don't exist");
            MoveToItem();
             
        }

        if (isRunningAway)
        {
            //Debug.Log($"{m_agent.name}: in isRunningAway {currentTarget.name}");
            escapeTimer += Time.deltaTime;
            if (escapeTimer >= 10f)
            {
                ExecuteEscape();
                StopCoroutine(escapeCoroutine);
                isRunningAway = false;
            }
            return;
        }

        // Проверка, принадлежит ли текущая цель игроку
        /*if (IsHolding && currentTarget.parent.CompareTag("Player"))
        {
            //m_animator.SetTrigger("AAA,MAN!!!");
            //Звук
            //Debug.Log("it don't be in this way");
            ResetCurrentTarget();
            MoveAwayFromPlayer();
            //MoveToItem();
            return;
        }*/

        float Distance = IsHolding 
            ? Vector3.Distance(m_agent.transform.position, DropOffPoint) 
            : Vector3.Distance(m_agent.transform.position, currentTarget.position);

        isSteal = currentTarget.GetComponent<Item>().i_item.IsTakedByPlayer;

        if (!IsHolding)
        {
            //Debug.Log($"{m_agent.name}: Handle !Holding");
            HandleItemCollection(Distance);
        }
        else
        {
            //Debug.Log($"{m_agent.name}: Handle Holding");
            HandleItemDropping(Distance);
        }
        Debug.Log($"currentTarget: {currentTarget.name}");
        m_agent.SetDestination(currentTarget.position);
    }
    void FixedUpdate()
    {
        // Обновление анимаций только если не в процессе кражи
        if (!IsHolding || !isRunningAway)
        {
            UpdateMovementAnimations();
        }
    }
    //
    //Инициализация предметов
    //
    private void InitializeItems()
    {
        Transform ItemsContainer = GameObject.FindGameObjectWithTag("item")?.transform;
        if (ItemsContainer != null)
        {
            
            foreach (Transform tr in ItemsContainer)
            {
                //Debug.Log(tr.name);
                active_items.Add(tr);
            }
        }
    }

    //
    //                          ОСНОВНАЯ
    //Работа с подбором предметов
    //
    private void HandleItemCollection(float Distance)
    {
        if (m_agent.stoppingDistance >= Distance)
        {
            if (isSteal)
            {
                Debug.Log($"{m_agent.name}: Norm dist steal for colle");
                StealItemFromPlayer();
            }
            else
            {
                Debug.Log($"{m_agent.name}: Norm dist not steal for colle");
                PickUpItem();
            }
        }
        else
        {
            //Debug.Log($"{m_agent.name}: Distance more than stoppingDist for colle");
            m_agent.SetDestination(currentTarget.position);
        }
    }

    //
    //                      ОСНОВНАЯ
    //Работа со сбросом
    //
    private void HandleItemDropping(float Distance)
    {
        if (currentTarget.parent == Player)
        {
            ResetCurrentTarget();
            MoveAwayFromPlayer();
            //MoveToItem();
        }
        else
        {
            if (m_agent.stoppingDistance >= Distance)
            {
                //Debug.Log($"{m_agent.name}: Norm dist for drop");
                DropItem();
            }
            else
            {
                //Debug.Log($"{m_agent.name}: Not Norm dist for drop");
                m_agent.SetDestination(DropOffPoint);
            }
        }
    }

    //
    //Удаление текущей цели
    //
    private void ResetCurrentTarget()
    {
        //m_animator.SetTrigger("Jump!");
        //
        //Debug.Log($"{m_agent.name} Reset");
        currentTarget = null;
        IsHolding = false;
    }

    //
    //                          УПРАВЛЯЮЩАЯ МЕТОДАМИ
    // Кража предмета
    //
    private void StealItemFromPlayer()
    {
        if (!isSteal  || !CanReach(currentTarget.position) || currentTarget.parent == Player)
        {
            AttemptRandomSteal();
            m_animator.SetBool("IsRunning", true);
            MoveAwayFromPlayer();
        }

        else if (isSteal && currentTarget.parent.CompareTag("Player"))
        {
            NotifyStealFromPlayer();
            m_animator.SetTrigger("IsPicking");
            MoveAwayFromPlayer();
        }

    }
    //
    //                          ВСПОМОГАТЕЛЬНАЯ к StealItemFromPlayer
    //Кража случайного предмета
    //
    private void AttemptRandomSteal()
    {
        int count = 0;
        //Debug.Log($"{m_agent.name}: Random steal");
        
        while (true)
        {
            RandInd = Random.Range(0, quickslotParent.childCount);
            count++;

            if (TryStealItemFromSlot(RandInd))
            {
                //MakeVisible();
                PickUpItem();
                
                isSteal = false;
                return;
            }

            if (count >= 3)
            {
                MoveToItem();
                return;
            }
        }
    }
    //
    //                          ПРОВЕРКА к AttemptStealItem
    //   Можно ли украсть предмет
    //
    private bool TryStealItemFromSlot(int slotIndex)
    {
        InventorySlot slot = quickslotParent.GetChild(slotIndex)?.GetComponent<InventorySlot>();
        if (slot != null && slot.is_item != null && slot.amount > 0)
        {
            slot.amount--;
            slot.textItemAmount.text = slot.amount.ToString();
            if (slot.amount <= 0)
            {
                slot.NullifySlotData();
            }
            
            return true;
        }
        return false;
    }
    //
    //                              ВСПОМГАТЕЛЬНАЯ
    //"Обнуление"/Очистка инвентаря
    //
    private void NotifyStealFromPlayer()
    {
        //Debug.Log($"{m_agent.name}: Steal");
        MakeVisible();
        PickUpItem();
        for (int i = 0; i < quickslotParent.childCount; ++i)
        {
            if (quickslotParent.GetChild(i).name == currentTarget.name)
            {
                currentTarget.SetParent(m_agent.transform);
                quickslotParent.GetChild(i).GetComponent<InventorySlot>().NullifySlotData();
                return;
            }
        }
        //Если своего предмета уже не нашёл взять что-то рандомное
        //Debug.Log("не нашёл своего беру на рандом");
        AttemptRandomSteal();
    }
    //
    //Чек Есть ли предмет в инвентаре
    //

    //
    // Движене к предмету
    //
    private void MoveToItem()
    {
        //Debug.Log($"{m_agent.name}: WENT TO THE mOVEtOiTEM");
        foreach (Transform item in active_items)
        {
            //Debug.Log($"{m_agent.name}: {item}");
            if (!item.GetComponent<Item>().IsTaked && LastItem != item && CanReach(item.position))
            {
                //Debug.Log($"{m_agent.name}: O kak!");
                SetCurrentTarget(item);
                return;
            }
        }
        //Debug.Log($"{m_agent.name}: ne ponyal");
        GenerateDropOffPoint();
        MoveToDrop();
    }
    //
    //                          ВСПОМОГАТЕЛЬНАЯ 
    // Установка текущей цели
    //
    private void SetCurrentTarget(Transform item)
    {
        currentTarget = item;
        LastItem = item;
        item.GetComponent<Item>().IsTaked = true;
        isSteal = currentTarget.parent == Player;
        m_agent.SetDestination(currentTarget.position);
        //audioSource.PlayOneShot(walkSound);
        m_animator.SetBool("IsWalking", true);
    }
    

    //
    //Движение от игрока 
    //
    private void MoveAwayFromPlayer()
    {
            //StopAllCoroutines();
            StartCoroutine(EscapeBehavior());
    }
    //
    //                                  ВСПОМОГАТЕЛЬНАЯ
    //Корутина побега 
   private IEnumerator EscapeBehavior()
{
    isRunningAway = true;
    m_animator.SetBool(IsHolding ? "IsRunningHold" : "IsRunning", true);

    float timer = 0f;
    while (timer < escapeDuration)
    {
        Vector3 escapeDirection = (transform.position - Player.position).normalized;
        Vector3 targetPosition = transform.position + escapeDirection * 10f;
        
        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            m_agent.SetDestination(hit.position);
        }
        
        timer += Time.deltaTime;
        yield return null;
    }
    
    m_animator.SetBool("IsRunning", false);
    m_animator.SetBool("IsRunningHold", false);
    isRunningAway = false;
    
    if (!IsHolding) MoveToItem();
}
    //
    //                                  ВСПОМОГАТЕЛЬНАЯ
    //Выброс предмета/Завершение побега от игрока
    //
    private void ExecuteEscape()
    {
        currentTarget.parent = null;
        currentTarget.position = m_agent.transform.position;
        m_agent.speed *= 3f;
        currentTarget = null;  
        MoveAwayFromPlayer(); 
    }

    //
    //Подбор предмета
    //
    private void PickUpItem()
    {
        if (currentTarget != null)
        {
            StartCoroutine(PickUpAnimationRoutine());
        }
    }
    //
    //Корутина
    //
   private IEnumerator PickUpAnimationRoutine()
{
    m_animator.SetTrigger("IsPicking");
    m_animator.SetBool("IsWalking", false);
    m_animator.SetBool("IsRunning", false);

    yield return new WaitForSeconds(1.2f);
    
    currentTarget.SetParent(m_agent.transform);
    currentTarget.localPosition = new Vector3(0, m_agent.transform.localScale.y + 5f, 0);
    currentTarget.GetComponent<Rigidbody>().isKinematic = true;
    
    IsHolding = true;
    m_animator.ResetTrigger("IsPicking");

    if (!isSteal)
    {
        MoveToDrop();
    }
    else
    {
        MoveAwayFromPlayer();
    }
}

    //
    //Движение к дропу
    //
    private void MoveToDrop()
{
    GenerateDropOffPoint();
    m_agent.SetDestination(DropOffPoint);
    m_animator.SetBool(IsHolding ? "IsWalkingHold" : "IsWalking", true);
}

    //
    //Выброс предмета
    //
    private void DropItem()
{
    if (currentTarget != null)
    {
        StartCoroutine(DropAnimationRoutine());
    }
}
private IEnumerator DropAnimationRoutine()
{
    m_animator.SetTrigger("IsDropping");
    
    yield return new WaitForSeconds(0.8f);
    
    currentTarget.SetParent(null);
    currentTarget.position = DropOffPoint;
    currentTarget.GetComponent<Item>().IsTaked = false;
    
    IsHolding = false;
    isSteal = false;
    currentTarget = null;
    DropOffPoint = default;
    
    m_animator.ResetTrigger("IsDropping");
    MoveToItem();
}
    //Немедленный выброс предмета под себя
    private void ImmediateDropItem()
    {
        //m_animator.SetBool("IsDropping", true);
        //audioSource.PlayOneShot(dropSound);

        currentTarget.SetParent(null);
        currentTarget.position = m_agent.transform.position;
        IsHolding = false;
        currentTarget.GetComponent<Item>().IsTaked = false;
        isSteal = false;
        currentTarget = null;
        DropOffPoint = default;
        //StartCoroutine(StopDroppingAnimation());
        //MoveAwayFromPlayer();
        
    }
    
    //
    //Генерация позиции
    //
    private void GenerateDropOffPoint()
    {

        Terrain terrain = Terrain.activeTerrain;

        if (terrain == null)
        {
            //Debug.LogError("Террайн не найден");
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

            if (IsSlopeAcceptable(RandomX, groundHeight, RandomZ, maxSlopeAngle))
            {
                DropOffPoint = new Vector3(RandomX, groundHeight, RandomZ);
                //Debug.Log($"{m_agent.name}: Сгенерирована подходящая точка сброса: {DropOffPoint}");
                break; 
            }
            else
            {
                //Debug.Log($"{m_agent.name}: Выбрана не подходящая точка, повторяем.");
            }
        }
    }
    //
    //                      ВСПОМОГАТЕЛЬНАЯ к DroppOffPoint
    //Приемлисмость позиции
    //
    private bool IsSlopeAcceptable(float RandomX, float groundHeight, float RandomZ, float maxSlopeAngle)
    {
        Vector3 normal = Vector3.up; 
        RaycastHit hit;

        if (Physics.Raycast(new Vector3(RandomX, groundHeight + 100, RandomZ), Vector3.down, out hit, Mathf.Infinity))
        {
            normal = hit.normal; 
        }

        return Vector3.Angle(normal, Vector3.up) <= maxSlopeAngle;
    }

    //
    //Обработчик создания предмета 
    //
    private void HandleItemCreating(Item new_item)
    {
        if (!active_items.Contains(new_item.transform))
        {
            active_items.Add(new_item.transform);
        }

        if (!IsHolding && isSteal && currentTarget.GetComponent<Item>().i_item.itemName == new_item.i_item.itemName)
        {
            currentTarget = new_item.transform;
            m_agent.SetDestination(currentTarget.position);
        }


    }

    //
    //Обработчик использования предмета
    //
    private void HandleItemUsed(ItemScriptableObject used_item)
    {
        if (active_items.Contains(used_item.itemPrefab.transform))
        {
            active_items.Remove(used_item.itemPrefab.transform);
        }

        if (!isRunningAway && 
            currentTarget.GetComponent<Item>().i_item.itemName == used_item.itemName &&
            Vector3.Distance(Player.transform.position, m_agent.transform.position) < 40f)
        {
            m_animator.SetTrigger("IsPicking");
            //AudioSource.Play("AAA");
            //yield return new WaitForSeconds(1.6f);

            StartCoroutine(EscapeBehavior());
        }

        else
        {
            m_agent.SetDestination(Player.position);
        }
    }
    //
    //Управление разрушением предмета
    //
    private void HandleItemDestroy(Item Destroyed_item)
    {
        if (currentTarget.name == Destroyed_item.transform.name)
        {
            currentTarget = Player;
            isSteal = true;
            m_agent.SetDestination(Player.position);
        }
        else
        {
            m_agent.SetDestination(currentTarget.position);
        }
    }

    //
    //Уничтожение предмета
    //
    void OnDestroy()
    {
        Item.OnItemCreated -= HandleItemCreating;
        QuickSlotPanel.OnItemUsed -= HandleItemUsed;
    }







    //
    //Сделать Видимым
    //
    private void MakeVisible()
    {
        Renderer itemRender = currentTarget.GetComponent<Renderer>();
        if (itemRender != null)
        {
            itemRender.enabled = true;
        }

        Collider itemCollider = currentTarget.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }

        currentTarget.GetComponent<Rigidbody>().isKinematic = false;
    }

    //
    //Возможно ли достичь объекта NPC
    //
    private bool CanReach(Vector3 targetPosition)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(targetPosition, out hit, 100.0f, NavMesh.AllAreas);
    }

    //
    //Находится ли в поле зрения
    //
    /*private bool IsInView()
    {
        float realAngle = Vector3.Angle(Eye.forward, Player.position - Eye.position);
        RaycastHit hit;
        
        if (Physics.Raycast(Eye.position, Player.position - Eye.position, out hit, ViewDistance))
        {
            if (realAngle < ViewAngle / 2f && Vector3.Distance(Eye.position, Player.position) <= ViewDistance && hit.transform == Player.transform)
            {
                return true;
            }
        }
        return false;
    }*/

    //
    //КОРУТИНЫ
    //
    /*private IEnumerator StopPickingUpAnimation()
    {
        yield return new WaitForSeconds(1.5f);
        m_animator.SetBool("IsPickingUpHash", false);
        //m_animator.SetBool("IsWalking", false);
    }*/

    private IEnumerator StopDroppingAnimation()
    {
        yield return new WaitForSeconds(10.5f);
        m_animator.SetBool("IsDropping", false);
    }

    
    private void UpdateMovementAnimations()
{
    bool isMoving = m_agent.velocity.magnitude > 0.1f;

    if (IsHolding)
    {
        if (isRunningAway)
        {
            m_animator.SetBool("IsRunningHold", isMoving);
            m_animator.SetBool("IsWalkingHold", false);
        }
        else
        {
            m_animator.SetBool("IsWalkingHold", isMoving);
            m_animator.SetBool("IsRunningHold", false);
        }
        m_animator.SetBool("IsRunning", false);
        m_animator.SetBool("IsWalking", false);
    }
    else
    {
        if (isRunningAway)
        {
            m_animator.SetBool("IsRunning", isMoving);
            m_animator.SetBool("IsWalking", false);
        }
        else
        {
            m_animator.SetBool("IsWalking", isMoving);
            m_animator.SetBool("IsRunning", false);
        }
        m_animator.SetBool("IsRunningHold", false);
        m_animator.SetBool("IsWalkingHold", false);
    }
}


}