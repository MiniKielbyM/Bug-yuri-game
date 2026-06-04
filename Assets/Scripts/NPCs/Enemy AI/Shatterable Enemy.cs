using UnityEngine;
using System.Collections.Generic;

public class ShatterableEnemy : MonoBehaviour
{
    [Range(2, 10)]
    public int shatterDetail = 4; // Number of subdivisions per axis
    public float explosionForce = 15f;
    public float explosionTorque = 8f;
    public float pieceLifetime = 3f;

    private List<SpriteRenderer> spriteRenderers;
    private bool isShattered = false;

    void Start()
    {
        // Get all SpriteRenderers in this enemy and its children
        spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());

        // Log the enemy's scale
        Debug.Log($"Enemy global scale: {transform.lossyScale}");
        Debug.Log($"Enemy local scale: {transform.localScale}");
    }

    public void Shatter()
    {
        if (isShattered) return;
        isShattered = true;

        // Shatter each sprite
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            ShatterSprite(spriteRenderer);
        }

        // Destroy the original enemy
        Destroy(gameObject);
    }

    private void ShatterSprite(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer.sprite == null)
        {
            Debug.LogWarning("No sprite assigned to SpriteRenderer: " + spriteRenderer.gameObject.name);
            return;
        }

        Texture2D texture = spriteRenderer.sprite.texture;
        Rect spriteRect = spriteRenderer.sprite.textureRect;
        Vector2 pivot = spriteRenderer.sprite.pivot / spriteRect.size;

        // Create a readable copy of the texture
        Texture2D readableTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height, TextureFormat.RGBA32, false);

        if (texture.isReadable)
        {
            Color[] spritePixels = texture.GetPixels(
                (int)spriteRect.x,
                (int)spriteRect.y,
                (int)spriteRect.width,
                (int)spriteRect.height
            );
            readableTexture.SetPixels(spritePixels);
            readableTexture.Apply();
        }
        else
        {
            RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height);
            Graphics.Blit(texture, rt);
            RenderTexture.active = rt;

            Texture2D tempTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            tempTexture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            tempTexture.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            Color[] spritePixels = tempTexture.GetPixels(
                (int)spriteRect.x,
                (int)spriteRect.y,
                (int)spriteRect.width,
                (int)spriteRect.height
            );
            readableTexture.SetPixels(spritePixels);
            readableTexture.Apply();

            Destroy(tempTexture);
        }

        // Calculate piece size
        float pieceWidth = spriteRect.width / shatterDetail;
        float pieceHeight = spriteRect.height / shatterDetail;

        // Account for sprite renderer scale
        float spriteScale = spriteRenderer.transform.lossyScale.x;

        // Create pieces
        for (int x = 0; x < shatterDetail; x++)
        {
            for (int y = 0; y < shatterDetail; y++)
            {
                Color[] pixels = readableTexture.GetPixels(
                    (int)(x * pieceWidth),
                    (int)(y * pieceHeight),
                    (int)pieceWidth,
                    (int)pieceHeight
                );

                Texture2D pieceTex = new Texture2D((int)pieceWidth, (int)pieceHeight, TextureFormat.RGBA32, false);
                pieceTex.SetPixels(pixels);
                pieceTex.Apply();

                // Create GameObject for piece
                GameObject piece = new GameObject("EnemyPiece");

                // Offset piece position based on grid position to spread them out
                float offsetX = (x - shatterDetail / 2f) * (pieceWidth / spriteRenderer.sprite.pixelsPerUnit) * 0.5f;
                float offsetY = (y - shatterDetail / 2f) * (pieceHeight / spriteRenderer.sprite.pixelsPerUnit) * 0.5f;

                piece.transform.position = spriteRenderer.transform.position + new Vector3(offsetX, offsetY, 0);
                piece.transform.rotation = spriteRenderer.transform.rotation;

                SpriteRenderer pieceRenderer = piece.AddComponent<SpriteRenderer>();
                pieceRenderer.sprite = Sprite.Create(
                    pieceTex,
                    new Rect(0, 0, pieceTex.width, pieceTex.height),
                    new Vector2(0.5f, 0.5f),
                    spriteRenderer.sprite.pixelsPerUnit
                );
                pieceRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
                pieceRenderer.sortingOrder = spriteRenderer.sortingOrder;

                // Scale the piece to match the parent sprite renderer
                piece.transform.localScale = new Vector3(spriteScale, spriteScale, 1f);

                // Add physics
                Rigidbody2D rb = piece.AddComponent<Rigidbody2D>();
                rb.constraints = RigidbodyConstraints2D.None;
                rb.linearDamping = 0.5f;
                rb.angularDamping = 0.5f;

                // Apply explosion force
                Vector2 explosionDirection = (piece.transform.position - transform.position).normalized;
                rb.AddForce(explosionDirection * explosionForce, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-explosionTorque, explosionTorque), ForceMode2D.Impulse);

                // Destroy after lifetime
                Destroy(piece, pieceLifetime);
            }
        }

        Destroy(readableTexture);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            Debug.Log("Enemy hit by weapon: " + collision.gameObject.name);
            Shatter();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            Debug.Log("Enemy hit by weapon (collision): " + collision.gameObject.name);
            Shatter();
        }
    }
}
