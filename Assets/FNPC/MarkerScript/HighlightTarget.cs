// HighlightTarget.cs
using TMPro;
using UnityEngine;

public class HighlightTarget : MonoBehaviour
{
    public Transform target;
    public TextMeshProUGUI distanceText;
    
    private Transform _player;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if(target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Позиционирование метки
        transform.position = target.position + Vector3.up * 2f;
        
        // Расчет расстояния
        float distance = Vector3.Distance(_player.position, target.position);
        distanceText.text = $"{distance:0.0}m";
        
        // Поворот к камере
        transform.rotation = Camera.main.transform.rotation;
    }
}