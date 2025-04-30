using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
public class ButtonTrigger : MonoBehaviour
{
    [SerializeField] private TextAsset _inkJSON;
    [SerializeField] DialogueManager _dialogueManager;

    public void ChangeField(int value)
    {
        Story story = new Story(_inkJSON.text);

        story.variablesState["nameField"] = value;
        Debug.Log("nameField? " + story.variablesState["nameField"]);

    }
}
