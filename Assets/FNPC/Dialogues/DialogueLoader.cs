// DialogueLoader.cs
/*using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    // DialogueLoader.cs
public DialogueData LoadDialogue(string fileName)
{
    TextAsset textAsset = Resources.Load<TextAsset>(fileName);
    
    if(textAsset == null)
    {
        Debug.LogError($"Файл '{fileName}' не найден в папке Resources!");
        return null;
    }
    else
    {
        Debug.Log($"Файл '{fileName}' успешно загружен!"); // Лог успеха
    }
    
    return DialogueParser.ParseDialogue(textAsset.text);
}
}*/