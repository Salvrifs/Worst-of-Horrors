using UnityEngine;
using UnityEngine.AI;

public class LandingBehaviour : StateMachineBehaviour
{
    [SerializeField] private float m_landingDuration = 0.5f;
    private float m_timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_timer = 0;
        animator.GetComponent<NavMeshAgent>().isStopped = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_timer += Time.deltaTime;

        if (m_timer >= m_landingDuration)
        {
            animator.SetBool("IsWaiting", true);
            animator.SetBool("IsLanding", false);
        }
    }
}