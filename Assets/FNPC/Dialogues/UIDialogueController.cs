// UIDialogueController.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIDialogueController : MonoBehaviour
{
    public static UIDialogueController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Text dialogueText;
    [SerializeField] private GameObject optionButtonPrefab;
    [SerializeField] private Transform optionsContainer;
    [SerializeField] private GameObject dialoguePanel;
    
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            dialoguePanel.SetActive(false); 
        }
        
    }

    public void AppendDialogueText(char character)
    {
        if(dialogueText != null)
        {
            dialogueText.text += character;
        }
    }

    public void ShowDialogue()
    {
        if(dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            optionsContainer.gameObject.SetActive(false);
        }
    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }

    public void ShowOptions(List<DialogueOption> options)
    {
        ClearOptions();
        dialoguePanel.SetActive(true);

        foreach(var option in options)
        {
            GameObject button = Instantiate(optionButtonPrefab, optionsContainer);
            button.GetComponentInChildren<Text>().text = option.text;
            button.GetComponent<Button>().onClick.AddListener(() => 
                DialogueManager.Instance.SelectOption(option.targetNodeID));
        }
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
        ClearOptions();
    }

    private void ClearOptions()
    {
        foreach(Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }
    }
}