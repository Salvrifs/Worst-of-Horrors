using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using Ink.Runtime;

[RequireComponent(typeof(DialogueWindow), typeof(DialogueTag))]
public class DialogueManager : MonoBehaviour
{
    private DialogueWindow _dialogueWindow;
    private DialogueTag _dialogueTag;
    public Story currentStory {get; private set;}
    private Coroutine displayLineCoroutine;

    void Awake()
    {
        _dialogueTag = GetComponent<DialogueTag>();
        _dialogueWindow = GetComponent<DialogueWindow>();

        _dialogueTag.Init();
        _dialogueWindow.Init();
    }

    private void Start()
    {
        _dialogueWindow.SetActive(false);   
    }


    private void Update()
    {
        if (_dialogueWindow.IsStatusAnswer == true || 
            _dialogueWindow.IsPlaying == false ||
            _dialogueWindow.CanContinueTONextLine == false)
            {
                return;
            }   
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(0))
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJson)
    {
        currentStory = new Story(inkJson.text);

        _dialogueWindow.SetActive(true);

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(_dialogueWindow.CoolDownNextLetter);

        _dialogueWindow.SetActive(false);
        _dialogueWindow.ClearText();
    }

    private void ContinueStory()
{
    if (currentStory.canContinue == false)
    {
        if (displayLineCoroutine != null)
        {
            StopCoroutine(displayLineCoroutine);
        }
        StartCoroutine(ExitDialogueMode());
        return;
    }

        if (displayLineCoroutine != null)
        {
            StopCoroutine(displayLineCoroutine);
        }

        displayLineCoroutine = StartCoroutine(_dialogueWindow.DisplayLine(currentStory));

        try
        {
            _dialogueTag.HandleTags(currentStory.currentTags);
        }

        catch(ArgumentException ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public void MakeChoice(int choiceIndex)
{
    // Проверка на валидность индекса
    /*if (choiceIndex < 0 || choiceIndex >= currentStory.currentChoices.Count)
    {
        Debug.LogError($"Invalid choice index: {choiceIndex}");
        return;
    }*/

    _dialogueWindow.MakeChoice();
    currentStory.ChooseChoiceIndex(choiceIndex);
    ContinueStory();
}
}