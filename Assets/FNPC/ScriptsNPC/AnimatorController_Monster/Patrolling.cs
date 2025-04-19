using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolBehaviour : StateMachineBehaviour
{

    float timer;
    [SerializeField] private List<Transform> m_Points = new List<Transform>();
    NavMeshAgent m_agent;

    Transform m_player;
    [Range(0, 360)] float ViewAngle = 130f;
    float ViewDistance = 75f; 
    Transform EnemyEye;
    Transform currentTarget;
    //float ChaseDist = 5f;
    [SerializeField] AudioSource intimidate_audioSource;
    [SerializeField] AudioClip[] intimidate_audioClips;

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
        m_player = GameObject.FindGameObjectWithTag("Player").transform; 
        EnemyEye = GameObject.FindGameObjectWithTag("Eye").transform;
        intimidate_audioSource = animator.GetComponent<AudioSource>();

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

    //float distance = Vector3.Distance(m_agent.transform.position, m_player.transform.position);

    if (IsInView())
    {
            //Debug.Log("PatrolBeh: преследование началось");
            animator.SetBool("IsChasing", true); 
            animator.SetBool("IsPatrolling", false);
    }

    
}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_agent.SetDestination(m_agent.transform.position);
        
    }


//Установка направления до следующей точке
 private void SetNextDestination()
    {
        if (m_Points.Count > 0)
        {
            int RandInd = Random.Range(0, m_Points.Count);
            currentTarget = m_Points[RandInd];
            m_agent.SetDestination(currentTarget.position);
        }
    }  

//Находится ли в поле зрения
private bool IsInView()
    {
        float realAngle = Vector3.Angle(EnemyEye.forward, m_player.position - EnemyEye.position);
        RaycastHit hit;
        if (Physics.Raycast(EnemyEye.transform.position, m_player.position - EnemyEye.position, out hit, ViewDistance))
        {
            if (realAngle < ViewAngle / 2f && Vector3.Distance(EnemyEye.position, m_player.position) <= ViewDistance && hit.transform == m_player.transform)
            {
                return true;
            }
        }
        return false;
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
