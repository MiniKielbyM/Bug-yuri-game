using System;
using UnityEngine;

public class KeyboardControls : MonoBehaviour
{
    public Rigidbody2D rb;
    bool isGrounded = true;
    float move = 0f;
    bool isFacingRight = true;
    public GameObject SidewaysSprite;
    public GameObject ForwardSprite;
    public GameObject AttackArm1;
    public GameObject AttackArm2;
    public GameObject WalkArm1;
    public GameObject WalkkArm2;
    public float isAttacking = 0f;
    int attackArm = 1;
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        AttackArm1.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.AddForce(new Vector2(0f, 11.5f * (1f + (Math.Abs(move) / 10f) - 0.1f)), ForceMode2D.Impulse);
            isGrounded = false;
        }
        if (Input.GetMouseButtonDown(0) && isAttacking < 1f)
        {
            isAttacking += 0.5f;
            Debug.Log("Attack initiated");
            if (attackArm == 1)
            {
                attackArm = 2;
                AttackArm1.SetActive(true);
                WalkArm1.SetActive(false);
                animator.SetTrigger("Attack 1");
            }
            else
            {
                attackArm = 1;
                AttackArm2.SetActive(true);
                WalkkArm2.SetActive(false);
                animator.SetTrigger("Attack 1");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {

        float acceleration = 20f;
        float maxSpeed = 5f;
        float groundDrag = 10f;

        if (isFacingRight)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(-0.25f, 0.25f, 0.25f), Time.deltaTime * 10f);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.25f, 0.25f, 0.25f), Time.deltaTime * 10f);
        }
        // Input
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            ForwardSprite.SetActive(false);
            SidewaysSprite.SetActive(true);
            move = -1f;
            isFacingRight = false;
            animator.SetInteger("State", 1);
        }
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            ForwardSprite.SetActive(false);
            SidewaysSprite.SetActive(true);
            move = -2f;
            isFacingRight = false;
            animator.SetInteger("State", 2);
        }
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            ForwardSprite.SetActive(false);
            SidewaysSprite.SetActive(true);
            move = 1f;
            isFacingRight = true;
            animator.SetInteger("State", 1);
        }
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            ForwardSprite.SetActive(false);
            SidewaysSprite.SetActive(true);
            move = 2f;
            isFacingRight = true;
            animator.SetInteger("State", 2);
        }
        if (Input.GetKey(KeyCode.D) && !isGrounded)
        {
            ForwardSprite.SetActive(false);
            SidewaysSprite.SetActive(true);
            move = 0.75f;
            isFacingRight = true;
            animator.SetInteger("State", 1);
        }
        if (Input.GetKey(KeyCode.A) && !isGrounded)
        {
            ForwardSprite.SetActive(false);
            SidewaysSprite.SetActive(true);
            move = -0.75f;
            isFacingRight = false;
            animator.SetInteger("State", 1);
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            move = 0f;
            animator.SetInteger("State", 0);
            ForwardSprite.SetActive(true);
            SidewaysSprite.SetActive(false);
        }
        // Apply movement force
        rb.AddForce(new Vector2(move * acceleration, 0f));

        // Apply drag when no movement key is pressed
        if (move == 0f)
        {
            rb.linearVelocity = new Vector2(
                Mathf.Lerp(rb.linearVelocity.x, 0f, groundDrag * Time.fixedDeltaTime),
                rb.linearVelocity.y
            );
        }
        if (isAttacking > 0f)
        {
            ForwardSprite.SetActive(false);
            SidewaysSprite.SetActive(true);
        }
        // Clamp speed
        rb.linearVelocity = new Vector2(
            Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed),
            rb.linearVelocity.y
        );
    }
    public void EndAttack(int arm)
    {
        isAttacking -= 0.5f;
        animator.ResetTrigger("Attack 1");
        animator.ResetTrigger("Attack 2");
        if (arm == 1)
        {
            AttackArm1.SetActive(false);
            WalkArm1.SetActive(true);
        }
        else
        {
            AttackArm2.SetActive(false);
            WalkkArm2.SetActive(true);
        }
    }
}
