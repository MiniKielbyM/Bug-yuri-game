using UnityEngine;

public class ResizeDiaHead : MonoBehaviour
{
    Vector2 AnchoredPos;
    Vector2 SizeDelta;
    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector2 pos = rect.anchoredPosition;
        pos.x = rect.sizeDelta.x / 2f;
        rect.anchoredPosition = pos;
    }
}
