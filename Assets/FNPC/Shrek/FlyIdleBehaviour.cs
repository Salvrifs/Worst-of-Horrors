// FlyIdleBehaviour.cs
using UnityEngine;
using UnityEngine.AI;

public class FlyIdleBehaviour : StateMachineBehaviour
{
    private float timer;
    [SerializeField] private float waitTime = 3f;
    private NavMeshAgent agent;
    private NPCReactionController reactionController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        agent = animator.GetComponent<NavMeshAgent>();
        reactionController = animator.GetComponent<NPCReactionController>();
        
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        animator.SetBool("IsPreparing", false);
        animator.SetBool("IsFlying", false);
        animator.SetBool("IsLanding", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
{
    if (reactionController == null || !reactionController.IsReacting)
    {
        timer += Time.deltaTime;
        if (timer >= waitTime)
            animator.SetBool("IsPreparing", true);
    }
}
}