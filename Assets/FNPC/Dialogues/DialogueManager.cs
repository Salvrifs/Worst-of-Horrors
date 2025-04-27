using System.Collections;
using UnityEditor.PackageManager;
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
    [SerializeField] private GGCameraMoving cameraController;
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

    if (cameraController != null)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cameraController.SetControlEnabled(false);
    }
    else
    {
        Debug.LogError("Camera Controller не назначен в инспекторе!");
    }

    // Принудительно активируем UI
    UIDialogueController.Instance.ShowDialogue();
    typingCoroutine = StartCoroutine(TypeText(currentNode.npcText));
}

    private IEnumerator TypeText(string text)
{
    isTyping = true;
    UIDialogueController.Instance.SetDialogueText("");
    
    Debug.Log($"Typing text: '{text}'"); // Логируем текущий текст
    
    foreach (char c in text)
    {
        UIDialogueController.Instance.AppendDialogueText(c);
        yield return new WaitForSeconds(textSpeed);
    }
    
    isTyping = false;
    Debug.Log("Text typing complete.");
    ShowOptions();
}

private void ShowOptions()
{
    Debug.Log($"Showing options for node ID '{currentNode.nodeID}'. Options count: {currentNode.options.Count}");

    if (currentNode != null && currentNode.options.Count > 0)
    {
        UIDialogueController.Instance.ShowOptions(currentNode.options);
    }
    else
    {
        Debug.Log("No options found, ending dialogue...");
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
    Debug.Log($"Выбрана опция с targetNodeID: {targetNodeID}");
    var nextNode = currentDialogue.nodes.Find(n => n.nodeID == targetNodeID);
    if (nextNode != null)
    {
        Debug.Log($"Узел найден: {nextNode.nodeID}");
        currentNode = nextNode;
        typingCoroutine = StartCoroutine(TypeText(currentNode.npcText));
    }
    else
    {
        Debug.LogError($"Узел с ID {targetNodeID} не найден!");
    }
}


    private void EndDialogue()
    {
        if (cameraController != null)
        {
            cameraController.SetControlEnabled(true);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isDialogueActive = false;
        UIDialogueController.Instance.HideDialogue();
    }
}