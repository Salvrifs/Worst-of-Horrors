using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using Unity.VisualScripting;
using System.ComponentModel.Design;

public class DialogueMethods : MonoBehaviour
{
    public void ExitForGame() => Application.Quit();

    // Новый метод для обработки тега <method:ChangeField(5)>
    public void ChangeField(int value)
    {
        Debug.Log($"Field changed to: {value}");
    }
}