using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
public class PrologueController : MonoBehaviour
{
    public TextFade textFade1;
    public TextFade textFade2;
    public float lineDelay = 2f;
    public float fadeTime = 1f;
    public GameObject PrologueBG;
    private IEnumerator Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        for (int i = 0; i < textFade1.text.Length; i++)
        {
            yield return StartCoroutine(
                textFade1.ShowText(i, fadeTime)
            );

            yield return new WaitForSeconds(lineDelay);
        }
        StartCoroutine(textFade1.FadeOut(fadeTime));
        yield return StartCoroutine(FadeImageOut(PrologueBG));
        Destroy(PrologueBG.transform.parent.gameObject);
        Debug.Log("end prologue");
        yield return new WaitForSeconds(1f);
        textFade2.gameObject.transform.parent.gameObject.SetActive(true);
        for (int i = 0; i < textFade2.text.Length; i++)
        {
            yield return StartCoroutine(
                textFade2.ShowText(i, fadeTime)
            );
            yield return new WaitForSeconds(lineDelay);
        }
        foreach (Transform o in GetAllNestedChildren(textFade2.gameObject.transform.parent.parent))
        {
            if (o.gameObject.GetComponent<Image>() != null)
            {
                StartCoroutine(FadeImageOut(o.gameObject));
            }
        }
        yield return StartCoroutine(textFade2.FadeOut(fadeTime));
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(1);
    }
    private IEnumerator FadeImageOut(GameObject image)
    {
        Color color = image.GetComponent<Image>().color;

        // Fade out
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, t / fadeTime);
            image.GetComponent<Image>().color = color;
            yield return null;
        }

    }
    public static List<Transform> GetAllNestedChildren(Transform parent, bool includeInactive = true)
    {
        List<Transform> children = new List<Transform>();

        if (parent == null)
            return children;

        // Recursive helper method
        void GetChildrenRecursive(Transform current)
        {
            foreach (Transform child in current)
            {
                if (includeInactive || child.gameObject.activeSelf)
                {
                    children.Add(child);
                    GetChildrenRecursive(child); // Recurse into grandchildren
                }
            }
        }

        GetChildrenRecursive(parent);
        return children;
    }
}
