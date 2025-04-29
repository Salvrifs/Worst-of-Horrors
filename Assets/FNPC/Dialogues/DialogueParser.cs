// DialogueParser.cs
/*using System.Text.RegularExpressions;
using UnityEngine;

public static class DialogueParser
{
    public static DialogueData ParseDialogue(string text)
    {
        DialogueData data = new DialogueData();
        DialogueNode currentNode = null;
        string[] lines = text.Split('\n');

    Debug.Log($"Начало парсинга. Строк: {lines.Length}");

    foreach (string line in lines)
    {
        string trimmed = line.Trim();
        if (string.IsNullOrEmpty(trimmed)) continue;

        // Логируем каждую обработанную строку
        Debug.Log($"Обработка строки: '{trimmed}'");

            // Обработка узлов NPC (например: "Вариант [2.1]")
            if (Regex.IsMatch(trimmed, @"^Вариант\s*\[[\d\.]+\]"))
            {
                string nodeID = Regex.Match(trimmed, @"[\d\.]+").Value;
                currentNode = new DialogueNode { nodeID = nodeID, npcText = "" };
                
                data.nodes.Add(currentNode);
            }
            // Обработка опций игрока (например: "[2.1] Текст опции")
            else if (Regex.IsMatch(trimmed, @"^\[[\d\.]+\]"))
{
    if (currentNode == null) continue;

    
    Match match = Regex.Match(trimmed, @"\[([\d\.]+)\]");
    string targetID = match.Groups[1].Value; 
    string optionText = trimmed.Substring(match.Index + match.Length).Trim();

    currentNode.options.Add(new DialogueOption
    {
        text = optionText,
        targetNodeID = targetID 
    });
}
            // Обработка текста
            else
            {
                if (currentNode == null)
                {
                    var rootNode = data.nodes.Find(n => n.nodeID == "start");
                    if (rootNode == null)
                    {
                        rootNode = new DialogueNode { nodeID = "start", npcText = "" };
                        data.nodes.Add(rootNode);
                    }
                    rootNode.npcText += trimmed + "\n";
                    currentNode = rootNode; // <-- Добавьте эту строку
                }
                else
                {
                    currentNode.npcText += trimmed + "\n";
                }
        }
        }
        Debug.Log($"Парсинг завершен. Узлов: {data.nodes.Count}");
        return data;
    }
}*/