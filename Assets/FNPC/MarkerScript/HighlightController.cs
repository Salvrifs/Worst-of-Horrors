// HighlightController.cs
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private List<GameObject> targetObjects = new List<GameObject>();

    private DialogueManager _dialogueManager;
    private List<GameObject> _activeHighlights = new List<GameObject>();

    void Start()
    {
        //_dialogueManager = DialogueManager.Instance;
        //DialogueManager.OnDialogueEnd += ActivateHighlights;
        gameObject.SetActive(false);
    }

    void ActivateHighlights()
    {
        gameObject.SetActive(true);
        foreach (var obj in targetObjects)
        {
            GameObject highlight = Instantiate(highlightPrefab, transform);
            HighlightTarget ht = highlight.GetComponent<HighlightTarget>();
            ht.target = obj.transform;
            _activeHighlights.Add(highlight);
        }
    }

    void Update()
    {
        // Опционально: проверка уничтоженных объектов
        for (int i = _activeHighlights.Count - 1; i >= 0; i--)
        {
            if (_activeHighlights[i] == null)
                _activeHighlights.RemoveAt(i);
        }
    }

    public void RemoveHighlight(GameObject targetObject)
    {
        // Удаление метки при сборе объекта
        foreach (var highlight in _activeHighlights)
        {
            if (highlight.GetComponent<HighlightTarget>().target == targetObject.transform)
            {
                Destroy(highlight);
                break;
            }
        }
    }
}