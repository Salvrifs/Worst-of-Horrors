using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolBehaviour : StateMachineBehaviour
{

    float timer;
    [SerializeField] private List<Transform> m_Points = new List<Transform>();
    NavMeshAgent m_agent;

    Transform player;
    float chaseRadius = 13f; 

    Transform currentTarget;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        Transform pointsObjects = GameObject.FindGameObjectWithTag("Points").transform;

        foreach (Transform tr in pointsObjects)
        {
            m_Points.Add(tr);
        }

        m_agent = animator.GetComponent<NavMeshAgent>();
        m_agent.SetDestination(m_Points[0].position);
        player = GameObject.FindGameObjectWithTag("Player").transform; 
        
    }

   
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log($"destination: {m_agent.destination.ToString()}, remainingDist: {m_agent.remainingDistance}, StoppingDist: {m_agent.stoppingDistance}");
    
    if (m_agent.isOnNavMesh && m_agent.remainingDistance <= m_agent.stoppingDistance)
    {
        SetNextDestination();
    }

    timer += Time.deltaTime;
    if (timer > 10)
    {
        //Debug.Log("PatrolBeh: Патрулирование закончено");
        animator.SetBool("IsPatrolling", false);
    }

    float distance = Vector3.Distance(animator.transform.position, player.position);
    if (distance < chaseRadius)
    {
        //Debug.Log("PatrolBeh: преследование началось");
        animator.SetBool("IsChasing", true); 
    }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_agent.SetDestination(m_agent.transform.position);
    }


 private void SetNextDestination()
    {
        if (m_Points.Count > 0)
        {
            int RandInd = Random.Range(0, m_Points.Count);
            currentTarget = m_Points[RandInd];
            m_agent.SetDestination(currentTarget.position);
        }
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