using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class RunBehaviour : StateMachineBehaviour
{
    NavMeshAgent m_agent;
    Transform player;
    float AttackRadius = 6f;
    float ChasingRadius = 13f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_agent = animator.GetComponent<NavMeshAgent>();
        m_agent.speed *= 2;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        //Debug.Log($"Run: {player.name}");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_agent.SetDestination(player.position);

        float distance = Vector3.Distance(player.position, animator.transform.position);

        if (distance < AttackRadius) 
        {
            //Debug.Log("RunBeh: АТАКУЮТ!!");
            animator.SetBool("IsAttack", true);
            
        }

        if (distance > ChasingRadius)
        {
            //Debug.Log("RunBeh: преследование закончилось");
            animator.SetBool("IsChasing", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_agent.SetDestination(m_agent.transform.position);
        m_agent.speed /= 2;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
        // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
