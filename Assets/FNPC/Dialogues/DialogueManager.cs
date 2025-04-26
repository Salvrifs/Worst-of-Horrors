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
    private bool isDialogueActive;
    private Coroutine typingCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void StartDialogue(DialogueData dialogue)
{
    currentDialogue = dialogue;
    currentNode = currentDialogue.nodes.Find(n => n.nodeID == "start");
    isDialogueActive = true;

    // Принудительно активируем UI
    UIDialogueController.Instance.ShowDialogue();
    typingCoroutine = StartCoroutine(TypeText(currentNode.npcText));
}

    private IEnumerator TypeText(string text)
{
    isTyping = true;
    UIDialogueController.Instance.SetDialogueText("");
    
    foreach (char c in text)
    {
        UIDialogueController.Instance.AppendDialogueText(c);
        yield return new WaitForSeconds(textSpeed);
    }
    
    isTyping = false;
    ShowOptions(); // Убедитесь, что метод вызывается
}

    private void ShowOptions()
{
    // Добавьте проверку на существование узла
    if (currentNode != null && currentNode.options.Count > 0)
    {
        UIDialogueController.Instance.ShowOptions(currentNode.options);
    }
    else
    {
        EndDialogue();
    }
}

    public void SkipTyping()
{
    if (!isDialogueActive || currentNode == null) return;

    if (isTyping)
    {
        StopCoroutine(typingCoroutine);
        UIDialogueController.Instance.SetDialogueText(currentNode.npcText);
        isTyping = false;
        ShowOptions();
    }
    else if (currentNode.options.Count == 0)
    {
        EndDialogue();
    }
}

    public void SelectOption(string targetNodeID)
    {
        var nextNode = currentDialogue.nodes.Find(n => n.nodeID == targetNodeID);
        if (nextNode != null)
        {
            currentNode = nextNode;
            typingCoroutine = StartCoroutine(TypeText(currentNode.npcText));
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        UIDialogueController.Instance.HideDialogue();
    }
}