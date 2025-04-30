using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class ChasingStop : StateMachineBehaviour
{
    float timer;
    NavMeshAgent m_agent;
    Transform m_player;
    float stopRadius = 5f;
    float resumeRadius = 7f;
    [SerializeField] AudioClip[] intimidateSound; 
    [SerializeField] AudioSource intimidate_audioSource;
    int ind;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        m_agent = animator.GetComponent<NavMeshAgent>();
        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        intimidate_audioSource = animator.GetComponent<AudioSource>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float distance = Vector3.Distance(m_player.position, animator.transform.position);
        
        // Если игрок далеко - продолжаем преследование
        if (distance > resumeRadius)
        {
            animator.SetBool("ChasingStop", false);
            m_agent.isStopped = false;
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
        //         int ind = 1;
        //         var s = quickslot.transform;
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
        
        // RaycastHit hit;
        // if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 7) && Input.GetKeyDown(KeyCode.E))
        // {
        //     GameObject obj = hit.collider.gameObject;
        //     if (obj.CompareTag("NPCgrib"))
        //     {
        //         if (inventory.quickslotParent.GetChild(inventory.currentQuickslotID).GetComponent<InventorySlot>()
        //                 .is_item != null)
        //         {
        //             if (inventory.quickslotParent.GetChild(inventory.currentQuickslotID).GetComponent<InventorySlot>()
        //                     .is_item.isConsumeable &&
        //                 inventory.quickslotParent.GetChild(inventory.currentQuickslotID).GetComponent<Image>().sprite ==
        //                 inventory.selectedSprite)
        //             {
        //
        //                 inventory.RemoveItem();
        //                 animator.SetBool("ChasingStop", false);
        //                 animator.SetBool("IsChasing", false);
        //                 animator.SetBool("IsApp", true);
        //
        //             }
        //         }
        //     }
        // }
    }
    

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_agent.isStopped = false;
        intimidate_audioSource.Stop();
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
    
    private void PlayChase_Sound()
    {
        if (intimidate_audioSource.clip != intimidateSound[ind])
        {
            intimidate_audioSource.PlayOneShot(intimidateSound[ind]);
            ind = Random.Range(0, intimidateSound.Length);
        }
    }
}
