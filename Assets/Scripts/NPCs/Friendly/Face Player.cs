using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public Transform playerTransform;
    public float smoothSpeed = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            Vector3 targetScale = transform.localScale;
            if (playerTransform.position.x < transform.position.x)
            {
                // Player is to the left, face left
                targetScale = new Vector3(-0.25f, 0.25f, 0.25f);
            }
            else
            {
                // Player is to the right, face right
                targetScale = new Vector3(0.25f, 0.25f, 0.25f);
            }
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * smoothSpeed);
        }
    }
}
