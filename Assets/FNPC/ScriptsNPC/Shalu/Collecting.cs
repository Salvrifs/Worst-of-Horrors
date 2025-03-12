using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

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

    // Время и звуки
    private float timer = 0f;
    public AudioSource walkSoundS; 
    //public AudioClip pickUpSound; 
    //public AudioClip dropSound; 
    public AudioClip walkSound; 
    public AudioClip runSound; 
    public AudioClip jumpSound; 

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
       /* if (IsInView())
        {
            MoveAwayFromPlayer();
            return;
        }*/
        Debug.Log($"{m_agent.name}: {currentTarget.name}");

        if (currentTarget == null && DropOffPoint == default)
        {
            Debug.Log("Both things don't exist");
            MoveToItem();
             
        }

        if (isRunningAway)
        {
            Debug.Log($"{m_agent.name}: in isRunningAway {currentTarget.name}");
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
            //m_animator.SetTrigger("AAA,MAN!!!");
            //Звук
            Debug.Log("it don't be in this way");
            ResetCurrentTarget();
            MoveAwayFromPlayer();
            //MoveToItem();
            return;
        }

        float Distance = IsHolding 
            ? Vector3.Distance(m_agent.transform.position, DropOffPoint) 
            : Vector3.Distance(m_agent.transform.position, currentTarget.position);

        isSteal = currentTarget.parent == Player;

        if (!IsHolding)
        {
            Debug.Log($"{m_agent.name}: Handle !Holding");
            HandleItemCollection(Distance);
        }
        else
        {
            Debug.Log($"{m_agent.name}: Handle Holding");
            HandleItemDropping(Distance);
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
                Debug.Log(tr.name);
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
            Debug.Log($"{m_agent.name}: Distance more than stoppingDist for colle");
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
                Debug.Log($"{m_agent.name}: Norm dist for drop");
                DropItem();
            }
            else
            {
                Debug.Log($"{m_agent.name}: Not Norm dist for drop");
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
        Debug.Log($"{m_agent.name} Reset");
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
            //m_animator.SetBool("AAA,MAN!");
            MoveAwayFromPlayer();
        }

        else if (isSteal && currentTarget.parent == Player)
        {
            NotifyStealFromPlayer();
            //m_animator.SetTrigger("AAA,MAN!");
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
        Debug.Log($"{m_agent.name}: Random steal");
        
        while (true)
        {
            RandInd = Random.Range(0, quickslotParent.childCount);
            count++;

            if (TryStealItemFromSlot(RandInd))
            {
                MakeVisible();
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
        Debug.Log($"{m_agent.name}: Steal");
        MakeVisible();
        PickUpItem();
        for (int i = 0; i < quickslotParent.childCount; ++i)
        {
            if (quickslotParent.GetChild(i) == currentTarget)
            {
                currentTarget.SetParent(m_agent.transform);
                quickslotParent.GetChild(i).GetComponent<InventorySlot>().NullifySlotData();
                return;
            }
        }
        //Если своего предмета уже не нашёл взять что-то рандомное
        //AttemptRandomSteal();
    }
    
    //
    // Движене к предмету
    //
    private void MoveToItem()
    {
        //Debug.Log($"{m_agent.name}: WENT TO THE mOVEtOiTEM");
        foreach (Transform item in items)
        {
            Debug.Log($"{m_agent.name}: {item}");
            if (!item.GetComponent<Item>().IsTaked && LastItem != item)
            {
                Debug.Log($"{m_agent.name}: O kak!");
                SetCurrentTarget(item);
                return;
            }
        }
        Debug.Log($"{m_agent.name}: ne ponyal");
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
    //                          ВСПОМОГАТЕЛЬНАЯ 
    //Есть ли путь
    //
    private bool IsPathPossible(Vector3 targetPosition)
{
    NavMeshPath path = new NavMeshPath();
    m_agent.CalculatePath(targetPosition, path);
    return path.status == NavMeshPathStatus.PathComplete;
}
    //
    //Движение к точке сброса
    //
    private void MoveToDropOffPoint()
    {
        m_animator.SetBool("IsWalking", true);
        m_agent.SetDestination(DropOffPoint);
        //audioSource.PlayOneShot(walkSound);
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
    private IEnumerator EscapeRoutine()
{
    
    isRunningAway = true;
    escapeTimer = 0f;
    float originalSpeed = m_agent.speed; 
    m_agent.speed *= 1.5f; 
    //audioSource.PlayOneShot(runSound);
    

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
            Debug.Log($"{m_agent.name}: ImmediateDrop: If");
            MoveToItem();
        }
        else
        {
            Debug.Log($"{m_agent.name}: ImmediateDrop: else");
            MoveAwayFromPlayer();
        }

    }
        // Возвращаемся к обычному поведению
        m_agent.speed = originalSpeed;
        isRunningAway = false;
        m_animator.SetBool("IsRunning", false);
        
        if (IsHolding)
        {
            Debug.Log($"{m_agent.name}: EscapeRoutine: If");
            //GenerateDropOffPoint();
            //m_agent.SetDestination(DropOffPoint);
            MoveToDrop();
            //yield break;
        }

        else
        {
            Debug.Log($"{m_agent.name}: PickUpRoutine: else");
            MoveToItem();
        }
    
    
    
}
    //Задание направления от игрока
    //
    /*private void MoveAway()
    {
        
        Vector3 directionAway = (transform.position - Player.position).normalized;
        Vector3 targetPosition = transform.position + directionAway * 1.5f;
        m_agent.SetDestination(targetPosition);
    }*/
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
        m_animator.SetBool("IsPickingUpHash", true);
        yield return new WaitForSeconds(1f); 
        
        currentTarget.SetParent(m_agent.transform);
        currentTarget.localPosition = new Vector3(0, m_agent.transform.localScale.y + 5f, 0);
        currentTarget.GetComponent<Rigidbody>().isKinematic = true;
        
        IsHolding = true;
        m_animator.SetBool("IsPickingUpHash", false);

        if (!isSteal)
        {
            Debug.Log($"{m_agent.name}: PickUpRoutine: If");
            MoveToDrop();
        }
        
        else
        {
            Debug.Log($"{m_agent.name}: PickUpRoutine: else");
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
        Debug.Log($"{m_agent.name}: Движение к точке сброса на позицию {DropOffPoint}");
    }

    //
    //Выброс предмета
    //
    private void DropItem()
    {
        if (currentTarget != null)
        {
            m_animator.SetBool("IsDropping", true);
            //audioSource.PlayOneShot(dropSound);

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

            if (IsSlopeAcceptable(RandomX, groundHeight, RandomZ, maxSlopeAngle))
            {
                DropOffPoint = new Vector3(RandomX, groundHeight, RandomZ);
                Debug.Log($"{m_agent.name}: Сгенерирована подходящая точка сброса: {DropOffPoint}");
                break; 
            }
            else
            {
                Debug.Log($"{m_agent.name}: Выбрана не подходящая точка, повторяем.");
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
        return NavMesh.SamplePosition(targetPosition, out hit, 1.0f, NavMesh.AllAreas);
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

void FixedUpdate()
{
    // Обновление анимаций только если не в процессе кражи
    if (!IsHolding || !isRunningAway)
    {
        UpdateMovementAnimations();
    }
}
}