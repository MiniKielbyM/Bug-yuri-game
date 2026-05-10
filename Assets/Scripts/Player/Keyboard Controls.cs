using UnityEngine;

public class KeyboardControls : MonoBehaviour
{
    public Rigidbody2D rb;
    bool isGrounded = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) && isGrounded == true)
        {
            Debug.Log("Jump");
            rb.AddForce(Vector2.up * 250f, ForceMode2D.Force);
            isGrounded = false;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("Crouch");
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * Time.deltaTime * 5f);
            transform.localScale = new Vector3(0.25f, 0.25f, 0.25f); // Flip the sprite to face left
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Time.deltaTime * 5f);
            transform.localScale = new Vector3(-0.25f, 0.25f, 0.25f); // Flip the sprite to face right
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
