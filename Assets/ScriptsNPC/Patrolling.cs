using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PatrollingNPC : MonoBehaviour
{
    private Animator m_animator;
    [SerializeField] private float chaseDistance;
    [SerializeField] private float attackRadius;

    private List<Transform> m_patrolPoints = new List<Transform>();
    private NavMeshAgent m_agent;
    private Transform m_player;
    int RandInd = 0;
    private bool isChasing;

    void Start()
    {
        
        m_animator = GetComponent<Animator>();
        InitializePlayer();
        InitializeNavMeshAgent();
        InitializePatrolPoints();
        
    }

    //Инициализация точек
    private void InitializePatrolPoints()
    {
        Transform pointsContainer = GameObject.FindGameObjectWithTag("Points")?.transform;  //находит точки
        //запоминает в список точки
        if (pointsContainer != null)
        {
            foreach (Transform point in pointsContainer)
            {
                m_patrolPoints.Add(point);
            }
        }

        if (m_patrolPoints.Count == 0)
        {
            m_animator.SetBool("IsIdle", true);
            Debug.LogError("Ошибка: нет точек в списке");
            return;
        }
        
        SetRandomDestination();
    }

    //Инициализация Player
    private void InitializePlayer()
    {
        m_player = GameObject.FindGameObjectWithTag("player")?.transform;
        if (m_player == null)
        {
            Debug.LogError("Error: Player doesn't exist in this World!");
            return;
        }
    }

    //инициализация NavMeshAgent
    private void InitializeNavMeshAgent()
    {
        m_agent = GetComponent<NavMeshAgent>();
        if (m_agent == null)
        {
            Debug.LogError("Error: Player hasn't NavMeshAgent component in this World!");
            return;
        }
    }

    //установка направления NPC
    private void SetRandomDestination()
    {
        if (m_patrolPoints.Count > 0)
        {
            RandInd = Random.Range(0, m_patrolPoints.Count);
            //Debug.Log($"RandInd: {RandInd}\nname: {m_patrolPoints[RandInd].name}\nposition: {m_patrolPoints[RandInd].position}");

            m_agent.SetDestination(m_patrolPoints[RandInd].position);
            //Debug.Log($"{transform.name} Идёт к точке {m_patrolPoints[RandInd].name} на позиции {m_patrolPoints[RandInd].position}");   
        }
    }

    /// <summary>
    /// Патрулирование (Main function)
    /// </summary>
    public void StartPatrolling()
    {
        if (m_agent == null || m_patrolPoints.Count == 0 || m_player == null)
    {
        return; 
    }
        if (m_agent.remainingDistance <= m_agent.stoppingDistance && !isChasing)
        {
            SetRandomDestination(); 
        }
 
    }

    // Возврат к патрулированию (для других скриптов)
    public void ReturnToPatrolling()
    {
        isChasing = false;
        //m_animator.SetBool("IsChasing", false);
        //m_animator.SetBool("IsPatrolling", true);
        SetRandomDestination(); // Возврат к новой цели патрулирования
    }
}
