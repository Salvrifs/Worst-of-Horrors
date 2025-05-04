// NPCReactionController.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
public class NPCReactionController : MonoBehaviour
{
    [Header("Reaction Settings")]
    [SerializeField] private float reactionDistance = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private KeyCode reactionKey = KeyCode.P;
    
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private bool isReacting;
    public bool IsReacting => isReacting;
    private Vector3 originalDestination;
    private bool wasFlying;
    private bool wasWaiting;
    
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isReacting && Input.GetKeyDown(reactionKey) && IsPlayerInRange())
        {
            StartCoroutine(ReactionRoutine());
        }
    }

    private bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.position) <= reactionDistance;
    }

    public void ResumePatrol()
    {
        if (!agent.isOnNavMesh) return;
        
        agent.isStopped = false;
        animator.SetBool("IsFlying", wasFlying);
        animator.SetBool("IsWaiting", wasWaiting);
        
        if(wasFlying)
            agent.SetDestination(originalDestination);
    }

     private IEnumerator ReactionRoutine()
{
    // Сохраняем оригинальное состояние
    originalDestination = agent.destination;
    wasFlying = animator.GetBool("IsFlying"); // Используем поле класса
    wasWaiting = animator.GetBool("IsWaiting");

    isReacting = true;
    
    // Прерываем текущее движение
    if (agent.isOnNavMesh)
    {
        agent.isStopped = true;
        animator.SetBool("IsFlying", false);
        animator.SetBool("IsWaiting", false);
    }

    // Поворот к игроку
    Vector3 direction = (player.position - transform.position).normalized;
    Quaternion targetRotation = Quaternion.LookRotation(direction);
    
    while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation, 
            rotationSpeed * Time.deltaTime
        );
        yield return null;
    }

    // Возврат к предыдущему состоянию
    yield return new WaitForSeconds(1f);
    
    if (wasFlying && agent.isOnNavMesh) // Теперь используем поле класса
    {
        agent.isStopped = false;
        animator.SetBool("IsFlying", true);
    }
    else
    {
        animator.SetBool("IsWaiting", true);
    }
    
    isReacting = false;
}

public void InterruptReaction()
{
    StopAllCoroutines();
    isReacting = true;
    
    // Сохраняем состояние
    originalDestination = agent.destination;
    wasFlying = animator.GetBool("IsFlying");
    wasWaiting = animator.GetBool("IsWaiting");
    
    // Поворот к игроку
    StartCoroutine(RotateToPlayer());
}

private IEnumerator RotateToPlayer()
{
    Vector3 direction = (player.position - transform.position).normalized;
    Quaternion targetRotation = Quaternion.LookRotation(direction);
    
    while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation, 
            rotationSpeed * Time.deltaTime
        );
        yield return null;
    }
}
}