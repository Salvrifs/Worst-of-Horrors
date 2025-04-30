using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using Ink.Runtime;


public class DialogueOption : MonoBehaviour
{
    [SerializeField] private GameObject[] _options;
    private TextMeshProUGUI[] _optionText;

    public void Init()
    {
        _optionText = new TextMeshProUGUI[_options.Length];

        ushort index = 0;
        foreach (GameObject option in _options)
        {
            Debug.Log("OptionText: " + option + " name: " + option.name);
            _optionText[index++] = option.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public bool DisplayOptions(Story story)
{
    Debug.Log("Display!"); 
    
    
    // Получите текущие варианты выбора
    Choice[] currentOptions = story.currentChoices.ToArray();
    Debug.Log(story.currentText);
    // Логируем количество доступных вариантов
    Debug.Log("Number of choices available: " + currentOptions.Length);
    
    foreach (var curr in currentOptions)
    {
        Debug.Log("Choice text: " + curr.text + " - Index: " + Array.IndexOf(currentOptions, curr));
    }

    if (currentOptions.Length == 0) 
    {
        Debug.LogWarning("No choices found!");
        return false;
    }
    
    if (currentOptions.Length > _options.Length)
        throw new ArgumentException("Слишком много вариантов ответа!");

    HideOptions(); // Скрыть все кнопки перед отображением новых

    for (int i = 0; i < currentOptions.Length; i++)
    {
        if (i >= _options.Length) break;
        _options[i].SetActive(true);
        _optionText[i].text = currentOptions[i].text; // Убедитесь, что это корректно
    }

    return currentOptions.Length > 0;
}


    public void HideOptions()
    {
        Array.ForEach(_options, (button) => {button.SetActive(false);});
    }
}
