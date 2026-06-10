using System.Collections;
using UnityEngine;
using TMPro;

public class TextFade : MonoBehaviour
{
    public string[] text;

    private TextMeshProUGUI tmpText;

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    public IEnumerator ShowText(int index, float fadeTime)
    {
        yield return StartCoroutine(FadeToText(index, fadeTime));
    }
    public IEnumerator FadeOut(float fadeTime)
    {
        Color color = tmpText.color;
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, t / fadeTime);
            tmpText.color = color;
            yield return null;
        }
        tmpText.text = "";
    }
    private IEnumerator FadeToText(int index, float fadeTime)
    {
        Color color = tmpText.color;
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, t / fadeTime);
            tmpText.color = color;
            yield return null;
        }
        tmpText.text = text[index];
        t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, t / fadeTime);
            tmpText.color = color;
            yield return null;
        }
    }
}