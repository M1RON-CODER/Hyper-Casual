using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Rect saveArea = Screen.safeArea;
        Vector2 anchorMin = saveArea.position;
        Vector2 anchorMax = anchorMin + saveArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
