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
            _optionText[index++] = option.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public bool DisplayOptions(Story story)
    {
        Choice[] currentOptions = story.currentChoices.ToArray();

        if (currentOptions.Length > _options.Length)
        {
            throw new ArgumentException("Ошибка! Слишком много выборов!");
        }

        HideOptions();

        ushort index = 0;
        
        foreach (Choice option in currentOptions)
        {
            _options[index].SetActive(true);
            _optionText[index++].text = option.text;
        }

        return currentOptions.Length > 0;
    }

    public void HideOptions()
    {
        Array.ForEach(_options, (button) => {button.SetActive(false);});
    }
}
