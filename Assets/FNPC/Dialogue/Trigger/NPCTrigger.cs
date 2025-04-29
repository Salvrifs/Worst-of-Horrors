using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class NPCTrigger : MonoBehaviour
{
    [SerializeField] private TextAsset _inkJson;

    private bool IsPlayerEnter;

    private DialogueManager _dialogueManager;
    private DialogueWindow _dialogueWindow;

    private void Start()
    {
        IsPlayerEnter = false;

        _dialogueManager = FindObjectOfType<DialogueManager>();
        _dialogueWindow = FindObjectOfType<DialogueWindow>();
    }

    private void Update()
    {
        if (_dialogueWindow.IsPlaying || IsPlayerEnter == false)
        {
            return;
        }   

        if (Input.GetKeyDown(KeyCode.P))
        {
            _dialogueManager.EnterDialogueMode(_inkJson);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        GameObject obj = collider.gameObject;

        if (obj.GetComponent<GGMoving>() != null)
        {
            IsPlayerEnter = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        GameObject obj = collider.gameObject;

        if (obj.GetComponent<GGMoving>() != null)
        {
            IsPlayerEnter = false;
        }
    }
}
