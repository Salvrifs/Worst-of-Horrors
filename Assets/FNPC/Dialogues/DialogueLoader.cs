// DialogueLoader.cs
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    public DialogueData LoadDialogue(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        if(textAsset == null)
        {
            Debug.LogError($"Dialogue file {fileName} not found!");
            return null;
        }

        return DialogueParser.ParseDialogue(textAsset.text);
    }
}