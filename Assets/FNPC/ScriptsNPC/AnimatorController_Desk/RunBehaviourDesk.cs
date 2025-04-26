using UnityEngine;
using UnityEngine.AI;

public class RunBehaviourDesk : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private float timer_esc;    
    [SerializeField] private float safeDistance = 40f;
    [SerializeField] private float speedMultiplier = 1.5f;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer_esc = 0f;

        agent = animator.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        agent.speed *= speedMultiplier;
        agent.isStopped = false;
        UpdateEscapeDestination();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer_esc = 0f;

        timer_esc += Time.deltaTime;

        if (timer_esc >= 20f) 
        {
            animator.SetBool("Run", false);
            animator.SetBool("Patrol", false);
        }

        // Обновляем направление побега
        if (agent.remainingDistance < safeDistance)
        {
            UpdateEscapeDestination();
        }
        
        // Проверка безопасности
        if (Vector3.Distance(agent.transform.position, player.position) > safeDistance * 2)
        {
            animator.SetBool("Run", false);
            animator.SetBool("Patrol", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.speed /= speedMultiplier;
        
    }

    private void UpdateEscapeDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 5f; 
        randomDirection.y = 0;
        Vector3 dirFromPlayer = (agent.transform.position - player.position).normalized + randomDirection;
        Vector3 escapePoint = agent.transform.position + dirFromPlayer.normalized * safeDistance;
        agent.SetDestination(escapePoint);
    }
}