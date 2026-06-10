using UnityEngine;

public class DirectatWave : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        if (!CompareTag("Untagged"))
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime * -1;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Boss Wall"))
        {
            tag = "Untagged";
            gameObject.GetComponent<SpriteWispEffect>().WispAway();
        }
        if (other.gameObject.CompareTag("Weapon"))
        {
            tag = "Untagged";
        }
    }
}