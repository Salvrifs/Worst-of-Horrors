using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class WalkBehaviourDesk : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private Transform[] waypoints;
    private int currentWaypointIndex;
    List<Transform> m_points = new List<Transform>();
    private float timer;
    [SerializeField] private float detectionAngle = 130f;
    [SerializeField] private float detectionDistance = 75f;
    [SerializeField] private float waypointThreshold = 1f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0f;
        agent = animator.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Инициализация точек патрулирования
       Transform pointsObjects = GameObject.FindGameObjectWithTag("Points").transform;

        foreach (Transform tr in pointsObjects)
        {
            m_points.Add(tr);
        }
        currentWaypointIndex = Random.Range(0, m_points.Count);
        
        agent.isStopped = false;
        SetNextDestination();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        if (timer >= 15f)
        {
            animator.SetBool("Patrol", false);
        }

        // Проверка обнаружения игрока
        if (IsPlayerDetected(animator.transform))
        {
            animator.SetBool("Patrol", false);
            animator.SetBool("Run", true);
            return;
        }

        // Проверка достижения точки
        if (agent.remainingDistance <= waypointThreshold && !agent.pathPending)
        {
            currentWaypointIndex = (currentWaypointIndex) % waypoints.Length;
            SetNextDestination();
        }
    }

    private void SetNextDestination()
    {
        agent.SetDestination(m_points[currentWaypointIndex].position);
    }

    private bool IsPlayerDetected(Transform npcTransform)
    {
        // Аналогичная реализация из IdleBehaviour
        Vector3 dirToPlayer = player.position - npcTransform.position;
        float distance = dirToPlayer.magnitude;
        
        if (distance > detectionDistance) return false;
        
        float angle = Vector3.Angle(npcTransform.forward, dirToPlayer.normalized);
        if (angle > detectionAngle / 2) return false;
        
        if (Physics.Raycast(npcTransform.position, dirToPlayer, distance, LayerMask.GetMask("Obstacle")))
            return false;
            
        return true;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}