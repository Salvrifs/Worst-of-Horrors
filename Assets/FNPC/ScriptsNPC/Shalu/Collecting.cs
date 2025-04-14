using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using System.Linq;
using System.Runtime.CompilerServices;

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
        private List<Transform> items = new List<Transform>();

        // Параметры для кражи
        private Transform LastItem;
        private int RandInd;
        private bool IsHolding, isSteal, isRunningAway;
        public Transform quickslotParent;
        
        // Настройки обзора
        [SerializeField] private Transform Eye;
        private float ViewDistance = 75f;
        [Range(0, 360)] private float ViewAngle = 130f;

        [Header("Stealing Settings")]
        [SerializeField] private Transform itemHolderPoint; 
        [SerializeField] private float stealDetectionRange = 20f;
        [SerializeField] private float escapeSpeedMultiplier = 2f;
    
        private GameObject stolenItemInstance;
        private bool isChasingPlayer;
        private float chaseTimer;
        private float baseSpeed;
        


        // Время и звуки
        private float timer = 0f;
        private AudioSource audioSource;
    
    // Existing variables
    [SerializeField] private AudioClip walkSoundS; 
    [SerializeField] private AudioClip pickUpSound; 
    [SerializeField] private AudioClip dropSound; 
    [SerializeField] private AudioClip walkSound; 
    [SerializeField] private AudioClip runSound; 
    [SerializeField] private AudioClip jumpSound;

    private Vector3 temp_size;  

        void Start()
        {
            
            audioSource = GetComponent<AudioSource>();
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            m_agent = GetComponent<NavMeshAgent>();
            m_animator = GetComponent<Animator>();
            InitializeItems();
            baseSpeed = m_agent.speed;
            Item.OnItemCreated += OnItemCreatedHandler;
            InventoryManager.OnDestroyItem += OnItemDestroyedHandler;
            MoveToItem();
        }
        void OnDestroy()
    {
        Item.OnItemCreated -= OnItemCreatedHandler;
        InventoryManager.OnDestroyItem -= OnItemDestroyedHandler;
    }
        void Update()
        {
        
            
            Debug.Log($"{m_agent.name}: {currentTarget.name}");
            

            if (currentTarget == null && DropOffPoint == default)
            {
                MoveToItem();
                
            }

            if (isChasingPlayer)
            {
                chaseTimer += Time.deltaTime;
                float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

                if (chaseTimer >= 10f && distanceToPlayer > stealDetectionRange)
                {
                    DropItemAndEscape();
                }
            }

            if (isRunningAway)
            {
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
            if (IsHolding && currentTarget.parent.CompareTag("Player"))
            {   
                ResetCurrentTarget();
                MoveAwayFromPlayer();
                
                return;
            }

            float Distance = IsHolding 
                ? Vector3.Distance(m_agent.transform.position, DropOffPoint) 
                : Vector3.Distance(m_agent.transform.position, currentTarget.position);

            if (currentTarget != null)
            {
                isSteal = currentTarget.CompareTag("Player");
        
                if (!IsHolding)
                {   
                    HandleItemCollection(Distance);
                }
            }
            
            else
            {
                Debug.Log("Holding");
                HandleItemDropping(Distance);
            }
            InitializeItems();
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
                    items.Add(tr);
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
                if (currentTarget.CompareTag("Player"))
                {
                    Debug.Log("IsSteal");
                    StealItemFromPlayer();
                }
                else
                {
                    Debug.Log("!IsSteal");
                    PickUpItem();
                }
            }
            else if (currentTarget != null)
            {
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
        }

        //
        //Удаление текущей цели
        //
        private void ResetCurrentTarget()
        {
            
            currentTarget = null;
            IsHolding = false;
        }

        //
        //                          УПРАВЛЯЮЩАЯ МЕТОДАМИ
        // Кража предмета
        //
  // В методе StealItemFromPlayer внесем правки:
private void StealItemFromPlayer()
{
    if (IsHolding) return;

    m_animator.SetTrigger("Take");
    CoroutineForTake();

    // Поиск первого подходящего предмета
    InventorySlot targetSlot = quickslotParent.GetComponentsInChildren<InventorySlot>().FirstOrDefault(s => s.amount > 0 && s.is_item != null);

    if (targetSlot != null)
    {
        // Создаем копию предмета
        stolenItemInstance = Instantiate(
            targetSlot.is_item.itemPrefab,
            itemHolderPoint.position,
            Quaternion.identity,
            itemHolderPoint
        );

        // Настройка объекта
        stolenItemInstance.GetComponent<Item>().IsTakedByPlayer = false;
        stolenItemInstance.tag = "Untagged"; 
        targetSlot.amount--;

        // Обновление данных слота
        if (targetSlot.amount <= 0) 
            targetSlot.NullifySlotData();
        else
            targetSlot.textItemAmount.text = targetSlot.amount.ToString();

        // Переход в режим удержания
        IsHolding = true;
        currentTarget = stolenItemInstance.transform;
        GenerateDropOffPoint();
        StartCoroutine(EscapeWithItem());
    }
    else
    {
        MoveToItem();
    }


}


private IEnumerator EscapeWithItem()
{
    m_agent.speed *= 1.5f;
    m_agent.SetDestination(DropOffPoint);
    
    while (Vector3.Distance(transform.position, DropOffPoint) > 1f)
    {
        yield return null;
    }
    
    DropItem();
    m_agent.speed = baseSpeed;
}



// Новая корутина для обработки кражи
private IEnumerator StealProcess(Transform stolenItem)
{
    
    yield return new WaitForSeconds(1.5f);
    
    // Переносим предмет к NPC
    stolenItem.SetParent(itemHolderPoint);
    stolenItem.localPosition = Vector3.zero;
    
    // Настраиваем параметры
    IsHolding = true;
    currentTarget = stolenItem;
    
    // Запускаем побег
    GenerateDropOffPoint();
    m_agent.speed *= 1.5f;
    m_agent.SetDestination(DropOffPoint);
}

        private void OnItemCreatedHandler(Item newItem)
    {
        if (!IsHolding && newItem.IsTakedByPlayer)
        {
            currentTarget = newItem.transform;
            m_agent.SetDestination(currentTarget.position);

        }
    }

    private void OnItemDestroyedHandler(Item destroyedItem)
    {
        temp_size = destroyedItem.transform.localScale;
        if (destroyedItem.IsTakedByPlayer && !IsHolding)
        {
            StartChasingPlayer();
        }

        else if (destroyedItem.IsTakedByPlayer && IsHolding)
        {
            MoveAwayFromPlayer();
            StartCoroutine(ResetAfterEscape());
        }
    }

    private void StartChasingPlayer()
    {
        isChasingPlayer = true;
        chaseTimer = 0f;
        currentTarget = Player;
        m_agent.SetDestination(Player.position);
    }

    private void DropItemAndEscape()
    {
        // Сбрасываем предмет
        if (stolenItemInstance != null)
        {
            Destroy(stolenItemInstance);
            Instantiate(currentTarget.GetComponent<Item>().i_item.itemPrefab, 
                      transform.position, Quaternion.identity);
        }

        // Ускоряемся и убегаем
        m_agent.speed = baseSpeed * escapeSpeedMultiplier;
        MoveAwayFromPlayer();
        StartCoroutine(ResetAfterEscape());
    }

    private IEnumerator ResetAfterEscape()
    {
        yield return new WaitForSeconds(5f);
        m_agent.speed = baseSpeed;
        isChasingPlayer = false;
        MoveToItem();
    }


        //
        //                          
        //
        //
        private void AttemptRandomSteal()
        {
            Debug.Log("AttemptSteal xmmm...");
            int count = 0;
            
            while (true)
            {
                RandInd = Random.Range(0, quickslotParent.childCount);
                count++;

                if (TryStealItemFromSlot(RandInd))
                {
                    
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
        //                          
        //   
        //
        private bool TryStealItemFromSlot(int slotIndex)
        {
            Debug.Log("TryStealItemFromSlot");
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
        //                              
        //
        private void NotifyStealFromPlayer()
        {
            
            Debug.Log("Now Notify so has been Instantiate but where?!");
             IsHolding = true;
            GenerateDropOffPoint(); 
            MoveAwayFromPlayer(); 
        }
        
        //
        // Движене к предмету
        //
        private void MoveToItem()
        {
             m_animator.SetBool("IsWalking", true);
            if (PlayerHasItems())
            {
                currentTarget = Player;
                m_agent.SetDestination(Player.position);
            }

            else {
            
           
            foreach (Transform item in items)
            {
                
                if (!item.GetComponent<Item>().IsTakedByPlayer && LastItem != item && CanReach(item.position))
                {
                    
                    SetCurrentTarget(item);
                    return;
                }
            }
                
                GenerateDropOffPoint();
                MoveToDrop();
            }
        }
        //
        //                          ВСПОМОГАТЕЛЬНАЯ 
        // Установка текущей цели
        //
        private void SetCurrentTarget(Transform item)
        {
            currentTarget = item;
            LastItem = item;
            item.GetComponent<Item>().IsTakedByPlayer = true;
            isSteal = currentTarget.parent == Player;
            m_agent.SetDestination(currentTarget.position);
            
        }
        
        private bool PlayerHasItems()
    {
        foreach (Transform slot in quickslotParent)
        {
            if (slot.GetComponent<InventorySlot>().is_item != null) 
                return true;
        }
        return false;
    }
        //
        //Движение от игрока 
        //
        private void MoveAwayFromPlayer()
        {
                //StopAllCoroutines();
                StartCoroutine(EscapeRoutine());
        }
        //
        //                                  ВСПОМОГАТЕЛЬНАЯ
        //Корутина побега 
        //
        private IEnumerator EscapeRoutine()
    {
        
        isRunningAway = true;
        escapeTimer = 0f;
        float originalSpeed = m_agent.speed; 
        m_agent.speed *= 1.5f; 
        
        

        while (escapeTimer < 10f && Vector3.Distance(Player.position, m_agent.transform.position) < escapeDist)
        {
            // Всегда двигаемся от игрока
            Vector3 directionAway = (transform.position - Player.position).normalized;
            Vector3 targetPosition = transform.position + directionAway * 5f; // Увеличиваем дистанцию
            
            // Ищем валидную позицию на NavMesh
            if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                m_agent.SetDestination(hit.position);
            }
            
            escapeTimer += Time.deltaTime;
            yield return null;
        }

        
        if (IsHolding && escapeTimer >= 10f && Vector3.Distance(Player.position, m_agent.transform.position) < escapeDist)
        {
            ImmediateDropItem();
            m_agent.speed *= 2f; 
            yield return new WaitForSeconds(3f); 

            if (Vector3.Distance(Player.position, m_agent.transform.position) > escapeDist)
            {
                
                MoveToItem();
            }
            else
            {
               
                MoveAwayFromPlayer();
            }

        }
            // Возвращаемся к обычному поведению
            m_agent.speed = originalSpeed;
            isRunningAway = false;
            m_animator.SetBool("IsRunning", false);
            
            if (IsHolding)
            {   
                MoveToDrop();
                //yield break;
            }

            else
            {
              
                MoveToItem();
            }
        
        
        
    }
        //
        //Задание направления от игрока
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
        StartCoroutine(PickUpProcess());
    }
}

private IEnumerator PickUpProcess()
{
    m_animator.SetTrigger("Take");
    //audioSource.PlayOneShot(pickUpSound);
    
    float timer = 0f;
    while (timer < 1.5f)
    {
        timer += Time.deltaTime;
        yield return null; 
    }
    
    currentTarget.SetParent(m_agent.transform);
    currentTarget.localPosition = new Vector3(0, m_agent.transform.localScale.y + 5f, 0);
    currentTarget.GetComponent<Rigidbody>().isKinematic = true;
    IsHolding = true;

    Debug.Log("Choice between Moveaway and MoveToDrop");
    if (!isSteal)
    { 
        Debug.Log("MoveToDrop");
        MoveToDrop();
        
    }

    else
    {
        isSteal = false;
        currentTarget.GetComponent<Item>().IsTakedByPlayer = false;
        Debug.Log("MoveAway"); 
        MoveAwayFromPlayer();
    }
}
        //
        //Движение к дропу
        //
        private void MoveToDrop()
        {
            m_animator.SetBool("IsWalking", true);
            GenerateDropOffPoint();
            m_agent.SetDestination(DropOffPoint);
        }

        //
        //Выброс предмета
        //
        private void DropItem()
{
    StartCoroutine(DropProcess());
}

private IEnumerator DropProcess()
{
    m_animator.SetTrigger("Drop");
    //audioSource.PlayOneShot(dropSound);
    
    float timer = 0f;
    while (timer < 1.5f)
    {
        timer += Time.deltaTime;
        yield return null;
    }
    
    m_animator.SetTrigger("Drop");
    m_animator.SetBool("IsWalkingHold", false);
    m_animator.SetBool("IsWalking", true);


    currentTarget.SetParent(null);
    currentTarget.position = DropOffPoint;
    IsHolding = false;
    currentTarget.GetComponent<Item>().IsTakedByPlayer = false;
    currentTarget = null;
    
    MoveToItem();
}
        //Немедленный выброс предмета под себя
        private void ImmediateDropItem()
        {

            currentTarget.SetParent(null);
            currentTarget.position = m_agent.transform.position;
            IsHolding = false;
            currentTarget.GetComponent<Item>().IsTakedByPlayer = false;
            isSteal = false;
            currentTarget = null;
            DropOffPoint = default;
            
        }
        
        //
        //Генерация позиции
        //
        private void GenerateDropOffPoint()
        {

            Terrain terrain = Terrain.activeTerrain;

            if (terrain == null)
            {
               
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
                   
                    break; 
                }
                else
                {
                   
                }
            }
        }
        //
        //                      ВСПОМОГАТЕЛЬНАЯ к DroppOffPoint
        //  Приемлисмость позиции
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

        
        private IEnumerator StopDroppingAnimation()
        {
            yield return new WaitForSeconds(10.5f);
            m_animator.SetBool("IsDropping", false);
        }

        
        private void UpdateMovementAnimations()
    {
        bool isMoving = m_agent.velocity.magnitude > 0.1f;
        
        // Приоритет анимации бега
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
    }

private IEnumerator CoroutineForTake()
{
    yield return new WaitForSeconds(1.5f);
    m_animator.SetTrigger("Take");
    m_animator.SetBool("IsWalkingHold", true);
}

private IEnumerator CoroutineForDrop()
{
    yield return new WaitForSeconds(1.5f);
    m_animator.SetTrigger("Drop");
    m_animator.SetBool("IsWalkingHold", false);
    m_animator.SetBool("IsWalking", true);
}

}

