// DialogueParser.cs
using System.Text.RegularExpressions;
using UnityEngine;

public static class DialogueParser
{
    public static DialogueData ParseDialogue(string text)
    {
        DialogueData data = new DialogueData();
        DialogueNode currentNode = null;
        string[] lines = text.Split('\n');

        foreach(string line in lines)
        {
            string trimmed = line.Trim();
            if(string.IsNullOrEmpty(trimmed)) continue;

            // Обработка узлов NPC
            if(Regex.IsMatch(trimmed, @"^Вариант\s*\[\d+.*\]"))
            {
                currentNode = new DialogueNode {
                    nodeID = Regex.Match(trimmed, @"\d+.*").Value
                };
                data.nodes.Add(currentNode);
            }
            // Обработка опций игрока
            else if(Regex.IsMatch(trimmed, @"^\[\d+.*\]"))
            {
                if(currentNode == null) continue;

                string[] parts = trimmed.Split(new[] {']'}, 2);
                string targetID = Regex.Match(parts[0], @"\d+.*").Value;
                string optionText = parts[1].Trim();

                currentNode.options.Add(new DialogueOption {
                    text = optionText,
                    targetNodeID = targetID
                });
            }
            // Обработка текста
            else
            {
                if(currentNode == null)
                {
                    // Корневой узел
                    var rootNode = data.nodes.Find(n => n.nodeID == "start");
                    if(rootNode == null)
                    {
                        rootNode = new DialogueNode { nodeID = "start" };
                        data.nodes.Add(rootNode);
                    }
                    rootNode.npcText += (rootNode.npcText == null ? "" : "\n") + trimmed;
                }
                else
                {
                    currentNode.npcText += (currentNode.npcText == null ? "" : "\n") + trimmed;
                }
            }
        }

        return data;
    }
}