using UnityEngine;

public class GenericEnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stoppingDistance = 0.5f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private string playerTag = "Player";

    [Header("Combat")]
    [SerializeField] private float attackRange = 0.15f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDelayOnReach = 0.5f;
    private float lastAttackTime = 0f;
    private float timeEnteredAttackRange = -999f;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private bool isChasing = false;
    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.FindWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Detect player
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > detectionRange * 1.5f)
        {
            isChasing = false;
        }

        // Chase or attack player
        if (isChasing)
        {
            if (distanceToPlayer > attackRange)
            {
                ChasePlayer();
                timeEnteredAttackRange = -999f; // Reset when distance increases
            }
            else
            {
                // Record when we first enter attack range
                if (timeEnteredAttackRange < 0)
                {
                    timeEnteredAttackRange = Time.time;
                }

                StopMovement();

                // Check if enough time has passed since entering attack range
                if (Time.time >= timeEnteredAttackRange + attackDelayOnReach &&
                    Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                    lastAttackTime = Time.time;
                }
            }
        }
        else
        {
            StopMovement();
            timeEnteredAttackRange = -999f; // Reset when no longer chasing
        }
        if (isFacingRight)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(-0.15f, 0.15f, 1f), Time.deltaTime * 10f);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.15f, 0.15f, 1f), Time.deltaTime * 10f);
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        if (rb != null)
        {
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }

        // Rotate to face player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (direction.x > 0 && !isFacingRight)
        {
            isFacingRight = true;
        }
        else if (direction.x < 0 && isFacingRight)
        {
            isFacingRight = false;
        }
    }
    private void AttackPlayer()
    {
        Debug.Log("Enemy attacking player! Player died!");
        UniversalCanvasAnimationController.FadeOut();
        KeyboardControls playerControls = playerTransform.GetComponent<KeyboardControls>();
        if (playerControls != null)
        {
            playerControls.HandleDeath();
        }
    }

    private void StopMovement()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
