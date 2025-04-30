using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using Unity.VisualScripting;
using System.ComponentModel.Design;
using Ink.Runtime;
using System.Collections;

[RequireComponent(typeof(DialogueOption))]
public class DialogueWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _displayName;
    [SerializeField] private TextMeshProUGUI _displayText;
    [SerializeField] private GameObject _dialogueWindow;
    [SerializeField, Range(0,20)] private float _cooldownNextLetter;

    private DialogueOption _dialogueOption;
public DialogueOption DialogueOption => _dialogueOption;
    public bool IsStatusAnswer {get; private set;}
    public bool IsPlaying {get; private set;}
    public bool CanContinueTONextLine {get; private set;}

    public float CoolDownNextLetter
    {
        get => _cooldownNextLetter;

        private set
        {
            _cooldownNextLetter = CheckCoolDown(value);
        }
    }

    private float CheckCoolDown(float value)
    {
        if (value < 0)
            throw new ArgumentException("Неверное значение");
        return value;
    }

    public void Init()
    {
        IsStatusAnswer = false;
        CanContinueTONextLine = false;

        _dialogueOption = GetComponent<DialogueOption>();
        _dialogueOption.Init();
    }

    public void SetText(char letter)
    {
        _displayText.text += letter;
    }

    public void ClearText()
    {
        SetText("");
    }

    public void Add(string text)
    {
        _displayText.text += text;
    }

    public void Add(char letter)
    {
        _displayText.text += letter;
    }

    public void SetName(string name)
    {
        _displayName.text = name;       
    }

    public void SetText(string text)
    {
        _displayText.text = text;
    }

    public void SetCooldown(float cooldown)
    {
        CoolDownNextLetter = cooldown;
    }

    public void SetActive(bool IsActive)
    {
        IsPlaying = IsActive;
        _dialogueWindow.SetActive(IsActive);
    }

    public void MakeChoice()
{
    if (!CanContinueTONextLine || !IsPlaying) 
    {
        return;
    }
    IsStatusAnswer = true;
}

    // DialogueWindow.cs
public IEnumerator DisplayLine(Story story)
{
    //Debug.Log("DisplayLine" + $"CanContinue? {story.canContinue} currentChoices: {story.currentChoices.ToArray()[0]}, {story.currentChoices.ToArray()[1]}, {story.currentChoices.ToArray()[2]}");
    string line = story.Continue();
    ClearText();
    CanContinueTONextLine = false;
    IsStatusAnswer = false; // Сбросить статус перед новой строкой

    bool isAddingRichText = false;

    foreach (char letter in line.ToCharArray())
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetText(line);
            break;
        }

        isAddingRichText = letter == '<' || isAddingRichText;
        if (letter == '>') isAddingRichText = false;

        Add(letter);
        if (!isAddingRichText)
            yield return new WaitForSeconds(_cooldownNextLetter);
    }

    CanContinueTONextLine = true;
    Debug.Log("story: " + story.currentText + " story: ");
    IsStatusAnswer = _dialogueOption.DisplayOptions(story); // Показать кнопки после текста
}


}