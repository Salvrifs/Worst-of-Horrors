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
    [SerializeField] private GGCameraMoving cameraMoving;
    
    private float previousTimeScale;

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
        if (_dialogueWindow.IsStatusAnswer || 
            !_dialogueWindow.IsPlaying ||
            !_dialogueWindow.CanContinueTONextLine)
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
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        cameraMoving.SetControlEnabled(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        currentStory = new Story(inkJson.text);
        _dialogueWindow.SetActive(true);

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSecondsRealtime(_dialogueWindow.CoolDownNextLetter);

        _dialogueWindow.SetActive(false);
        _dialogueWindow.ClearText();

        cameraMoving.SetControlEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = previousTimeScale;
    }

    private void ContinueStory()
{
    Debug.Log("ContinueStory");
    if (currentStory.canContinue == false)
    {
        Debug.Log("can continue == false");
        if (displayLineCoroutine != null)
        {
            StopCoroutine(displayLineCoroutine);
        }
        StartCoroutine(ExitDialogueMode());
        return;
    }
Debug.Log("can continue == true");
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

    // DialogueManager.cs
public void MakeChoice(int choiceIndex)
{
    Debug.Log("Choice was made!");
    _dialogueWindow.MakeChoice();
    for (int i = 0; i < currentStory.currentChoices.ToArray().Length; ++i)
    {
        Debug.Log("Choice: " + currentStory.currentChoices[i]);
    }
    currentStory.ChooseChoiceIndex(choiceIndex);
    _dialogueWindow.DialogueOption.HideOptions(); 
    currentStory.Continue(); // Запускает следующий узел
    ContinueStory();
}
}