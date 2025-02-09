using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class CollectingNPC : MonoBehaviour
{
    private Animator m_animator;
    private NavMeshAgent m_agent;
    private Transform Player;
    private Transform currentTarget = null;
    private Vector3 DropOffPoint;
    private List<Transform> items = new List<Transform>();

    // Параметры для кражи
    private Transform LastItem;
    private int RandInd;
    private bool IsHolding;
    private bool isSteal;
    public Transform quickslotParent;
    // Настройки обзора
    [SerializeField] private Transform Eye;
    private float ViewDistance = 75f;
    [Range(0, 360)] private float ViewAngle = 130f;

    // Время и звуки
    private float timer = 0f;
    public AudioSource audioSource; 
    public AudioClip pickUpSound; 
    public AudioClip dropSound; 
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
        if (IsInView())
        {
            MoveAwayFromPlayer();
            return;
        }

        if (currentTarget == null)
        {
            MoveToItem();
            return; 
        }

        // Проверка, принадлежит ли текущая цель игроку
        if (currentTarget.parent == Player)
        {
            ResetCurrentTarget();
            MoveToItem();
            return;
        }

        float Distance = IsHolding 
            ? Vector3.Distance(m_agent.transform.position, DropOffPoint) 
            : Vector3.Distance(m_agent.transform.position, currentTarget.position);

        isSteal = currentTarget.parent == Player;

        if (!IsHolding)
        {
            HandleItemCollection(Distance);
        }
        else
        {
            HandleItemDropping(Distance);
        }
    }

    private void HandleItemCollection(float Distance)
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
        }
        else
        {
            m_agent.SetDestination(currentTarget.position);
        }
    }

    private void HandleItemDropping(float Distance)
    {
        if (currentTarget.parent == Player)
        {
            ResetCurrentTarget();
            MoveToItem();
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

    private void ResetCurrentTarget()
    {
        currentTarget = null;
        IsHolding = false;
    }

    private void StealItemFromPlayer()
    {
        if (!isSteal || currentTarget.parent != Player || !CanReach(currentTarget.position))
        {
            AttemptRandomSteal();
        }
        else
        {
            NotifyStealFromPlayer();
        }
    }

    private void AttemptRandomSteal()
    {
        int count = 0;
        Debug.Log("Random steal");
        
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

    private void NotifyStealFromPlayer()
    {
        Debug.Log("Steal");
        for (int i = 0; i < quickslotParent.childCount; ++i)
        {
            if (quickslotParent.GetChild(i) == currentTarget)
            {
                quickslotParent.GetChild(i).GetComponent<InventorySlot>().NullifySlotData();
                return;
            }
        }
    }

    private void MoveToItem()
    {
        foreach (Transform item in items)
        {
            if (!item.GetComponent<Item>().IsTaked && LastItem != item && CanReach(item.position))
            {
                SetCurrentTarget(item);
                return;
            }
        }

        GenerateDropOffPoint();
        MoveToDropOffPoint();
    }

    private void SetCurrentTarget(Transform item)
    {
        currentTarget = item;
        LastItem = item;
        item.GetComponent<Item>().IsTaked = true;
        isSteal = currentTarget.parent == Player;
        m_agent.SetDestination(currentTarget.position);
        audioSource.PlayOneShot(walkSound);
        m_animator.SetBool("IsWalking", true);
    }

    private void MoveToDropOffPoint()
    {
        m_agent.SetDestination(DropOffPoint);
        audioSource.PlayOneShot(walkSound);
        m_animator.SetBool("IsWalking", true);
    }

    private void MoveAwayFromPlayer()
    {
        m_animator.SetBool("IsRunning", true);
        
        while (Vector3.Distance(m_agent.transform.position, Player.position) > 7f && timer <= 10f)
        {
            MoveAway();
        }

        if (timer > 10f)
        {
            ExecuteEscape();
        }
        
        m_animator.SetBool("IsRunning", false);
        StartCoroutine(StopRunningAnimation());
    }

    private void MoveAway()
    {
        timer += Time.deltaTime;
        Vector3 directionAway = (transform.position - Player.position).normalized * Time.deltaTime;
        Vector3 targetPosition = transform.position + directionAway * 1.5f * Time.deltaTime;
        m_agent.SetDestination(targetPosition);
    }

    private void ExecuteEscape()
    {
        m_agent.speed *= 3f; // Увеличение скорости при побеге
        currentTarget.parent = null;
        currentTarget.position = m_agent.transform.position;
        currentTarget = null;  
        MoveAwayFromPlayer(); // Убегаем с увеличенной скоростью 
    }

    private void PickUpItem()
    {
        if (currentTarget != null)
        {
            m_animator.SetBool("IsPickingUpHash", true);
            audioSource.PlayOneShot(pickUpSound);

            currentTarget.SetParent(m_agent.transform);
            currentTarget.localPosition = new Vector3(0, m_agent.transform.localScale.y + 5f, 0);
            IsHolding = true;

            Debug.Log($"Подбор предмета: {currentTarget.name}. Позиция агента: {m_agent.transform.position}");
            StartCoroutine(StopPickingUpAnimation());

            if (!isSteal)
            {
                MoveToDrop();
            }
            else
            {
                MoveAwayFromPlayer();
                MoveToDrop();
            }
        }
    }

    private void MoveToDrop()
    {
        m_animator.SetBool("IsWalking", true);
        GenerateDropOffPoint();
        m_agent.SetDestination(DropOffPoint);
        Debug.Log($"Движение к точке сброса на позицию {DropOffPoint}");
    }

    private void DropItem()
    {
        if (currentTarget != null)
        {
            m_animator.SetBool("IsDropping", true);
            audioSource.PlayOneShot(dropSound);

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
                Debug.Log($"Сгенерирована подходящая точка сброса: {DropOffPoint}");
                break; 
            }
            else
            {
                Debug.Log("Выбрана не подходящая точка, повторяем.");
            }
        }
    }

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

    private bool CanReach(Vector3 targetPosition)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(targetPosition, out hit, 1.0f, NavMesh.AllAreas);
    }

    private bool IsInView()
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
    }

    private IEnumerator StopPickingUpAnimation()
    {
        yield return new WaitForSeconds(1.5f);
        m_animator.SetBool("IsPickingUpHash", false);
        m_animator.SetBool("IsWalking", false);
    }

    private IEnumerator StopDroppingAnimation()
    {
        yield return new WaitForSeconds(1.5f);
        m_animator.SetBool("IsDropping", false);
    }

    private IEnumerator StopRunningAnimation()
    {
        yield return new WaitForSeconds(1.5f);
        m_animator.SetBool("IsRunning", false);
    }
}
