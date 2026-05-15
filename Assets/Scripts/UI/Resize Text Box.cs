using System.Runtime.InteropServices;
using UnityEngine;

public class ResizeTextBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();
        RectTransform parentRect = transform.parent as RectTransform;
        if (parentRect != null)
        {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentRect.rect.width);
        }
        rect.sizeDelta = new Vector2(rect.sizeDelta.x - parentRect.Find("Head").GetComponent<RectTransform>().rect.width, rect.sizeDelta.y);
        Vector2 pos = rect.anchoredPosition;
        pos.x = 0f - (rect.sizeDelta.x / 2f);
        rect.anchoredPosition = pos;
    }
}
