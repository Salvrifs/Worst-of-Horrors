using UnityEngine;
using UnityEngine.AI;

public class FlyIdleBehaviour : StateMachineBehaviour
{
    private float m_timer;
    [SerializeField] private float m_waitTime = 3f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_timer = 0;
        animator.GetComponent<NavMeshAgent>().isStopped = true;

        animator.SetBool("IsPreparing", false);
        animator.SetBool("IsFlying", false);
        animator.SetBool("IsLanding", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_timer += Time.deltaTime;

        if (m_timer >= m_waitTime)
            animator.SetBool("IsPreparing", true);
    }
}