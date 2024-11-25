using UnityEngine;
using UnityEngine.AI;

public class IdleBehaviour : StateMachineBehaviour
{

    float timer;
    Transform player;
    NavMeshAgent m_agent;
    float chaseRadius = 13f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0f;

        m_agent = animator.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform; 
        //Debug.Log($"Idle: {player.name}"); 
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        if (timer >= 3)
        {
            //Debug.Log("IdleBeh: Патрулирование началось");
            animator.SetBool("IsPatrolling", true);
            
        }
        
        float distance = Vector3.Distance(animator.transform.position, player.position);
        if (distance < chaseRadius)
        {
            //Debug.Log("IdleBeh: погоня началась");
            animator.SetBool("IsChasing", true);
            
        }
        

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
