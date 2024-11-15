using UnityEngine;
using UnityEngine.AI;

public class ChasingNPC : MonoBehaviour
{
    private NavMeshAgent m_agent;
    private Transform m_player;
    private Animator m_animator;

    void Start()
    {
       m_animator = GetComponent<Animator>();
       InitializeAgent();
       InitializePlayer();
    }

    //инициализация игрока
    private void InitializePlayer()
    {
        m_player = GameObject.FindGameObjectWithTag("player")?.transform;  // Находим игрока
        if (m_player == null)
        {
            Debug.LogError("Error: Player doesn't exist in this World!");
        }
    }
    //инициализация агента
    private void InitializeAgent()
    {
        m_agent = GetComponent<NavMeshAgent>();
        if (m_agent == null)
        {
            Debug.LogError("Error: Player hasn't NavMeshAgent component in this World!");
        }
    }
    //начинает преследование
    public void StartChasing()
    {
        Debug.Log("Меня преследуют!");
        
            //isChasing = true;
            //m_agent.ResetPath();
            m_agent.SetDestination(m_player.position);
            //m_animator.SetBool("IsChasing", true);

    }
}
