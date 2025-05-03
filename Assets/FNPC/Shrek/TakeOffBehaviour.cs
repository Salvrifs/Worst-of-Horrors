using UnityEngine;
using UnityEngine.AI;

public class TakeOffBehaviour : StateMachineBehaviour
{
    [SerializeField] private float m_takeOffDuration = 0.5f;
    private float m_timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_timer = 0;
        animator.GetComponent<NavMeshAgent>().isStopped = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_timer += Time.deltaTime;

        if (m_timer >= m_takeOffDuration)
            animator.SetBool("IsFlying", true);
    }
}