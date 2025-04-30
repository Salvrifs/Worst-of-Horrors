using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class Chase : StateMachineBehaviour
{
    float timer;
    NavMeshAgent m_agent;
    Transform m_player;
    float stopRadius = 5f;
    float resumeRadius = 7f; // Дистанция для возобновления преследования
    private float speedPlus = 2.5f;
    [SerializeField] AudioClip[] intimidateSound; 
    [SerializeField] AudioSource intimidate_audioSource;
    int ind;
    

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        m_agent = animator.GetComponent<NavMeshAgent>();
        m_agent.speed += speedPlus;
        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        animator.SetBool("IsSlapped", false);
        intimidate_audioSource = animator.GetComponent<AudioSource>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float distance = Vector3.Distance(m_player.position, animator.transform.position);
        
        // Если игрок близко - останавливаемся
        if (distance < stopRadius)
        {
            m_agent.isStopped = true;
            animator.SetBool("ChasingStop", true);
        }
        
        // Если в режиме преследования - двигаемся к игроку
        if (animator.GetBool("IsChasing"))
        {
            m_agent.SetDestination(m_player.position);
        }
        
        timer += Time.deltaTime;
        if (timer > 3)
        {
            PlayChase_Sound();
            timer = 0;
        }
        
        // RaycastHit hit;
        // if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 7) && Input.GetKeyDown(KeyCode.E))
        // {
        //     GameObject obj = hit.collider.gameObject;
        //     if (obj.CompareTag("NPCgrib"))
        //     {
        //         GameObject quickslot = GameObject.Find("QuickSlotPanel");
        //         int ind = quickslot.transform.GetSiblingIndex();
        //         InventorySlot slot = quickslot.transform.GetChild(ind)?.GetComponent<InventorySlot>();
        //         if (slot != null && slot.is_item != null && slot.amount > 0)
        //         {
        //             slot.amount--;
        //             slot.textItemAmount.text = slot.amount.ToString();
        //             if (slot.amount <= 0)
        //             {
        //                 slot.NullifySlotData();
        //                 animator.SetBool("ChasingStop", false);
        //                 animator.SetBool("IsChasing", false);
        //                 animator.SetBool("IsApp", true);
        //             }
        //         }
        //     }
        // }
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_agent.isStopped = true;
        m_agent.speed -= speedPlus;
        intimidate_audioSource.Stop();
    }
    
    private void PlayChase_Sound()
    {
        if (intimidate_audioSource.clip != intimidateSound[ind])
        {
            intimidate_audioSource.PlayOneShot(intimidateSound[ind]);
            ind = Random.Range(0, intimidateSound.Length);
        }
    }
}
