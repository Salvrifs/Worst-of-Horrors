using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingBehaviour : StateMachineBehaviour
{
    private NavMeshAgent m_agent;
    private List<Transform> m_points;
    private Transform m_currentTarget;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_agent = animator.GetComponent<NavMeshAgent>();
        m_agent.baseOffset = 2f;
        m_agent.isStopped = false;

        if (m_points == null)
        {
            m_points = new List<Transform>();
            Transform pointsParent = GameObject.FindGameObjectWithTag("FlyPoints").transform;
            foreach (Transform tr in pointsParent)
                m_points.Add(tr);
        }

        SetRandomDestination();
    }

override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
{
    var reactionController = animator.GetComponent<NPCReactionController>();
    if (reactionController != null && reactionController.IsReacting) return;

    // Добавляем повторный выбор точки при застревании
    if (m_agent.remainingDistance <= m_agent.stoppingDistance + 0.1f)
    {
        SetRandomDestination();
    }
}

    private void SetRandomDestination()
    {
        if (m_points.Count > 0)
        {
            int randomIndex = Random.Range(0, m_points.Count);
            m_currentTarget = m_points[randomIndex];
            m_agent.SetDestination(m_currentTarget.position);
        }
    }
}