using System.Collections;
using UnityEngine;

public class SpriteWispEffect : MonoBehaviour
{
    public float duration = 2f;
    public float upwardDistance = 2f;
    public float horizontalDrift = 0.5f;
    public bool destroyAfter = true;

    public void WispAway()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sprite in sprites)
        {
            StartCoroutine(WispSprite(sprite));
        }
    }

    private IEnumerator WispSprite(SpriteRenderer sprite)
    {
        Vector3 startPos = sprite.transform.position;
        Vector3 endPos = startPos +
                         Vector3.up * upwardDistance +
                         Vector3.right * Random.Range(-horizontalDrift, horizontalDrift);

        Vector3 startScale = sprite.transform.localScale;
        Vector3 endScale = Vector3.zero;

        Color startColor = sprite.color;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Smooth easing
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            sprite.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
            sprite.transform.localScale = Vector3.Lerp(startScale, endScale, smoothT);

            Color c = startColor;
            c.a = Mathf.Lerp(startColor.a, 0f, smoothT);
            sprite.color = c;

            yield return null;
        }

        if (destroyAfter)
            Destroy(sprite.gameObject);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            WispAway();
        }
    }
}