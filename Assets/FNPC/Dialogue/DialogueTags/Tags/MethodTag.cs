using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueMethods))]
public class MethodTag : MonoBehaviour, ITag
{
    public void Calling(string value)
    {
        var dialogueMethods = GetComponent<DialogueMethods>();
        
        // Проверка наличия параметров
        if (value.Contains("(") && value.Contains(")"))
        {
            string methodName = value.Split('(')[0];
            string paramStr = value.Split('(', ')')[1];
            
            // Проверка, что параметр является числом
            if (int.TryParse(paramStr, out int param))
            {
                var method = dialogueMethods.GetType().GetMethod(methodName, new[] { typeof(int) });
                method?.Invoke(dialogueMethods, new object[] { param });
            }
            else
            {
                Debug.LogError($"Invalid parameter format: {paramStr}");
            }
        }
        else
        {
            // Метод без параметров
            var method = dialogueMethods.GetType().GetMethod(value);
            method?.Invoke(dialogueMethods, null);
        }
    }
}