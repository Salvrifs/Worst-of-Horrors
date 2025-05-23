using System;
using System.Collections;
using Unity.VisualScripting;
//using UnityEditor.PackageManager;
using UnityEngine;
using Ink.Runtime;

[RequireComponent(typeof(DialogueWindow), typeof(DialogueTag))]
public class DialogueManager : MonoBehaviour
{
    private bool isDialoguing = false;
    
    // И это свойство для доступа извне
    public bool IsDialoguingPlayer => isDialoguing;
    private DialogueWindow _dialogueWindow;
    private DialogueTag _dialogueTag;
    public Story currentStory {get; private set;}
    private Coroutine displayLineCoroutine;
    [SerializeField] private GGCameraMoving cameraMoving;
    
    private float previousTimeScale;
    public static event Action<bool, bool> OnDialogueEnd;
    [SerializeField] private GGMoving playerMovement;
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
    //
    //Вход в диалог
    //
public void EnterDialogueMode(TextAsset inkJson, NPCReactionController npc)
{
    isDialoguing = true;
    
    // Блокируем движение игрока
    playerMovement.SetMovementAllowed(false);
    
    cameraMoving.SetControlEnabled(false);
    
    // Сохраняем и блокируем NPC
    NPCReactionController currentNPC = npc;
    currentNPC.InterruptReaction();

    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;

    currentStory = new Story(inkJson.text);
    _dialogueWindow.SetActive(true);
    
    ContinueStory();
}
    //
    //Окончание диалога
    //
    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSecondsRealtime(_dialogueWindow.CoolDownNextLetter);

        NPCReactionController currentNPC = FindObjectOfType<NPCTrigger>().GetComponent<NPCReactionController>();
        if(currentNPC != null)
        {
            currentNPC.ResumePatrol();
        }

        _dialogueWindow.SetActive(false);
        _dialogueWindow.ClearText();
        
        playerMovement.SetMovementAllowed(true);
        cameraMoving.SetControlEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        
        bool chosePotion = false;
        bool choseBridge = false;

        if (currentStory != null)
    {
        chosePotion =  GetInkVariable("chosePotion", false);
        choseBridge =  GetInkVariable("choseBridge", false);
    }
    isDialoguing = false;
        OnDialogueEnd?.Invoke(chosePotion, choseBridge);
         GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = false;
    }

    

    private bool GetInkVariable(string variableName, bool defaultValue)
{
    if (currentStory.variablesState[variableName] != null)
    {
        return (bool)currentStory.variablesState[variableName];
    }
    return defaultValue;
}

    public void ContinueStory()
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
            Debug.Log("ne display stop coroutine");
            StopCoroutine(displayLineCoroutine);
        }
Debug.Log("display non-stop coroutine");
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