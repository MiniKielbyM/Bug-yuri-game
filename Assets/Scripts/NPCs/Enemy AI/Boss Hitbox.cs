using UnityEngine;
using UnityEngine.Events;

public class BossHitbox : MonoBehaviour
{
    private float lastHitTime;
    public float hitCooldown = 0.5f;
    public UnityEvent onHit;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Weapon") || Time.time - lastHitTime < hitCooldown)
            return;
        lastHitTime = Time.time;
        onHit.Invoke();
    }
}