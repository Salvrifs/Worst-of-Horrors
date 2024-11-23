using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class MainBrain : MonoBehaviour
{
    [SerializeField] float Chase_Distance = 30f;
    //[SerializeField] float Attack_Distance = 10f;
    

//Состояния NPC
    public enum NPCState
    {
    Patrolling,
    Chasing,
    }

    private NPCState currentState;
    //Переменные
    //Animator m_animator;
    NavMeshAgent m_agent;
    Transform m_player;
    bool IsChasing;
    float current_distance;

    void Start()
    {
        //m_animator = GetComponent<Animator>();
        InitializeNavMeshAgent();
        InitializePlayer();

        currentState = NPCState.Patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        current_distance = Vector3.Distance(m_player.position, m_agent.transform.position);
        
        switch (currentState)
    {
        case NPCState.Patrolling:
            HandlePatrolling();
            break;
        case NPCState.Chasing:
            HandleChasing();
            break;

    }
}

private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Цвет сферы
        Gizmos.DrawWireSphere(transform.position, Chase_Distance); // Рисуем сферу
    }




    //Инициализация Player
    private void InitializePlayer()
    {
        m_player = GameObject.FindGameObjectWithTag("Player")?.transform;
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

    //Изменение состояния
    private void ChangeState(NPCState NewState)
    {
        currentState = NewState;
    }

    //Для состояния патрулирования
    private void HandlePatrolling()
    {
        if (TryGetComponent<ChasingNPC>(out var ChaseBeh))
    {
        // Преследование на достаточном расстоянии
        if (current_distance <= Chase_Distance)
        {
            ChangeState(NPCState.Chasing);
        }
        else 
        {
            // Проверка наличия PatrollingNPC
            if (TryGetComponent<PatrollingNPC>(out var patrolBeh))
            {
                patrolBeh.StartPatrolling();
            }
            else
            {
                Debug.LogError("PatrollingNPC component is missing!");
            }
        }
    }
    
        else
    {
        // Проверка наличия PatrollingNPC
        if (TryGetComponent<PatrollingNPC>(out var patrolBeh))
        {
            patrolBeh.StartPatrolling();
        }
        else
        {
            Debug.LogError("PatrollingNPC component is missing!");
        }
    }
    }

    //Для состояния преследования
    private void HandleChasing()
    {
        if (TryGetComponent<ChasingNPC>(out var ChaseBeh))
        {
            //ещё не преследуется
            if (!IsChasing)
            {
                if (current_distance <= Chase_Distance)
                {
                    IsChasing = true;
                    ChangeState(NPCState.Chasing);
                    ChaseBeh.StartChasing();
                }
            }

            //уже преследуется
            else
            {
                //Преследование
                if (current_distance <= Chase_Distance)
                {
                    ChaseBeh.StartChasing();
                }
                
                //отмена преследования
                else 
                {
                    IsChasing = false;
                    ChangeState(NPCState.Patrolling);
                }
            }

            
        }
    }




}
