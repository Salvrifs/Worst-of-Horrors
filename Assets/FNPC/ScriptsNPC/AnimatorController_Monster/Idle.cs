using UnityEngine;
using UnityEngine.AI;

public class IdleBehaviour : StateMachineBehaviour
{

    float timer;
    Transform m_player;
    Transform EnemyEye;
    NavMeshAgent m_agent;
    [Range(0, 360)] float ViewAngle = 130f;
    float ViewDistance = 75f;
    //float ChaseDist = 5f;
    
    //[SerializeField] AudioClip[] Idle;
    //AudioSource IdleSound;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0f;

        m_agent = animator.GetComponent<NavMeshAgent>();
        m_player = GameObject.FindGameObjectWithTag("Player").transform; 
        EnemyEye = GameObject.FindGameObjectWithTag("Eye").transform;
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
        
       //float distance = Vector3.Distance(m_agent.transform.position, m_player.transform.position);
        if ( IsInView() )
        {
                //Debug.Log("IdleBeh: погоня началась");
                animator.SetBool("IsChasing", true);
        }

        //PlayIdle_Sound();
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //IdleSound.Stop();
        
    }

//Находится ли в поле зрения
private bool IsInView() 
    {
        float currentAngle = Vector3.Angle(EnemyEye.forward, m_player.position - EnemyEye.position);
        RaycastHit hit;
        if (Physics.Raycast(EnemyEye.transform.position, m_player.position - EnemyEye.position, out hit, ViewDistance))
        {
            if (currentAngle < ViewAngle / 2f && Vector3.Distance(EnemyEye.position, m_player.position) <= ViewDistance && hit.transform == m_player.transform)
            {
                return true;
            }
        }
        return false;
    }
//
//Звук
//
    /*private void PlayIdle_Sound()
    {
        if (IdleSound.isPlaying)
        {
            return;
        }
        IdleSound.PlayOneShot(Idle[Random.Range(0, Idle.Length)]);
    }*/


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
