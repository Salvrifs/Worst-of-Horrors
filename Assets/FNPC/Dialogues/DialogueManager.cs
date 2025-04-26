// DialogueManager.cs
using System.Collections;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private float textSpeed = 0.05f;
    private DialogueData currentDialogue;
    private DialogueNode currentNode;
    private bool isTyping;

    void Awake()
{
    if(Instance == null)
    {
        Instance = this;
            // Уберите DontDestroyOnLoad если не требуется
        }
        else
    {
        Destroy(gameObject);
    }
    }

    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentNode = currentDialogue.nodes.Find(n => n.nodeID == "start");
        StartCoroutine(TypeText(currentNode.npcText));
    }

    private IEnumerator TypeText(string text)
{
    // Добавьте проверку
    if(UIDialogueController.Instance == null) yield break;
    
    UIDialogueController.Instance.ShowDialogue();
    
    isTyping = true;
    UIDialogueController.Instance.SetDialogueText("");
    
    foreach(char c in text)
    {
        // Проверка на уничтожение
        if(UIDialogueController.Instance == null) yield break;
        
        UIDialogueController.Instance.AppendDialogueText(c);
        yield return new WaitForSeconds(textSpeed);
    }
    
    isTyping = false;
    UIDialogueController.Instance.ShowOptions(currentNode.options);
}

    public void SelectOption(string targetNodeID)
    {
        currentNode = currentDialogue.nodes.Find(n => n.nodeID == targetNodeID);
        if(currentNode != null)
        {
            StartCoroutine(TypeText(currentNode.npcText));
        }
        else
        {
            EndDialogue();
        }
    }

    public void SkipTyping()
    {
        if(isTyping)
        {
            StopAllCoroutines();
            UIDialogueController.Instance.SetDialogueText(currentNode.npcText);
            isTyping = false;
            UIDialogueController.Instance.ShowOptions(currentNode.options);
        }
    }

    private void EndDialogue()
    {
        UIDialogueController.Instance.HideDialogue();
    }
}