using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode
{
public string nodeID;
    public string npcText;
    public List<DialogueOption> options = new List<DialogueOption>();
}
