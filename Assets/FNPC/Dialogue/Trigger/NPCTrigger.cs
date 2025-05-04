using UnityEngine;
using Ink.Runtime;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(SphereCollider))] // Добавляем автоматически Sphere Collider
public class NPCTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private TextAsset _inkJson;
    [SerializeField] private float interactionRadius = 3f; // Радиус взаимодействия
    
    [Header("Debug")]
    [SerializeField] private bool showGizmos = true; // Визуализация в сцене

    private DialogueManager _dialogueManager;
    private bool _isPlayerInRange;
    [SerializeField] private NPCReactionController npcController;
    private void Awake()
    {
        // Настройка коллайдера
        SphereCollider collider = GetComponent<SphereCollider>();
        collider.radius = interactionRadius;
        collider.isTrigger = true;
    }

    private void Start()
    {
        _dialogueManager = FindObjectOfType<DialogueManager>();
    }

   private void Update()
{
    if (!_isPlayerInRange || _dialogueManager.IsDialoguingPlayer) return;
    

    if (Input.GetKeyDown(KeyCode.P))
    {
        _dialogueManager.EnterDialogueMode(_inkJson, npcController);
    }

    
}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = false;
        }
    }

    // Визуализация радиуса в редакторе
    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}