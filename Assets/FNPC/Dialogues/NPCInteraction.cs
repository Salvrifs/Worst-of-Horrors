using UnityEngine;


public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private string dialogueFile;
    
    // NPCInteraction.cs
public void OnInteract()
{
    Debug.Log("Interact called!"); 
    
    DialogueLoader loader = FindObjectOfType<DialogueLoader>();
    if(loader != null)
    {
        DialogueData data = loader.LoadDialogue(dialogueFile);
        if(data != null)
        {
            Debug.Log("Dialogue data loaded!"); 
            DialogueManager.Instance.StartDialogue(data);
        }
        else
        {
            Debug.LogError("Failed to load dialogue data!");
        }
    }
}
}