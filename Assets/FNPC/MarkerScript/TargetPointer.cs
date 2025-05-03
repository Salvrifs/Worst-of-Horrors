// TargetPointer.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TargetPointer : MonoBehaviour
{
    public Sprite pointerIcon;
    public Sprite outOfScreenIcon;
    public float interfaceScale = 100f;
    public string distanceFormat = "0.0 m";
    public Vector2 textOffset = new Vector2(30f, 0f);
    
    private Camera mainCamera;
    private bool isActive;
    private Rect screenRect;

    [Header("Target Lists")]
    public List<TargetData> potionTargets = new List<TargetData>();
    public List<TargetData> bridgeTargets = new List<TargetData>();
    
    private List<TargetData> activeTargets = new List<TargetData>();

    private void Awake()
    {
        mainCamera = Camera.main;
        DialogueManager.OnDialogueEnd += HandleDialogueEnd;
        SetPointersActive(false);
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
    }

    private void OnDestroy()
    {
        DialogueManager.OnDialogueEnd -= HandleDialogueEnd;
    }

    private void ActivatePointers()
    {
        isActive = true;
        SetPointersActive(true);
    }

    private void HandleDialogueEnd(bool chosePotion, bool choseBridge)
    {
        activeTargets.Clear();
        
        if (chosePotion) activeTargets = potionTargets;
        else if (choseBridge) activeTargets = bridgeTargets;
        Debug.Log($"ChosePotion {chosePotion}, ChoseBridge: {choseBridge}");
        ActivatePointers();
    }

    private void SetPointersActive(bool state)
    {
        foreach (var targetData in activeTargets)
        {
            Debug.Log($"targetData: {targetData}\ttargetData: {targetData.name}");
            if (targetData.pointerUI != null)
                targetData.pointerUI.gameObject.SetActive(state);
            
            if (targetData.distanceText != null)
                targetData.distanceText.gameObject.SetActive(state);
        }
    }

    private void LateUpdate()
    {
        if (!isActive) return;

        foreach (var targetData in activeTargets)
        {
            if (targetData.target == null) continue;

            UpdatePointer(targetData);
            UpdateDistanceText(targetData);
        }
    }

    private void UpdatePointer(TargetData targetData)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetData.target.position);
        bool isBehind = IsBehind(targetData.target.position);
        bool isVisible = screenRect.Contains(screenPos) && !isBehind;

        Image pointerImage = targetData.pointerUI.GetComponent<Image>();
        pointerImage.sprite = isVisible ? pointerIcon : outOfScreenIcon;

        Vector3 clampedPos = isVisible ? 
            ClampPosition(screenPos) : 
            GetEdgePosition(screenPos);

        targetData.pointerUI.anchoredPosition = clampedPos;
        targetData.pointerUI.sizeDelta = new Vector2(interfaceScale, interfaceScale);

        if (!isVisible) RotatePointer(targetData.pointerUI, screenPos - clampedPos);
    }

	private Vector3 GetEdgePosition(Vector3 screenPos)
    {
        Vector3 direction = screenPos - new Vector3(Screen.width/2, Screen.height/2, 0);
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x);
        float slope = Mathf.Tan(angle);

        Vector3 edgePos = new Vector3();
        if (Mathf.Abs(slope) > Screen.height/(float)Screen.width)
        {
            edgePos.y = Mathf.Sign(direction.y) * Screen.height/2;
            edgePos.x = edgePos.y / slope;
        }
        else
        {
            edgePos.x = Mathf.Sign(direction.x) * Screen.width/2;
            edgePos.y = edgePos.x * slope;
        }

        return edgePos + new Vector3(Screen.width/2, Screen.height/2, 0);
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, 0, Screen.width),
            Mathf.Clamp(position.y, 0, Screen.height),
            position.z
        );
    }

     private void UpdateDistanceText(TargetData targetData)
    {
        if (targetData.distanceText == null || !targetData.showDistance) return;

        float distance = Vector3.Distance(mainCamera.transform.position, targetData.target.position);
        targetData.distanceText.text = distance.ToString(distanceFormat);

        // Рассчитываем позицию текста относительно указателя
        Vector2 textPosition = CalculateTextPosition(targetData);
        targetData.distanceText.rectTransform.anchoredPosition = textPosition;
    }
	private Vector2 CalculateTextPosition(TargetData targetData)
    {
        Vector2 pointerPos = targetData.pointerUI.anchoredPosition;
        bool isBehind = IsBehind(targetData.target.position);

        // Если цель за экраном
        if (isBehind)
        {
            // Определяем направление от центра экрана
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 dir = (pointerPos - screenCenter).normalized;

            // Смещаем текст в противоположную сторону от края
            return pointerPos + dir * textOffset;
        }

        // Если цель на экране
        Vector2 offset = textOffset;

        // Проверяем выход за границы экрана
        if (pointerPos.x + textOffset.x > Screen.width) offset.x = -textOffset.x;
        if (pointerPos.y + textOffset.y > Screen.height) offset.y = -textOffset.y;

        return pointerPos + offset;
    }

    private Vector3 CalculateClampedPosition(Vector3 screenPos, bool isBehind)
    {
        Vector3 result = screenPos;
        if (isBehind)
        {
            result.x = Mathf.Clamp(result.x, 0, Screen.width);
            result.y = result.y < Screen.height/2 ? Screen.height : 0;
        }

        result.x = Mathf.Clamp(result.x, 0, Screen.width);
        result.y = Mathf.Clamp(result.y, 0, Screen.height);
        return result;
    }

    private bool IsBehind(Vector3 position)
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 toTarget = position - mainCamera.transform.position;
        return Vector3.Dot(cameraForward, toTarget) < 0;
    }

    private void RotatePointer(RectTransform pointer, Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        pointer.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}