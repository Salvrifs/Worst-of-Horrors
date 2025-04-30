using UnityEngine;
using UnityEngine.AI;

public class Idle : StateMachineBehaviour
{
    float timer;
    Transform m_player;
    NavMeshAgent m_agent;
    [SerializeField] AudioSource Slap_audioSource;
    [SerializeField] AudioClip[] SlapSound;
    private Camera mainCamera;
    public float reachDistance = 3f;
    //[SerializeField] AudioClip[] Idle;
    //AudioSource IdleSound;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0f;
        m_agent = animator.GetComponent<NavMeshAgent>();
        m_player = GameObject.FindGameObjectWithTag("Player").transform; 
        Slap_audioSource = animator.GetComponent<AudioSource>();
        animator.SetBool("IsApp", false);
        //Debug.Log($"Idle: {player.name}"); 
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        if (timer >= 3)
        {
            //Debug.Log("IdleBeh: Патрулирование началось");
            animator.SetBool("IsPatrol", true);
            
        }
        
       float distance = Vector3.Distance(m_agent.transform.position, m_player.transform.position);
       
       RaycastHit hit;
       if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 7))
       {
           GameObject obj = hit.collider.gameObject;
           if (obj.CompareTag("NPCgrib"))
           {
               // GameObject GribText = GameObject.Find("GribTextSlap");
               // GribText.SetActive(true);
               if (Input.GetKeyDown(KeyCode.E))
               {
                   animator.SetBool("IsSlapped", true);
                   animator.SetBool("IsChasing", true);
                   PlaySlap_Sound();
               }
           }
       }

    }
    
    private void PlaySlap_Sound()
    {
        if (!Slap_audioSource.isPlaying)
        {
            Slap_audioSource.PlayOneShot(SlapSound[Random.Range(0, SlapSound.Length)]);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //IdleSound.Stop();
        
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
