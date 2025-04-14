using UnityEngine;
using UnityEngine.AI;

public class IdleDeskBehaviour : StateMachineBehaviour
{
    private float timer;
    
    private Transform player;
    private NavMeshAgent agent;
    
    [SerializeField] private float detectionAngle = 130f;
    [SerializeField] private float detectionDistance = 75f;
    [SerializeField] private float idleDuration = 3f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        agent = animator.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.isStopped = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        
        // Проверка обнаружения игрока
        if (IsPlayerDetected(animator.transform))
        {
            animator.SetBool("Run", true);
            return;
        }

        // Переход в патрулирование после ожидания
        if (timer >= idleDuration)
        {
            animator.SetBool("Patrol", true);
        }
    }

    private bool IsPlayerDetected(Transform npcTransform)
    {
        Vector3 dirToPlayer = player.position - npcTransform.position;
        float distance = dirToPlayer.magnitude;
        
        if (distance > detectionDistance) return false;
        
        float angle = Vector3.Angle(npcTransform.forward, dirToPlayer.normalized);
        if (angle > detectionAngle / 2) return false;
        
        if (Physics.Raycast(npcTransform.position, dirToPlayer, distance, LayerMask.GetMask("Obstacle")))
            return false;
            
        return true;
    }
}