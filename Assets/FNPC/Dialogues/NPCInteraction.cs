using UnityEngine;


public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private string dialogueFile;
    
    public void OnInteract()
    {
        // Используйте существующий DialogueLoader
        DialogueLoader loader = FindObjectOfType<DialogueLoader>();
        if(loader != null)
        {
            DialogueData data = loader.LoadDialogue(dialogueFile);
            DialogueManager.Instance.StartDialogue(data);
        }
    }
}