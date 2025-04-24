using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class RunBehaviour : StateMachineBehaviour
{
    NavMeshAgent m_agent;
    Transform m_player;
    float AttackRadius = 6f;
    //float ChasingRadius = 5f;
    Transform EnemyEye;
    [Range(0, 360)] float ViewAngle = 165f;
    [SerializeField] float ViewDistance = 75f;
    private Monster_Sound monsterSound;
    private Coroutine intimidateCoroutine;
    private float lastSeenTime;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        m_agent = animator.GetComponent<NavMeshAgent>();

        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        EnemyEye = GameObject.FindGameObjectWithTag("Eye").transform;
        monsterSound = animator.GetComponent<Monster_Sound>();
        monsterSound.PlayChaseMusic();
        intimidateCoroutine = monsterSound.StartChaseIntimidate();
       
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if(IsInView()) lastSeenTime = Time.time;

        if(Time.time - lastSeenTime > 3f) 
        {
            animator.SetBool("IsChasing", false);
        }

        m_agent.SetDestination(m_player.position);

        float distance = Vector3.Distance(m_player.position, animator.transform.position);

        if (distance < AttackRadius) 
        {
            if (IsInView())
            {
                //Debug.Log("RunBeh: АТАКУЮТ!!");
                animator.SetBool("IsAttack", true);
            }
            else
            {
                animator.SetBool("IsChasing", false);
            }
        }

        if (!IsInView())
        {
            //Debug.Log("RunBeh: преследование закончилось");
            animator.SetBool("IsChasing", false);
            
        }
    
    
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_agent.SetDestination(m_agent.transform.position);
        monsterSound.StopChaseMusic(false);
        if(intimidateCoroutine != null) 
            monsterSound.StopChaseIntimidate(intimidateCoroutine);
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


    
}
